using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CareerFlow.Core.Interfaces;
using CareerFlow.Infrastructure.Data;
using CareerFlow.Infrastructure.Data.Interceptors;
using CareerFlow.Infrastructure.Outbox;
using CareerFlow.Infrastructure.Repositories;

namespace CareerFlow.Infrastructure;

/// <summary>
/// Registro de serviços da camada de Infraestrutura.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adiciona os serviços de infraestrutura ao container DI.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ============================================
        // Database (PostgreSQL)
        // ============================================
        var connectionString = configuration.GetSection("Database:ConnectionString").Value
            ?? throw new InvalidOperationException("Connection string não configurada");

        services.AddDbContext<CareerFlowDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
                npgsqlOptions.MigrationsAssembly(typeof(CareerFlowDbContext).Assembly.FullName);
            });

            // Adiciona interceptors via DI
            options.AddInterceptors(
                sp.GetRequiredService<AuditInterceptor>(),
                sp.GetRequiredService<DomainEventInterceptor>());
        });

        // ============================================
        // Interceptors
        // ============================================
        services.AddScoped<AuditInterceptor>();
        services.AddScoped<DomainEventInterceptor>();

        // ============================================
        // Unit of Work
        // ============================================
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ============================================
        // Repositories
        // ============================================
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<UserRepository>();
        services.AddScoped<PersonRepository>();
        services.AddScoped<SkillRepository>();
        services.AddScoped<ExperienceRepository>();

        // ============================================
        // Outbox Processor (Singleton - Timer interno)
        // ============================================
        services.AddSingleton<OutboxProcessor>();

        // ============================================
        // Health Checks
        // ============================================
        services.AddHealthChecks()
            .AddNpgSql(connectionString, name: "PostgreSQL", tags: new[] { "database" });

        return services;
    }
}
