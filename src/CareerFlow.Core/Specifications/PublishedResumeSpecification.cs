using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;

namespace CareerFlow.Core.Specifications;

/// <summary>
/// Especificação para currículos publicados e visíveis publicamente.
/// </summary>
public class PublishedResumeSpecification : BaseSpecification<Person>
{
    /// <summary>
    /// Cria a especificação para currículos publicados
    /// </summary>
    public PublishedResumeSpecification()
        : base(person =>
            person.IsPublic &&
            person.ResumeSlug != null)
    {
        // Inclui dados relacionados
        AddInclude(p => p.User!);
        AddInclude(p => p.Experiences);
        AddInclude(p => p.Educations);
        AddInclude(p => p.Skills);
        AddInclude(p => p.Certificates);
        AddInclude(p => p.Languages);
        AddInclude(p => p.SocialNetworks);
        AddInclude(p => p.ResumeAnalytics!);
    }

    /// <summary>
    /// Verifica se o currículo pode ser visualizado publicamente
    /// </summary>
    public new bool IsSatisfiedBy(Person person)
    {
        return person != null
            && person.IsPublic
            && !string.IsNullOrWhiteSpace(person.ResumeSlug)
            && person.User != null
            && person.User.IsActive
            && !person.User.DeletedAt.HasValue;
    }

    /// <summary>
    /// Verifica se o currículo está publicado e o slug corresponde
    /// </summary>
    public bool IsSatisfiedBy(Person person, string slug)
    {
        return IsSatisfiedBy(person)
            && string.Equals(person.ResumeSlug, slug, StringComparison.OrdinalIgnoreCase);
    }
}
