using Mediator;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.PipelineMediators;

internal sealed class ExceptionHandlingBehavior<TRequest, TResponse>(
    ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async ValueTask<TResponse> Handle(
        TRequest request,
        MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return await next(request, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred: ");
            return (TResponse)(object)ServiceResult.Err(503, [ex.Message]);
        }
    }
}
