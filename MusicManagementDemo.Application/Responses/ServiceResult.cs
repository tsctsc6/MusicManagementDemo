using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Application.Responses;

public sealed record ServiceResult(
    bool IsFinish,
    int? Code,
    object? Data,
    IReadOnlyList<string>? Errors
) : IServiceResult
{
    public static ServiceResult Ok(object? data = null)
    {
        return new ServiceResult(IsFinish: true, Code: 200, Data: data, Errors: null);
    }

    public static ServiceResult Err(int code, IReadOnlyList<string>? errors = null)
    {
        return new ServiceResult(IsFinish: true, Code: code, Data: null, Errors: errors);
    }

    /// <summary>
    /// 表示业务仍未完成，还有后续步骤
    /// </summary>
    /// <returns></returns>
    public static ServiceResult Suspend()
    {
        return new ServiceResult(IsFinish: false, Code: 200, Data: null, Errors: null);
    }
}
