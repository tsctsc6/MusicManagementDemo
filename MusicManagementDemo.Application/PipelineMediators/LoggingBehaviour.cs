using MediatR;
using Microsoft.Extensions.Logging;

namespace MusicManagementDemo.Application.PipelineMediators;

internal sealed class LoggingBehaviour<TRequest, TResponse>(
    ILogger<LoggingBehaviour<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Request - {@request}", request);
        var response = await next(cancellationToken);
        logger.LogInformation("Response - {@response}", response);
        return response;
    }
}
