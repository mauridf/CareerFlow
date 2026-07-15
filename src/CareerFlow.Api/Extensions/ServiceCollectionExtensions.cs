using CareerFlow.Core.Interfaces.Settings;

namespace CareerFlow.Api.Extensions;

/// <summary>
/// Extensões para configuração de serviços
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra todas as configurações fortemente tipadas
    /// </summary>
    public static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind das configurações
        services.Configure<ApplicationSettings>(
            configuration.GetSection(ApplicationSettings.SectionName));

        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

        services.Configure<DatabaseSettings>(
            configuration.GetSection(DatabaseSettings.SectionName));

        services.Configure<RedisSettings>(
            configuration.GetSection(RedisSettings.SectionName));

        services.Configure<StorageSettings>(
            configuration.GetSection(StorageSettings.SectionName));

        services.Configure<RateLimitingSettings>(
            configuration.GetSection(RateLimitingSettings.SectionName));

        services.Configure<PdfSettings>(
            configuration.GetSection(PdfSettings.SectionName));

        return services;
    }
}
