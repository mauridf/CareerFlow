namespace CareerFlow.Core.Interfaces.Settings;

/// <summary>
/// Configurações do banco de dados
/// </summary>
public class DatabaseSettings
{
    public const string SectionName = "Database";

    /// <summary>Provedor de banco de dados</summary>
    public string Provider { get; set; } = "PostgreSQL";

    /// <summary>String de conexão</summary>
    public string ConnectionString { get; set; } = string.Empty;
}
