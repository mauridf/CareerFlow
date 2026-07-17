using System.Linq.Expressions;

namespace CareerFlow.Core.Specifications;

/// <summary>
/// Combinação AND de duas especificações
/// </summary>
public class AndSpecification<T> : BaseSpecification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
        : base(CombineExpressions(left.Criteria, right.Criteria))
    {
        _left = left;
        _right = right;
    }

    private static Expression<Func<T, bool>> CombineExpressions(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(left.Parameters[0], parameter);
        var leftBody = leftVisitor.Visit(left.Body)!;

        var rightVisitor = new ReplaceExpressionVisitor(right.Parameters[0], parameter);
        var rightBody = rightVisitor.Visit(right.Body)!;

        var combinedBody = Expression.AndAlso(leftBody, rightBody);

        return Expression.Lambda<Func<T, bool>>(combinedBody, parameter);
    }
}

/// <summary>
/// Combinação OR de duas especificações
/// </summary>
public class OrSpecification<T> : BaseSpecification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        : base(CombineExpressions(left.Criteria, right.Criteria))
    {
        _left = left;
        _right = right;
    }

    private static Expression<Func<T, bool>> CombineExpressions(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(left.Parameters[0], parameter);
        var leftBody = leftVisitor.Visit(left.Body)!;

        var rightVisitor = new ReplaceExpressionVisitor(right.Parameters[0], parameter);
        var rightBody = rightVisitor.Visit(right.Body)!;

        var combinedBody = Expression.OrElse(leftBody, rightBody);

        return Expression.Lambda<Func<T, bool>>(combinedBody, parameter);
    }
}

/// <summary>
/// Negação de uma especificação
/// </summary>
public class NotSpecification<T> : BaseSpecification<T>
{
    private readonly ISpecification<T> _specification;

    public NotSpecification(ISpecification<T> specification)
        : base(NegateExpression(specification.Criteria))
    {
        _specification = specification;
    }

    private static Expression<Func<T, bool>> NegateExpression(Expression<Func<T, bool>> expression)
    {
        var parameter = Expression.Parameter(typeof(T));
        var visitor = new ReplaceExpressionVisitor(expression.Parameters[0], parameter);
        var body = visitor.Visit(expression.Body)!;
        var negatedBody = Expression.Not(body);

        return Expression.Lambda<Func<T, bool>>(negatedBody, parameter);
    }
}

/// <summary>
/// Visitor para substituir parâmetros em expressões
/// </summary>
internal class ReplaceExpressionVisitor : ExpressionVisitor
{
    private readonly Expression _oldValue;
    private readonly Expression _newValue;

    public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
    {
        _oldValue = oldValue;
        _newValue = newValue;
    }

    public override Expression? Visit(Expression? node)
    {
        return node == _oldValue ? _newValue : base.Visit(node);
    }
}
