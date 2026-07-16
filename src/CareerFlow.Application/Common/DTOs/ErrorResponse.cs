namespace CareerFlow.Application.Common.DTOs;

/// <summary>
/// Resposta de erro da API.
/// </summary>
public class ErrorResponse
{
    public bool Success { get; set; } = false;
    public ErrorDetail Error { get; set; } = new();
    public MetaResponse Meta { get; set; } = new();
}

public class ErrorDetail
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<FieldError>? Details { get; set; }
}

public class FieldError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
