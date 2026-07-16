namespace CareerFlow.Core.Exceptions;

/// <summary>
/// Exceção para erros de validação de dados de entrada.
/// Contém uma lista de erros detalhados por campo.
/// </summary>
public class ValidationException : DomainException
{
    /// <summary>Lista de erros de validação por campo</summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Cria uma exceção de validação
    /// </summary>
    /// <param name="message">Mensagem geral do erro</param>
    public ValidationException(string message)
        : base(message, "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Cria uma exceção de validação com erros detalhados
    /// </summary>
    /// <param name="errors">Dicionário de erros (campo → mensagens)</param>
    public ValidationException(IDictionary<string, string[]> errors)
        : base("Um ou mais erros de validação ocorreram", "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>(errors);
    }

    /// <summary>
    /// Cria uma exceção de validação com um único erro
    /// </summary>
    /// <param name="field">Nome do campo</param>
    /// <param name="message">Mensagem de erro</param>
    public ValidationException(string field, string message)
        : base(message, "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { message } }
        };
    }
}
