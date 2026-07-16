using MediatR;
using Microsoft.Extensions.Logging;

namespace CareerFlow.Application.Common.Behaviors;

/// <summary>
/// Behavior que loga todas as requests e responses do MediatR.
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogDebug("▶️ Executando {RequestName}: {@Request}", requestName, request);

        var response = await next();

        _logger.LogDebug("✅ {RequestName} concluído: {@Response}", requestName, response);

        return response;
    }
}
