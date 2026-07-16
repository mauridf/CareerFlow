using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using CareerFlow.Application.Common.Behaviors;

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

            // Behaviors (pipeline)
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
        // Serviços de Aplicação
        // ============================================
        services.AddScoped(typeof(Common.Interfaces.ILoggerService<>), typeof(Common.Interfaces.LoggerService<>));

        return services;
    }
}
