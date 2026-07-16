namespace CareerFlow.Core.Interfaces;

/// <summary>
/// Interface para Unit of Work.
/// Gerencia transações e persistência de mudanças.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Salva todas as mudanças em uma transação
    /// </summary>
    /// <returns>Número de registros afetados</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Inicia uma transação explícita
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirma a transação atual
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Desfaz a transação atual
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se há mudanças pendentes
    /// </summary>
    bool HasChanges();
}
