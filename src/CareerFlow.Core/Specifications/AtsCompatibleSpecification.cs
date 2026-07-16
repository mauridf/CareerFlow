using CareerFlow.Core.Entities;

namespace CareerFlow.Core.Specifications;

/// <summary>
/// Especificação que verifica se um currículo atende aos requisitos ATS.
/// </summary>
public class AtsCompatibleSpecification : BaseSpecification<Person>
{
    private const int MinimumAtsScore = 60;
    private const int MinimumExperienceCount = 1;
    private const int MinimumEducationCount = 1;
    private const int MinimumSkillCount = 3;
    private const int MaximumSummaryLength = 2000;
    private const int MinimumSummaryLength = 100;

    /// <summary>
    /// Cria a especificação ATS
    /// </summary>
    public AtsCompatibleSpecification()
        : base(person => true)
    {
        // Inclui dados relacionados para análise completa
        AddInclude(p => p.Experiences);
        AddInclude(p => p.Educations);
        AddInclude(p => p.Skills);
        AddInclude(p => p.Certificates);
        AddInclude(p => p.Languages);
    }

    /// <summary>
    /// Verifica se o perfil é compatível com ATS
    /// </summary>
    public new bool IsSatisfiedBy(Person person)
    {
        if (person == null) return false;

        return HasMinimumExperiences(person)
            && HasMinimumEducation(person)
            && HasMinimumSkills(person)
            && HasProfessionalSummary(person)
            && HasContactInfo(person);
    }

    /// <summary>
    /// Calcula o score ATS do perfil (0-100)
    /// </summary>
    public int CalculateAtsScore(Person person)
    {
        if (person == null) return 0;

        int score = 0;
        int maxScore = 100;

        // 1. Informações de contato (20 pontos)
        if (!string.IsNullOrWhiteSpace(person.City) && !string.IsNullOrWhiteSpace(person.State))
            score += 10;
        if (!string.IsNullOrWhiteSpace(person.Phone))
            score += 5;
        if (person.User?.Email != null)
            score += 5;

        // 2. Resumo profissional (15 pontos)
        if (!string.IsNullOrWhiteSpace(person.ProfessionalSummary))
        {
            var summary = person.ProfessionalSummary;
            if (summary.Length >= MinimumSummaryLength && summary.Length <= MaximumSummaryLength)
                score += 15;
            else if (summary.Length >= MinimumSummaryLength)
                score += 10;
            else
                score += 5;
        }

        // 3. Experiências (30 pontos)
        var experienceCount = person.Experiences?.Count ?? 0;
        if (experienceCount >= 3) score += 30;
        else if (experienceCount >= 2) score += 20;
        else if (experienceCount >= 1) score += 10;

        // 4. Formação (15 pontos)
        var educationCount = person.Educations?.Count ?? 0;
        if (educationCount >= 2) score += 15;
        else if (educationCount >= 1) score += 10;

        // 5. Habilidades (15 pontos)
        var skillCount = person.Skills?.Count ?? 0;
        if (skillCount >= 5) score += 15;
        else if (skillCount >= 3) score += 10;
        else if (skillCount >= 1) score += 5;

        // 6. Certificações (bônus de 5 pontos)
        if ((person.Certificates?.Count ?? 0) >= 1)
            score += 5;

        return Math.Min(score, maxScore);
    }

    /// <summary>
    /// Retorna uma lista de problemas detectados
    /// </summary>
    public List<string> GetAtsIssues(Person person)
    {
        var issues = new List<string>();

        if (person == null)
        {
            issues.Add("Perfil não encontrado");
            return issues;
        }

        if (!HasContactInfo(person))
            issues.Add("Informações de contato incompletas (cidade, estado, telefone)");

        if (!HasProfessionalSummary(person))
            issues.Add("Resumo profissional ausente ou muito curto (mínimo 100 caracteres)");

        if (!HasMinimumExperiences(person))
            issues.Add("É necessário pelo menos 1 experiência profissional");

        if (!HasMinimumEducation(person))
            issues.Add("É necessário pelo menos 1 formação acadêmica");

        if (!HasMinimumSkills(person))
            issues.Add("É necessário pelo menos 3 habilidades");

        var summary = person.ProfessionalSummary;
        if (!string.IsNullOrWhiteSpace(summary) && summary.Length > MaximumSummaryLength)
            issues.Add($"Resumo profissional muito longo ({summary.Length} caracteres, máximo {MaximumSummaryLength})");

        if ((person.Experiences?.Count ?? 0) == 1)
            issues.Add("Considere adicionar mais experiências profissionais");

        if ((person.Skills?.Count ?? 0) < 5)
            issues.Add("Considere adicionar mais habilidades (mínimo recomendado: 5)");

        return issues;
    }

    private bool HasMinimumExperiences(Person person)
        => (person.Experiences?.Count ?? 0) >= MinimumExperienceCount;

    private bool HasMinimumEducation(Person person)
        => (person.Educations?.Count ?? 0) >= MinimumEducationCount;

    private bool HasMinimumSkills(Person person)
        => (person.Skills?.Count ?? 0) >= MinimumSkillCount;

    private bool HasProfessionalSummary(Person person)
    {
        var summary = person.ProfessionalSummary;
        return !string.IsNullOrWhiteSpace(summary)
            && summary.Length >= MinimumSummaryLength;
    }

    private bool HasContactInfo(Person person)
    {
        return !string.IsNullOrWhiteSpace(person.City)
            && !string.IsNullOrWhiteSpace(person.State)
            && !string.IsNullOrWhiteSpace(person.Phone);
    }
}
