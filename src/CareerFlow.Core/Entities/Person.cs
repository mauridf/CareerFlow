using CareerFlow.Core.Events;

namespace CareerFlow.Core.Entities;

/// <summary>
/// Entidade que representa o perfil/pessoa associada a um usuário (Aggregate Root).
/// </summary>
public class Person : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public User? User { get; private set; }

    // Dados Pessoais
    public string? Phone { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public string? ProfessionalSummary { get; private set; }
    public string? PhotoUrl { get; private set; }

    // Profissão Atual
    public string? CurrentPosition { get; private set; }
    public string? CurrentCompany { get; private set; }

    // Visibilidade
    public bool IsPublic { get; private set; } = true;
    public string? ResumeSlug { get; private set; }

    // Navegação
    public ICollection<SocialNetwork> SocialNetworks { get; private set; } = new List<SocialNetwork>();
    public ICollection<Skill> Skills { get; private set; } = new List<Skill>();
    public ICollection<Experience> Experiences { get; private set; } = new List<Experience>();
    public ICollection<Education> Educations { get; private set; } = new List<Education>();
    public ICollection<Certificate> Certificates { get; private set; } = new List<Certificate>();
    public ICollection<Language> Languages { get; private set; } = new List<Language>();
    public ICollection<ResumeView> ResumeViews { get; private set; } = new List<ResumeView>();
    public ResumeAnalytics? ResumeAnalytics { get; private set; }
    public ICollection<ResumeSuggestion> ResumeSuggestions { get; private set; } = new List<ResumeSuggestion>();

    private Person() { }

    public static Person Create(Guid userId)
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            IsPublic = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        person.AddDomainEvent(new ProfileCreatedEvent(person.Id, userId));

        return person;
    }

    public void UpdatePersonalInfo(
        string? phone,
        string? city,
        string? state,
        DateTime? birthDate,
        string? professionalSummary)
    {
        if (!string.IsNullOrWhiteSpace(professionalSummary))
        {
            if (professionalSummary.Length < 100)
                throw new DomainException("Resumo profissional deve ter no mínimo 100 caracteres");

            if (professionalSummary.Length > 2000)
                throw new DomainException("Resumo profissional deve ter no máximo 2000 caracteres");
        }

        if (birthDate.HasValue && birthDate.Value > DateTime.Now.AddYears(-14))
            throw new DomainException("Usuário deve ter pelo menos 14 anos");

        // Valida e formata o telefone se fornecido
        if (!string.IsNullOrWhiteSpace(phone))
        {
            var phoneVO = new ValueObjects.PhoneNumber(phone);
            Phone = phoneVO.Value;
        }
        else
        {
            Phone = null;
        }

        City = city?.Trim();
        State = state?.Trim()?.ToUpper();
        BirthDate = birthDate;
        ProfessionalSummary = professionalSummary?.Trim();

        MarkAsUpdated();
        AddDomainEvent(new ProfileUpdatedEvent(Id));
    }

    public void UpdateCurrentProfession(string? position, string? company)
    {
        CurrentPosition = position?.Trim();
        CurrentCompany = company?.Trim();
        MarkAsUpdated();
    }

    public void UpdatePhoto(string photoUrl)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
            throw new DomainException("URL da foto é obrigatória");

        PhotoUrl = photoUrl;
        MarkAsUpdated();
    }

    public void RemovePhoto()
    {
        PhotoUrl = null;
        MarkAsUpdated();
    }

    public void SetPublic(bool isPublic)
    {
        IsPublic = isPublic;
        MarkAsUpdated();
    }

    public void Share(string? shareMethod = "link")
    {
        if (!IsPublic)
            IsPublic = true;

        MarkAsUpdated();
        AddDomainEvent(new ResumeSharedEvent(Id, UserId, shareMethod ?? "link"));
    }

    public void SetResumeSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new DomainException("Slug não pode ser vazio");

        if (slug.Length > 100)
            throw new DomainException("Slug deve ter no máximo 100 caracteres");

        ResumeSlug = slug.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace(".", "")
            .Replace("@", "-")
            .Trim('-');

        MarkAsUpdated();
    }

    public string GenerateSlug(string userName)
    {
        var baseSlug = userName
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace(".", "")
            .Replace("@", "-")
            .Trim('-');

        baseSlug = System.Text.RegularExpressions.Regex.Replace(baseSlug, @"[^a-z0-9\-]", "");

        var uniqueSuffix = Id.ToString("N")[..8];

        return $"{baseSlug}-{uniqueSuffix}";
    }

    public int CalculateCompletionPercentage()
    {
        int totalFields = 8;
        int completedFields = 0;

        if (!string.IsNullOrWhiteSpace(Phone)) completedFields++;
        if (!string.IsNullOrWhiteSpace(City)) completedFields++;
        if (!string.IsNullOrWhiteSpace(State)) completedFields++;
        if (BirthDate.HasValue) completedFields++;
        if (!string.IsNullOrWhiteSpace(ProfessionalSummary)) completedFields++;
        if (!string.IsNullOrWhiteSpace(PhotoUrl)) completedFields++;
        if (!string.IsNullOrWhiteSpace(CurrentPosition)) completedFields++;
        if (!string.IsNullOrWhiteSpace(CurrentCompany)) completedFields++;

        return (int)Math.Round((double)completedFields / totalFields * 100);
    }

    /// <summary>
    /// Verifica se o perfil atende aos requisitos mínimos para gerar currículo
    /// </summary>
    public bool CanGenerateResume()
    {
        return CalculateCompletionPercentage() >= 60
            && !string.IsNullOrWhiteSpace(ProfessionalSummary)
            && !string.IsNullOrWhiteSpace(City)
            && !string.IsNullOrWhiteSpace(State)
            && Experiences.Any()
            && Educations.Any()
            && Skills.Any();
    }
}
