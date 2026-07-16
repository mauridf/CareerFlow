namespace CareerFlow.Application.Common.DTOs;

/// <summary>
/// Resposta padrão de sucesso da API.
/// </summary>
public class SuccessResponse<T>
{
    public bool Success { get; set; } = true;
    public T? Data { get; set; }
    public MetaResponse Meta { get; set; } = new();
}

public class MetaResponse
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? RequestId { get; set; }
    public int? Page { get; set; }
    public int? PerPage { get; set; }
    public int? Total { get; set; }
    public int? TotalPages { get; set; }
}
