using CareerFlow.Core.Entities;

namespace CareerFlow.Core.Specifications;

/// <summary>
/// Fábrica de especificações comuns para facilitar o uso.
/// </summary>
public static class SpecificationFactory
{
    /// <summary>
    /// Usuário ativo E email verificado
    /// </summary>
    public static ISpecification<User> ActiveAndVerified()
    {
        return new ActiveUserSpecification()
            .And(new VerifiedEmailSpecification());
    }

    /// <summary>
    /// Usuário ativo, verificado e premium
    /// </summary>
    public static ISpecification<User> ActiveVerifiedAndPremium()
    {
        return new ActiveUserSpecification()
            .And(new VerifiedEmailSpecification())
            .And(new PremiumUserSpecification());
    }

    /// <summary>
    /// Perfil completo (80%) E compatível com ATS
    /// </summary>
    public static ISpecification<Person> CompleteAndAtsCompatible()
    {
        return new CompleteProfileSpecification(80)
            .And(new AtsCompatibleSpecification());
    }

    /// <summary>
    /// Perfil publicado e visível
    /// </summary>
    public static ISpecification<Person> PublishedAndVisible()
    {
        return new PublishedResumeSpecification();
    }
}
