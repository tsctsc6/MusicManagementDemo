using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicManagementDemo.Infrastructure.Database;
using MusicManagementDemo.SharedKernel;
using RustSharp;

namespace MusicManagementDemo.Infrastructure.JobHandler;

internal sealed class JobManager(IServiceProvider service) : IJobManager
{
    private readonly ConcurrentDictionary<long, CancellationTokenSource> cancellationTokenSources =
    [];

    public Result<long, string> CreateJob(long jobId, JobType jobType)
    {
        var cts = new CancellationTokenSource();
        switch (jobType)
        {
            case JobType.ScanIncremental:
                // jobId 来自数据库，应该是不会重复的
                cancellationTokenSources.TryAdd(jobId, cts);
                HandleScanIncremental(jobId, cts.Token)
                    .ContinueWith(task =>
                    {
                        using var scope = service.CreateScope();
                        using var dbContext =
                            scope.ServiceProvider.GetRequiredService<ManagementAppDbContext>();
                        var jobToUpdate = dbContext.Job.SingleOrDefault(e => e.Id == jobId);
                        if (jobToUpdate is null)
                            // log
                            return;
                        if (task.IsCompleted)
                        {
                            jobToUpdate.Success = task.IsCompletedSuccessfully;
                        }
                        else if (task.IsFaulted)
                        {
                            jobToUpdate.ErrorMesage = task.Exception.Message;
                            jobToUpdate.Success = false;
                        }
                        else if (task.IsCanceled)
                        {
                            jobToUpdate.Success = false;
                        }

                        jobToUpdate.Status = JobStatus.Completed;
                        jobToUpdate.CompletedAt = DateTime.UtcNow;
                        dbContext.Job.Update(jobToUpdate);
                        dbContext.SaveChanges();
                        task.Dispose();
                        cts.Dispose();
                    });
                break;
            case JobType.Undefined:
            default:
                return Result.Err("Unknown job type");
        }
        using var scope = service.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<ManagementAppDbContext>();
        var jobToUpdate = dbContext.Job.SingleOrDefault(e => e.Id == jobId);
        if (jobToUpdate is null)
            return Result.Err("Job not found");
        jobToUpdate.Status = JobStatus.Running;
        dbContext.Job.Update(jobToUpdate);
        dbContext.SaveChanges();
        return Result.Ok<long>(jobToUpdate.Id);
    }

    public Result<long, string> CancelJob(long jobId)
    {
        cancellationTokenSources.TryGetValue(jobId, out var cts);
        if (cts is null)
        {
            return Result.Err("Can't cancel job");
        }
        cts.Cancel();
        return Result.Ok(jobId);
    }

    private async Task HandleScanIncremental(long jobId, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromSeconds(30), token);
        await using var scope = service.CreateAsyncScope();
        var managementDbContext =
            scope.ServiceProvider.GetRequiredService<ManagementAppDbContext>();
        var musicDbContext = scope.ServiceProvider.GetRequiredService<MusicAppDbContext>();
        var job = await managementDbContext.Job.AsNoTracking<Domain.Entity.Management.Job>().SingleOrDefaultAsync(
            e => e.Id == jobId,
            cancellationToken: token
        );
        if (job is null)
            throw new InvalidOperationException("Job not found");
        var storageId = job.JobArgs["storageId"]?.GetValue<int>();
        if (storageId is null)
        {
            throw new InvalidOperationException($"StorageId not found in JobArgs");
        }
        var storage = await managementDbContext.Storage.AsNoTracking().SingleOrDefaultAsync(
            e => e.Id == storageId,
            cancellationToken: token
        );
        if (storage is null)
        {
            throw new InvalidOperationException($"Storage {storageId} not found");
        }

        if (!Directory.Exists(storage.Path))
        {
            throw new InvalidOperationException($"storage.Path {storage.Path} not found");
        }

        await using var transaction = await musicDbContext.Database.BeginTransactionAsync(token);
        
        var rootDir = new DirectoryInfo(storage.Path);
        foreach (var fileInfo in rootDir.EnumerateFiles("*.flac", SearchOption.AllDirectories))
        {
            var ffprobeProcess = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "ffprobe",
                    Arguments = $"""-v error -i "{fileInfo.FullName}" -print_format json -show_format""",
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
                throw new InvalidOperationException("Can't parse ffprobe output to JsonNode");
            var resultTagsJsonObject = resultJsonNode["tags"]?.AsObject();
            if (resultTagsJsonObject is null)
                throw new InvalidOperationException("Can't find \"tags\" in ffprobe output");
            await musicDbContext.MusicInfo.AddAsync(new()
            {
                Title = resultTagsJsonObject["title"]?.GetValue<string>() ?? string.Empty,
                Artist = resultTagsJsonObject["artist"]?.GetValue<string>() ?? string.Empty,
                Album = resultTagsJsonObject["album"]?.GetValue<string>() ?? string.Empty,
                FilePath = fileInfo.FullName,
            }, cancellationToken: token);
        }
        await musicDbContext.SaveChangesAsync(token);
        await transaction.CommitAsync(token);
    }
}
