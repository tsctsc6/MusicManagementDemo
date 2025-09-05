namespace MusicManagementDemo.Infrastructure.Responses;

internal sealed record ServiceResult(
    bool IsFinish,
    int? Code,
    IReadOnlyDictionary<string, string[]>? Errors
) : IServiceResult
{
    public static ServiceResult Ok()
    {
        return new ServiceResult(IsFinish: true, Code: 200, Errors: null);
    }

    public static ServiceResult Err(int code, IReadOnlyDictionary<string, string[]>? errors = null)
    {
        return new ServiceResult(IsFinish: true, Code: code, Errors: errors);
    }

    public static ServiceResult Suspend()
    {
        return new ServiceResult(IsFinish: false, Code: 200, Errors: null);
    }
}

internal sealed record ServiceResult<T>(
    bool IsFinish,
    int? Code,
    IReadOnlyDictionary<string, string[]>? Errors,
    T? Data
) : IServiceResult
{
    public static ServiceResult<T> Ok(T data)
    {
        return new ServiceResult<T>(IsFinish: true, Code: 200, Errors: null, Data: data);
    }

    public static ServiceResult<T> Err(int code, IReadOnlyDictionary<string, string[]>? errors = null)
    {
        return new ServiceResult<T>(IsFinish: true, Code: code, Errors: errors, Data: default);
    }

    public static ServiceResult<T> Suspend(T? data)
    {
        return new ServiceResult<T>(IsFinish: false, Code: 200, Errors: null, Data: data);
    }
}
