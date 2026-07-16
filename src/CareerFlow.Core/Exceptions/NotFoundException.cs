namespace CareerFlow.Core.Exceptions;

/// <summary>
/// Exceção lançada quando um recurso não é encontrado.
/// </summary>
public class NotFoundException : DomainException
{
    /// <summary>Tipo da entidade não encontrada</summary>
    public string EntityName { get; }

    /// <summary>ID do recurso não encontrado</summary>
    public object? EntityId { get; }

    /// <summary>
    /// Cria uma exceção de recurso não encontrado
    /// </summary>
    /// <param name="entityName">Nome da entidade</param>
    /// <param name="entityId">ID procurado</param>
    public NotFoundException(string entityName, object? entityId = null)
        : base(entityId != null
            ? $"{entityName} com ID '{entityId}' não foi encontrado(a)"
            : $"{entityName} não foi encontrado(a)")
    {
        EntityName = entityName;
        EntityId = entityId;
    }

    /// <summary>
    /// Cria uma exceção de recurso não encontrado com mensagem personalizada
    /// </summary>
    public NotFoundException(string message)
        : base(message)
    {
        EntityName = string.Empty;
    }
}
