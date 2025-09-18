using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Domain.Entity.Management;
using MusicManagementDemo.Infrastructure.Database;
using RustSharp;

namespace MusicManagementDemo.Infrastructure.JobHandler;

internal sealed class JobManager(
    IServiceProvider service,
    IFileEnumerator fileEnumerator,
    ILogger<JobManager> logger
) : IJobManager
{
    private readonly ConcurrentDictionary<long, CancellationTokenSource> cancellationTokenSources =
    [];

    public async Task<Result<long, string>> CreateJobAsync(
        long jobId,
        JobType jobType,
        CancellationToken cancellationToken = default
    )
    {
        var cts = new CancellationTokenSource();
        switch (jobType)
        {
            case JobType.ScanIncremental:
                // jobId 来自数据库，应该是不会重复的
                if (!cancellationTokenSources.TryAdd(jobId, cts))
                {
                    logger.LogError("Can't add CancellationTokenSource");
                    return Result.Err("创建任务失败");
                }
                _ = HandleScanIncrementalAsync(jobId, cts.Token)
                    .ContinueWith(async task =>
                    {
                        cancellationTokenSources.TryRemove(jobId, out cts);
                        cts?.Dispose();
                        await HandleScanIncrementalContinueAsync(jobId, task.Result);
                        task.Dispose();
                    });
                break;
            case JobType.Undefined:
            default:
                logger.LogError("Unknown job type: {jobType}", jobType);
                return Result.Err("Unknown job type");
        }
        await using var scope = service.CreateAsyncScope();
        await using var dbContext =
            scope.ServiceProvider.GetRequiredService<ManagementAppDbContext>();
        var jobToUpdate = await dbContext.Job.SingleOrDefaultAsync(
            e => e.Id == jobId,
            // ReSharper disable once PossiblyMistakenUseOfCancellationToken
            cancellationToken: cancellationToken
        );
        if (jobToUpdate is null)
            return Result.Err("Job not found");
        jobToUpdate.Status = JobStatus.Running;
        dbContext.Job.Update(jobToUpdate);
        // ReSharper disable once PossiblyMistakenUseOfCancellationToken
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok(jobToUpdate.Id);
    }

    public async Task<Result<long, string>> CancelJobAsync(long jobId)
    {
        cancellationTokenSources.TryGetValue(jobId, out var cts);
        if (cts is null)
        {
            return Result.Err("Can't cancel job");
        }
        await cts.CancelAsync();
        return Result.Ok(jobId);
    }

    /// <summary>
    /// 扫描某个存储中的音乐文件
    /// </summary>
    /// <param name="jobId"></param>
    /// <param name="token"></param>
    /// <returns>
    /// <para>OkResult:</para>
    /// <para>0: Success</para>
    /// <para>1: Interrupt</para>
    /// <para>ErrResult:</para>
    /// <para>Error Message</para>
    /// </returns>
    private async Task<Result<int, string>> HandleScanIncrementalAsync(
        long jobId,
        CancellationToken token
    )
    {
        try
        {
            //await Task.Delay(TimeSpan.FromSeconds(10), token);
            await using var scope = service.CreateAsyncScope();
            var managementDbContext =
                scope.ServiceProvider.GetRequiredService<ManagementAppDbContext>();
            var musicDbContext = scope.ServiceProvider.GetRequiredService<MusicAppDbContext>();
            var job = await managementDbContext
                .Job.AsNoTracking()
                .SingleOrDefaultAsync(e => e.Id == jobId, cancellationToken: token);
            if (job is null)
            {
                return Result.Err("Job not found");
            }
            var storageId = job.JobArgs["storageId"]?.GetValue<int>();
            if (storageId is null)
            {
                return Result.Err("StorageId not found in JobArgs");
            }
            var storage = await managementDbContext
                .Storage.AsNoTracking()
                .SingleOrDefaultAsync(e => e.Id == storageId, cancellationToken: token);
            if (storage is null)
            {
                return Result.Err($"Storage {storageId} not found");
            }

            if (!Directory.Exists(storage.Path))
            {
                return Result.Err($"storage.Path {storage.Path} not found");
            }

            await using var transaction = await musicDbContext.Database.BeginTransactionAsync(
                token
            );

            foreach (
                var fileFullPath in fileEnumerator.EnumerateFiles(
                    new DirectoryInfo(storage.Path),
                    "*.flac",
                    SearchOption.AllDirectories
                )
            )
            {
                var ffprobeProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffprobe",
                        Arguments =
                            $"""-v error -i "{fileFullPath}" -print_format json -show_format""",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    },
                };
                ffprobeProcess.Start();
                await ffprobeProcess.WaitForExitAsync(token);
                var result = await ffprobeProcess.StandardOutput.ReadToEndAsync(token);
                ffprobeProcess.Close();
                ffprobeProcess.Dispose();

                var resultJsonNode = JsonNode.Parse(result);
                if (resultJsonNode is null)
                {
                    return Result.Err("Can't parse ffprobe output to JsonNode");
                }
                var resultFormatJsonObject = resultJsonNode["format"]?.AsObject();
                if (resultFormatJsonObject is null)
                {
                    return Result.Err("Can't find \"format\" in ffprobe output");
                }
                var resultFormatTagsJsonObject = resultFormatJsonObject["tags"]?.AsObject();
                if (resultFormatTagsJsonObject is null)
                {
                    return Result.Err("Can't find \"format:tags\" in ffprobe output");
                }
                var title = resultFormatTagsJsonObject["title"]?.GetValue<string>() ?? string.Empty;
                var artist =
                    resultFormatTagsJsonObject["artist"]?.GetValue<string>() ?? string.Empty;
                var album = resultFormatTagsJsonObject["album"]?.GetValue<string>() ?? string.Empty;
                var filePath = Path.GetRelativePath(storage.Path, fileFullPath);
                var oldMusicInfo = await musicDbContext
                    .MusicInfo.Where(e =>
                        e.Title == title && e.Artist == artist && e.Album == album
                    )
                    .SingleOrDefaultAsync(cancellationToken: token);
                if (oldMusicInfo is null)
                {
                    await musicDbContext.MusicInfo.AddAsync(
                        new()
                        {
                            Title = title,
                            Artist = artist,
                            Album = album,
                            FilePath = filePath,
                            StorageId = storage.Id,
                        },
                        cancellationToken: token
                    );
                }
                else
                {
                    oldMusicInfo.FilePath = filePath;
                }
            }
            await musicDbContext.SaveChangesAsync(token);
            await transaction.CommitAsync(token);
            return Result.Ok(0);
        }
        catch (Exception e) when (e is TaskCanceledException || e is OperationCanceledException)
        {
            return Result.Ok(1);
        }
        catch (Exception e)
        {
            return Result.Err(e.Message);
        }
    }

    private async Task HandleScanIncrementalContinueAsync(
        long jobId,
        Result<int, string> taskResult
    )
    {
        await using var scope = service.CreateAsyncScope();
        await using var dbContext =
            scope.ServiceProvider.GetRequiredService<ManagementAppDbContext>();
        var jobToUpdate = dbContext.Job.SingleOrDefault(e => e.Id == jobId);
        if (jobToUpdate is null)
        {
            logger.LogError("Job {JobId} not find", jobId);
            return;
        }
        switch (taskResult)
        {
            case OkResult<int, string> okResult:
                switch (okResult.Value)
                {
                    case 0:
                        jobToUpdate.Success = true;
                        break;
                    case 1:
                        jobToUpdate.Success = false;
                        break;
                    default:
                        logger.LogError("Unknown value: {okResultValue}", okResult.Value);
                        break;
                }
                break;
            case ErrResult<int, string> errResult:
                jobToUpdate.Success = false;
                jobToUpdate.ErrorMesage = errResult.Value;
                break;
            default:
                logger.LogError("Unknown taskResult type: {taskResult}", taskResult);
                break;
        }

        jobToUpdate.Status = JobStatus.Completed;
        jobToUpdate.CompletedAt = DateTime.UtcNow;
        dbContext.Job.Update(jobToUpdate);
        await dbContext.SaveChangesAsync();
    }
}
