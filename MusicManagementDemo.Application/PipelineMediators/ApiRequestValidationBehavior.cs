using FluentValidation;
using Mediator;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.PipelineMediators;

internal sealed class ApiRequestValidationBehavior<TRequest, T>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ApiRequestValidationBehavior<TRequest, T>> logger
) : IPipelineBehavior<TRequest, ApiResult<T>>
    where TRequest : IRequest<ApiResult<T>>
    where T : class
{
    public async ValueTask<ApiResult<T>> Handle(
        TRequest request,
        MessageHandlerDelegate<TRequest, ApiResult<T>> next,
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
            string[] errorMessages = [.. failures.Select(f => f.ErrorMessage)];
            // ReSharper disable once CoVariantArrayConversion
            logger.LogError("Validation failed: {@errorMessages}", errorMessages);
            return ApiResult<T>.Err(406, string.Join("\n", errorMessages));
        }

        return await next(request, cancellationToken);
    }
}
