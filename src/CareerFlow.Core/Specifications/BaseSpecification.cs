using System.Linq.Expressions;

namespace CareerFlow.Core.Specifications;

/// <summary>
/// Implementação base do padrão Specification.
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public abstract class BaseSpecification<T> : ISpecification<T>
{
    /// <summary>
    /// Expressão que define a regra de negócio
    /// </summary>
    public Expression<Func<T, bool>> Criteria { get; }

    /// <summary>
    /// Inclui navigation properties para eager loading
    /// </summary>
    public List<Expression<Func<T, object>>> Includes { get; } = new();

    /// <summary>
    /// Expressão de ordenação
    /// </summary>
    public Expression<Func<T, object>>? OrderBy { get; private set; }

    /// <summary>
    /// Ordenação descendente
    /// </summary>
    public bool OrderByDescending { get; private set; }

    /// <summary>
    /// Paginação: número de registros a pular
    /// </summary>
    public int? Skip { get; private set; }

    /// <summary>
    /// Paginação: número de registros a retornar
    /// </summary>
    public int? Take { get; private set; }

    /// <summary>
    /// Se a paginação está habilitada
    /// </summary>
    public bool IsPagingEnabled => Skip.HasValue && Take.HasValue;

    /// <summary>
    /// Construtor padrão (critério sempre verdadeiro)
    /// </summary>
    protected BaseSpecification()
    {
        Criteria = _ => true;
    }

    /// <summary>
    /// Construtor com critério específico
    /// </summary>
    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// Adiciona uma inclusão (eager loading)
    /// </summary>
    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    /// <summary>
    /// Define ordenação ascendente
    /// </summary>
    protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
        OrderByDescending = false;
    }

    /// <summary>
    /// Define ordenação descendente
    /// </summary>
    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
        OrderByDescending = true;
    }

    /// <summary>
    /// Aplica paginação
    /// </summary>
    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }

    /// <summary>
    /// Avalia se a entidade satisfaz a especificação
    /// </summary>
    public bool IsSatisfiedBy(T entity)
    {
        return Criteria.Compile()(entity);
    }

    /// <summary>
    /// Combina com AND
    /// </summary>
    public ISpecification<T> And(ISpecification<T> other)
    {
        return new AndSpecification<T>(this, other);
    }

    /// <summary>
    /// Combina com OR
    /// </summary>
    public ISpecification<T> Or(ISpecification<T> other)
    {
        return new OrSpecification<T>(this, other);
    }

    /// <summary>
    /// Nega a especificação
    /// </summary>
    public ISpecification<T> Not()
    {
        return new NotSpecification<T>(this);
    }
}
