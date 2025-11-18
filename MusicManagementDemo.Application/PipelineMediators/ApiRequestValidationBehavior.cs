using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using Mediator;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.PipelineMediators;

internal sealed class ApiRequestValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IApiResult, new()
{
    public async ValueTask<TResponse> Handle(
        TRequest request,
        MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var context = new ValidationContext<TRequest>(request);

        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToArray();

        if (failures.Length != 0)
        {
            return CreateErrorResponse(failures);
        }

        return await next(request, cancellationToken);
    }

    private TResponse CreateErrorResponse(ValidationFailure[] failures)
    {
        string[] errorMessages = [.. failures.Select(f => f.ErrorMessage)];
        return (TResponse)new TResponse().CreateError(406, string.Join("\n", errorMessages));
    }

    /*private TResponse CreateErrorResponseByReflection(ValidationFailure[] failures)
    {
        var responseType = typeof(TResponse);
        if (
            !responseType.IsGenericType
            || responseType.GetGenericTypeDefinition() != typeof(ApiResult<>)
        )
        {
            throw new InvalidOperationException("TResponse 必须是 ApiResult<T> 的泛型类型");
        }
        var errMethod = responseType.GetMethod(
            "Err",
            BindingFlags.Public | BindingFlags.Static,
            [typeof(int), typeof(string)]
        );
        string[] errorMessages = [.. failures.Select(f => f.ErrorMessage)];
        if (errMethod == null)
            throw new InvalidOperationException("找不到 ApiResult<T>.Err(string) 方法");
        return (TResponse)errMethod.Invoke(null, [406, string.Join("\n", errorMessages)])!;
    }*/
}
