using Serilog;

namespace CareerFlow.Api.Extensions;

/// <summary>
/// Extensões para configuração completa do Serilog
/// </summary>
public static class SerilogExtensions
{
    /// <summary>
    /// Registra os serviços do Serilog e enrichers
    /// </summary>
    public static IServiceCollection AddSerilogServices(this IServiceCollection services)
    {
        // Registra o serviço de log tipado para uso nas camadas
        services.AddScoped(typeof(Application.Common.Interfaces.ILoggerService<>),
                          typeof(Application.Common.Interfaces.LoggerService<>));

        return services;
    }
}
