namespace CareerFlow.Core.Interfaces.Settings;

/// <summary>
/// Configurações de Rate Limiting
/// </summary>
public class RateLimitingSettings
{
    public const string SectionName = "RateLimiting";

    public RateLimitConfig Global { get; set; } = new();
    public RateLimitConfig Login { get; set; } = new();
    public RateLimitConfig Register { get; set; } = new();
}

public class RateLimitConfig
{
    public int PermitLimit { get; set; } = 100;
    public int WindowSeconds { get; set; } = 60;

    public TimeSpan Window => TimeSpan.FromSeconds(WindowSeconds);
}
