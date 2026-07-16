using System.Linq.Expressions;

namespace CareerFlow.Core.Specifications;

/// <summary>
/// Interface para o padrão Specification.
/// Encapsula regras de negócio em objetos reutilizáveis e combináveis.
/// </summary>
/// <typeparam name="T">Tipo da entidade a ser avaliada</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Expressão que define a regra de negócio
    /// </summary>
    Expression<Func<T, bool>> Criteria { get; }

    /// <summary>
    /// Avalia se a entidade satisfaz a especificação
    /// </summary>
    bool IsSatisfiedBy(T entity);

    /// <summary>
    /// Combina duas especificações com AND
    /// </summary>
    ISpecification<T> And(ISpecification<T> other);

    /// <summary>
    /// Combina duas especificações com OR
    /// </summary>
    ISpecification<T> Or(ISpecification<T> other);

    /// <summary>
    /// Nega a especificação
    /// </summary>
    ISpecification<T> Not();
}
