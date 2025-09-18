using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.PipelineMediators;

internal sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
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
            return (TResponse)(object)ServiceResult.Err(406, errorMessages);
        }

        return await next(cancellationToken);
    }
}
