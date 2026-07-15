using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace CareerFlow.Api.Extensions;

/// <summary>
/// Enricher que adiciona o IP do cliente aos logs
/// Funciona com injeção de dependência via HttpContextAccessor
/// </summary>
public class ClientIpEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    // Construtor público sem parâmetros (exigido pelo Serilog)
    public ClientIpEnricher() : this(null) { }

    // Construtor com injeção de dependência
    public ClientIpEnricher(IHttpContextAccessor? httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor?.HttpContext;

        var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var property = new LogEventProperty("ClientIp", new ScalarValue(ipAddress));
        logEvent.AddPropertyIfAbsent(property);
    }
}

/// <summary>
/// Enricher que adiciona o User-Agent aos logs
/// </summary>
public class ClientAgentEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    // Construtor público sem parâmetros (exigido pelo Serilog)
    public ClientAgentEnricher() : this(null) { }

    // Construtor com injeção de dependência
    public ClientAgentEnricher(IHttpContextAccessor? httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor?.HttpContext;

        var userAgent = httpContext?.Request.Headers.UserAgent.ToString() ?? "unknown";

        // Trunca para evitar logs muito longos
        if (userAgent.Length > 200)
            userAgent = userAgent[..200] + "...";

        var property = new LogEventProperty("ClientAgent", new ScalarValue(userAgent));
        logEvent.AddPropertyIfAbsent(property);
    }
}

/// <summary>
/// Extensões para registro dos enrichers com suporte a DI
/// </summary>
public static class SerilogEnricherExtensions
{
    /// <summary>
    /// Adiciona o IP do cliente ao log
    /// </summary>
    public static LoggerConfiguration WithClientIp(
        this LoggerEnrichmentConfiguration enrich,
        IHttpContextAccessor? httpContextAccessor = null)
    {
        return enrich.With(new ClientIpEnricher(httpContextAccessor));
    }

    /// <summary>
    /// Adiciona o User-Agent ao log
    /// </summary>
    public static LoggerConfiguration WithClientAgent(
        this LoggerEnrichmentConfiguration enrich,
        IHttpContextAccessor? httpContextAccessor = null)
    {
        return enrich.With(new ClientAgentEnricher(httpContextAccessor));
    }
}
