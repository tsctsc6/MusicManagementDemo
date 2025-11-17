namespace MusicManagementDemo.Application.Responses;

public sealed record ServiceResult<T>(bool IsFinish, int? Code, T? Data, string? ErrorMessage)
    where T : class
{
    public static ServiceResult<T> Ok(T? data = null)
    {
        return new ServiceResult<T>(IsFinish: true, Code: 200, Data: data, ErrorMessage: null);
    }

    public static ServiceResult<T> Err(int code, string? errorMessage = null)
    {
        return new ServiceResult<T>(
            IsFinish: true,
            Code: code,
            Data: null,
            ErrorMessage: errorMessage
        );
    }
}
