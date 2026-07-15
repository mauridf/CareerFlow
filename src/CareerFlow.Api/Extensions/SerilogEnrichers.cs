using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace CareerFlow.Api.Extensions;

/// <summary>
/// Enricher que adiciona o IP do cliente aos logs
/// </summary>
public class ClientIpEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientIpEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return;

        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var property = propertyFactory.CreateProperty("ClientIp", ipAddress);
        logEvent.AddPropertyIfAbsent(property);
    }
}

/// <summary>
/// Enricher que adiciona o User-Agent aos logs
/// </summary>
public class ClientAgentEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientAgentEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return;

        var userAgent = httpContext.Request.Headers.UserAgent.ToString();
        // Trunca para evitar logs muito longos
        if (userAgent.Length > 200)
            userAgent = userAgent[..200] + "...";

        var property = propertyFactory.CreateProperty("ClientAgent", userAgent);
        logEvent.AddPropertyIfAbsent(property);
    }
}

/// <summary>
/// Extensões para registro dos enrichers
/// </summary>
public static class SerilogEnricherExtensions
{
    /// <summary>
    /// Adiciona o IP do cliente ao log
    /// </summary>
    public static LoggerConfiguration WithClientIp(this LoggerEnrichmentConfiguration enrich)
    {
        return enrich.With<ClientIpEnricher>();
    }

    /// <summary>
    /// Adiciona o User-Agent ao log
    /// </summary>
    public static LoggerConfiguration WithClientAgent(this LoggerEnrichmentConfiguration enrich)
    {
        return enrich.With<ClientAgentEnricher>();
    }
}
