using CareerFlow.Core.Entities;

namespace CareerFlow.Core.Specifications;

/// <summary>
/// Especificação para usuários premium ativos.
/// </summary>
public class PremiumUserSpecification : BaseSpecification<User>
{
    public PremiumUserSpecification()
        : base(user =>
            user.IsPremium &&
            user.PremiumUntil.HasValue &&
            user.PremiumUntil.Value > DateTime.UtcNow)
    {
    }
}
