namespace CareerFlow.Core.Interfaces.Settings;

/// <summary>
/// Configurações do Redis
/// </summary>
public class RedisSettings
{
    public const string SectionName = "Redis";

    /// <summary>Se o Redis está habilitado</summary>
    public bool Enabled { get; set; } = false;

    /// <summary>String de conexão</summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>Prefixo para chaves</summary>
    public string InstanceName { get; set; } = "CareerFlow:";

    /// <summary>Duração padrão do cache em minutos</summary>
    public int DefaultCacheDurationMinutes { get; set; } = 10;
}
