using CareerFlow.Core.Interfaces.Settings;
using CareerFlow.Api.Extensions;

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

        services.Configure<EmailSettings>(
            configuration.GetSection(EmailSettings.SectionName));

        return services;
    }

    /// <summary>
    /// Registra todos os serviços da aplicação
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Serilog
        services.AddSerilogServices();

        // Outros serviços serão adicionados nos próximos passos

        return services;
    }
}
