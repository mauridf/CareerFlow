namespace CareerFlow.Core.Interfaces.Settings;

/// <summary>
/// Configurações da aplicação
/// </summary>
public class ApplicationSettings
{
    public const string SectionName = "Application";

    /// <summary>Nome da aplicação</summary>
    public string Name { get; set; } = "CareerFlow";

    /// <summary>Ambiente atual</summary>
    public string Environment { get; set; } = "Development";

    /// <summary>URL base da aplicação</summary>
    public string Url { get; set; } = "http://localhost:5000";

    /// <summary>Origens permitidas para CORS</summary>
    public List<string> CorsOrigins { get; set; } = new();

    /// <summary>Exibir erros detalhados (apenas dev)</summary>
    public bool ShowDetailedErrors { get; set; } = false;
}
