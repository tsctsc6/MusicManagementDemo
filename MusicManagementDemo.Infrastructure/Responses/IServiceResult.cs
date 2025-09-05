namespace MusicManagementDemo.Infrastructure.Responses;

internal interface IServiceResult
{
    bool IsFinish { get; }

    int? Code { get; }

    IReadOnlyDictionary<string, string[]>? Errors { get; }
}
