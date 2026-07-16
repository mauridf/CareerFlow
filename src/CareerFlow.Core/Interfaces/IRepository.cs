using System.Linq.Expressions;

namespace CareerFlow.Core.Interfaces;

/// <summary>
/// Interface genérica para repositórios.
/// Define operações básicas de acesso a dados.
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Obtém uma entidade pelo ID
    /// </summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém todas as entidades
    /// </summary>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca entidades por critério
    /// </summary>
    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca entidades com ordenação
    /// </summary>
    Task<IReadOnlyList<T>> FindAsync<TOrderKey>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TOrderKey>> orderBy,
        bool descending = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca entidades paginadas
    /// </summary>
    Task<(IReadOnlyList<T> Items, int TotalCount)> FindPaginatedAsync(
        Expression<Func<T, bool>>? predicate,
        int page,
        int pageSize,
        Expression<Func<T, object>>? orderBy = null,
        bool descending = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se existe entidade com o critério
    /// </summary>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Conta entidades por critério
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona uma nova entidade
    /// </summary>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona múltiplas entidades
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza uma entidade
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Remove uma entidade
    /// </summary>
    void Delete(T entity);

    /// <summary>
    /// Remove múltiplas entidades
    /// </summary>
    void DeleteRange(IEnumerable<T> entities);
}
