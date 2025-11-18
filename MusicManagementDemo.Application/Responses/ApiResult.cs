namespace MusicManagementDemo.Application.Responses;

public interface IApiResult
{
    public IApiResult CreateError(int code, string? errorMessage = null);
}

public record ApiResult<T> : IApiResult
    where T : class
{
    public bool IsFinish { get; init; } = false;

    public int Code { get; init; } = 0;

    public T? Data { get; init; } = null;

    public string? ErrorMessage { get; init; } = null;

    public ApiResult() { }

    private ApiResult(bool isFinish, int code, T? data, string? errorMessage)
    {
        IsFinish = isFinish;
        Code = code;
        Data = data;
        ErrorMessage = errorMessage;
    }

    public static ApiResult<T> Ok(T data)
    {
        return new ApiResult<T>(true, 200, data, null);
    }

    public static ApiResult<T> Err(int code, string? errorMessage = null)
    {
        return new ApiResult<T>(true, code, null, errorMessage);
    }

    public IApiResult CreateError(int code, string? errorMessage = null)
    {
        return Err(code, errorMessage);
    }
}
