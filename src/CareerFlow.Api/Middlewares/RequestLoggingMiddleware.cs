using System.Diagnostics;
using Serilog;
using Serilog.Context;

namespace CareerFlow.Api.Middlewares;

/// <summary>
/// Middleware que adiciona informações detalhadas de request/response aos logs
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var correlationId = context.TraceIdentifier;

        // Adiciona CorrelationId ao contexto do log
        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("RequestMethod", context.Request.Method))
        using (LogContext.PushProperty("RequestPath", context.Request.Path))
        {
            try
            {
                // Log do request
                Log.Debug(
                    "➡️ Request {Method} {Path}{QueryString} iniciado | IP: {IP} | UA: {UserAgent}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.QueryString,
                    context.Connection.RemoteIpAddress,
                    context.Request.Headers.UserAgent);

                await _next(context);

                stopwatch.Stop();

                // Log do response
                var logLevel = context.Response.StatusCode >= 400
                    ? Serilog.Events.LogEventLevel.Warning
                    : Serilog.Events.LogEventLevel.Information;

                Log.Write(logLevel,
                    "⬅️ Response {Method} {Path} | {StatusCode} | {ElapsedMs}ms | {ContentType}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.Elapsed.TotalMilliseconds.ToString("F2"),
                    context.Response.ContentType);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                Log.Error(ex,
                    "❌ Request {Method} {Path} FALHOU após {ElapsedMs}ms | Erro: {ErrorMessage}",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.Elapsed.TotalMilliseconds.ToString("F2"),
                    ex.Message);

                throw;
            }
        }
    }
}

/// <summary>
/// Extensão para registro do middleware
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
