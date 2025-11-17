namespace MusicManagementDemo.Application.Responses;

public sealed record ApiResult<T>(bool IsFinish, int? Code, T? Data, string? ErrorMessage)
    where T : class
{
    public static ApiResult<T> Ok(T? data = null)
    {
        return new ApiResult<T>(IsFinish: true, Code: 200, Data: data, ErrorMessage: null);
    }

    public static ApiResult<T> Err(int code, string? errorMessage = null)
    {
        return new ApiResult<T>(
            IsFinish: true,
            Code: code,
            Data: null,
            ErrorMessage: errorMessage
        );
    }
}
