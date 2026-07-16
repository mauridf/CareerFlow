namespace CareerFlow.Core.Exceptions;

/// <summary>
/// Exceção para acesso não autorizado.
/// </summary>
public class UnauthorizedException : DomainException
{
    /// <summary>
    /// Cria uma exceção de acesso não autorizado
    /// </summary>
    public UnauthorizedException(string message = "Acesso não autorizado")
        : base(message, "UNAUTHORIZED")
    {
    }
}
