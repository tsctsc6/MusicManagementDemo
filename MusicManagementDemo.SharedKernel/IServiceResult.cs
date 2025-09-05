namespace MusicManagementDemo.SharedKernel;

public interface IServiceResult
{
    bool IsFinish { get; }

    int? Code { get; }

    IReadOnlyList<string>? Errors { get; }
}
