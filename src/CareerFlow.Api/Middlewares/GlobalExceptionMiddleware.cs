using System.Net;
using System.Text.Json;
using CareerFlow.Application.Common.DTOs;
using CareerFlow.Core.Exceptions;

namespace CareerFlow.Api.Middlewares;

/// <summary>
/// Middleware global para tratamento de exceções.
/// Converte exceções de domínio em respostas HTTP apropriadas.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();

        switch (exception)
        {
            case ValidationException validationEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Error = new ErrorDetail
                {
                    Code = "VALIDATION_ERROR",
                    Message = "Um ou mais erros de validação ocorreram",
                    Details = validationEx.Errors?
                        .SelectMany(e => e.Value.Select(m => new FieldError { Field = e.Key, Message = m }))
                        .ToList()
                };
                _logger.LogWarning(exception, "⚠️ Erro de validação");
                break;

            case NotFoundException notFoundEx:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Error = new ErrorDetail
                {
                    Code = "NOT_FOUND",
                    Message = notFoundEx.Message
                };
                _logger.LogInformation(exception, "🔍 Recurso não encontrado");
                break;

            case UnauthorizedException unauthorizedEx:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Error = new ErrorDetail
                {
                    Code = "UNAUTHORIZED",
                    Message = unauthorizedEx.Message
                };
                _logger.LogWarning(exception, "🔒 Acesso não autorizado");
                break;

            case ConflictException conflictEx:
                response.StatusCode = (int)HttpStatusCode.Conflict;
                errorResponse.Error = new ErrorDetail
                {
                    Code = "CONFLICT",
                    Message = conflictEx.Message
                };
                _logger.LogWarning(exception, "⚠️ Conflito");
                break;

            case DomainException domainEx:
                response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                errorResponse.Error = new ErrorDetail
                {
                    Code = domainEx.ErrorCode ?? "DOMAIN_ERROR",
                    Message = domainEx.Message
                };
                _logger.LogWarning(exception, "⚠️ Erro de domínio");
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Error = new ErrorDetail
                {
                    Code = "INTERNAL_ERROR",
                    Message = _env.IsDevelopment()
                        ? exception.Message
                        : "Ocorreu um erro interno. Tente novamente mais tarde."
                };
                _logger.LogError(exception, "❌ Erro interno não tratado");
                break;
        }

        errorResponse.Meta = new MetaResponse
        {
            Timestamp = DateTime.UtcNow,
            RequestId = context.TraceIdentifier
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(errorResponse, jsonOptions);
        await response.WriteAsync(json);
    }
}

/// <summary>
/// Extensão para registro do middleware
/// </summary>
public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
