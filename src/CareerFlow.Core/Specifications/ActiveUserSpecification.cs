using CareerFlow.Core.Entities;

namespace CareerFlow.Core.Specifications;

/// <summary>
/// Especificação para usuários ativos e não excluídos.
/// </summary>
public class ActiveUserSpecification : BaseSpecification<User>
{
    public ActiveUserSpecification()
        : base(user =>
            user.IsActive &&
            !user.DeletedAt.HasValue)
    {
    }
}
