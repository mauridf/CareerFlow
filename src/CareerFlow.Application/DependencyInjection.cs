using System.Reflection;
using CareerFlow.Application.Common.Behaviors;
using CareerFlow.Application.Common.Interfaces;
using CareerFlow.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CareerFlow.Application;

/// <summary>
/// Registro de serviços da camada de Aplicação.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adiciona os serviços de aplicação ao container DI.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // ============================================
        // MediatR (CQRS)
        // ============================================
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // ============================================
        // FluentValidation
        // ============================================
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // ============================================
        // AutoMapper
        // ============================================
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // ============================================
        // Serviços de Auth
        // ============================================
        services.AddScoped<IPasswordHasher, PasswordService>();
        services.AddScoped<ITokenService, TokenService>();

        // ============================================
        // Logger
        // ============================================
        services.AddScoped(typeof(ILoggerService<>), typeof(LoggerService<>));

        return services;
    }
}
