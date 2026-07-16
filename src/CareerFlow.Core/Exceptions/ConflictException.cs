namespace CareerFlow.Core.Exceptions;

/// <summary>
/// Exceção para conflitos (ex: recurso duplicado).
/// </summary>
public class ConflictException : DomainException
{
    /// <summary>
    /// Cria uma exceção de conflito
    /// </summary>
    public ConflictException(string message)
        : base(message, "CONFLICT")
    {
    }

    /// <summary>
    /// Cria uma exceção de conflito para recurso duplicado
    /// </summary>
    /// <param name="resourceName">Nome do recurso</param>
    /// <param name="field">Campo duplicado</param>
    /// <param name="value">Valor duplicado</param>
    public ConflictException(string resourceName, string field, string value)
        : base($"Já existe um(a) {resourceName} com {field} '{value}'", "CONFLICT")
    {
    }
}
