namespace MusicManagementDemo.Application.Responses;

public record ApiResult<T>(bool IsFinish, int? Code, T? Data, string? ErrorMessage)
    where T : class
{
    public static ApiResult<T> Ok(T data)
    {
        return new ApiResult<T>(true, 200, data, null);
    }

    public static ApiResult<T> Err(int code, string? errorMessage = null)
    {
        return new ApiResult<T>(true, code, null, errorMessage);
    }
}
