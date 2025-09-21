using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json.Nodes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Domain.DomainEvents;
using MusicManagementDemo.Domain.Entity.Management;
using RustSharp;

namespace MusicManagementDemo.Application.JobHandlers;

internal sealed class JobManager(
    IServiceProvider services,
    IMusicInfoParser musicInfoParser,
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

        await using var scope = services.CreateAsyncScope();
        await using var dbContext =
            scope.ServiceProvider.GetRequiredService<IManagementAppDbContext>();
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
            await using var scope = services.CreateAsyncScope();
            await using var dbContext =
                scope.ServiceProvider.GetRequiredService<IManagementAppDbContext>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var job = await dbContext
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
            var storage = await dbContext
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

            // 分批写入数据库
            var tasks = fileEnumerator
                .EnumerateFiles(
                    new DirectoryInfo(storage.Path),
                    "*.flac",
                    SearchOption.AllDirectories
                )
                .Select(async f => await musicInfoParser.ParseMusicInfoAsync(f, storage.Id, storage.Path, token))
                .Chunk(500);
            foreach (var task in tasks)
            {
                var results = await Task.WhenAll(task);
                var items = results
                    .OfType<OkResult<MusicFileFoundEventItem, string>>()
                    .Select(r => r.Value);
                await mediator.Publish(new MusicFileFoundEvent(items), token);
            }
            return Result.Ok(0);
        }
        catch (Exception e) when (e is TaskCanceledException or OperationCanceledException)
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
        await using var scope = services.CreateAsyncScope();
        await using var dbContext =
            scope.ServiceProvider.GetRequiredService<IManagementAppDbContext>();
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
