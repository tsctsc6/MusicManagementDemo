namespace MusicManagementDemo.SharedKernel;

public interface IServiceResult
{
    bool IsFinish { get; }

    int? Code { get; }

    object? Data { get; }

    IReadOnlyList<string>? Errors { get; }
}
