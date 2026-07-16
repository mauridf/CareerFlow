namespace CareerFlow.Core.Exceptions;

/// <summary>
/// Exceção base para violações de regras de domínio.
/// Representa erros de negócio que o usuário pode corrigir.
/// </summary>
public class DomainException : Exception
{
    /// <summary>Código de erro opcional para identificação</summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Cria uma nova exceção de domínio
    /// </summary>
    /// <param name="message">Mensagem descritiva do erro</param>
    public DomainException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Cria uma nova exceção de domínio com código de erro
    /// </summary>
    /// <param name="message">Mensagem descritiva</param>
    /// <param name="errorCode">Código de erro para identificação</param>
    public DomainException(string message, string errorCode)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Cria uma nova exceção de domínio com exceção interna
    /// </summary>
    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
