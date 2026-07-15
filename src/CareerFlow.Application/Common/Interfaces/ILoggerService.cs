using Microsoft.Extensions.Logging;

namespace CareerFlow.Application.Common.Interfaces;

/// <summary>
/// Interface para logging estruturado na camada de aplicação
/// </summary>
public interface ILoggerService<T>
{
    void LogInformation(string message, params object?[] args);
    void LogWarning(string message, params object?[] args);
    void LogError(Exception exception, string message, params object?[] args);
    void LogDebug(string message, params object?[] args);
    void LogCritical(string message, params object?[] args);

    IDisposable? BeginScope<TState>(TState state) where TState : notnull;
}

/// <summary>
/// Implementação do logger tipado usando Microsoft.Extensions.Logging
/// </summary>
public class LoggerService<T> : ILoggerService<T>
{
    private readonly ILogger<T> _logger;

    public LoggerService(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message, params object?[] args)
        => _logger.LogInformation(message, args);

    public void LogWarning(string message, params object?[] args)
        => _logger.LogWarning(message, args);

    public void LogError(Exception exception, string message, params object?[] args)
        => _logger.LogError(exception, message, args);

    public void LogDebug(string message, params object?[] args)
        => _logger.LogDebug(message, args);

    public void LogCritical(string message, params object?[] args)
        => _logger.LogCritical(message, args);

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        => _logger.BeginScope(state);
}
