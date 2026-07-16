using CareerFlow.Core.Entities;

namespace CareerFlow.Core.Specifications;

/// <summary>
/// Especificação para usuários com email verificado.
/// </summary>
public class VerifiedEmailSpecification : BaseSpecification<User>
{
    public VerifiedEmailSpecification()
        : base(user =>
            user.EmailVerifiedAt.HasValue)
    {
    }
}
