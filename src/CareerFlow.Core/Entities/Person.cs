using CareerFlow.Core.Events;

namespace CareerFlow.Core.Entities;

/// <summary>
/// Entidade que representa o perfil/pessoa associada a um usuário.
/// Contém dados pessoais, profissionais e de contato.
/// </summary>
public class Person : AggregateRoot<Guid>
{
    // ============================================
    // Relacionamentos
    // ============================================

    /// <summary>ID do usuário associado</summary>
    public Guid UserId { get; private set; }

    /// <summary>Usuário associado (navigation property)</summary>
    public User? User { get; private set; }

    // ============================================
    // Dados Pessoais
    // ============================================

    /// <summary>Telefone de contato</summary>
    public string? Phone { get; private set; }

    /// <summary>Cidade</summary>
    public string? City { get; private set; }

    /// <summary>Estado (UF)</summary>
    public string? State { get; private set; }

    /// <summary>Data de nascimento</summary>
    public DateTime? BirthDate { get; private set; }

    /// <summary>Resumo profissional</summary>
    public string? ProfessionalSummary { get; private set; }

    /// <summary>URL da foto de perfil</summary>
    public string? PhotoUrl { get; private set; }

    // ============================================
    // Profissão Atual
    // ============================================

    /// <summary>Cargo atual</summary>
    public string? CurrentPosition { get; private set; }

    /// <summary>Empresa atual</summary>
    public string? CurrentCompany { get; private set; }

    // ============================================
    // Preferências de Visibilidade
    // ============================================

    /// <summary>Currículo público</summary>
    public bool IsPublic { get; private set; } = true;

    /// <summary>Slug único para URL pública</summary>
    public string? ResumeSlug { get; private set; }

    // ============================================
    // Navegação
    // ============================================

    /// <summary>Redes sociais</summary>
    public ICollection<SocialNetwork> SocialNetworks { get; private set; } = new List<SocialNetwork>();

    /// <summary>Habilidades</summary>
    public ICollection<Skill> Skills { get; private set; } = new List<Skill>();

    /// <summary>Experiências profissionais</summary>
    public ICollection<Experience> Experiences { get; private set; } = new List<Experience>();

    /// <summary>Formação acadêmica</summary>
    public ICollection<Education> Educations { get; private set; } = new List<Education>();

    /// <summary>Certificados</summary>
    public ICollection<Certificate> Certificates { get; private set; } = new List<Certificate>();

    /// <summary>Idiomas</summary>
    public ICollection<Language> Languages { get; private set; } = new List<Language>();

    // ============================================
    // Construtor privado (EF Core)
    // ============================================
    private Person() { }

    // ============================================
    // Factory Method
    // ============================================

    /// <summary>
    /// Cria um novo perfil de pessoa
    /// </summary>
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

    // ============================================
    // Métodos de Comportamento
    // ============================================

    /// <summary>
    /// Atualiza dados pessoais
    /// </summary>
    public void UpdatePersonalInfo(
        string? phone,
        string? city,
        string? state,
        DateTime? birthDate,
        string? professionalSummary)
    {
        // Validações
        if (!string.IsNullOrWhiteSpace(professionalSummary) && professionalSummary.Length > 2000)
            throw new ArgumentException("Resumo profissional deve ter no máximo 2000 caracteres");

        if (birthDate.HasValue && birthDate.Value > DateTime.Now.AddYears(-14))
            throw new ArgumentException("Usuário deve ter pelo menos 14 anos");

        Phone = phone?.Trim();
        City = city?.Trim();
        State = state?.Trim()?.ToUpper();
        BirthDate = birthDate;
        ProfessionalSummary = professionalSummary?.Trim();

        MarkAsUpdated();
        AddDomainEvent(new ProfileUpdatedEvent(Id));
    }

    /// <summary>
    /// Atualiza profissão atual
    /// </summary>
    public void UpdateCurrentProfession(string? position, string? company)
    {
        CurrentPosition = position?.Trim();
        CurrentCompany = company?.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Atualiza foto de perfil
    /// </summary>
    public void UpdatePhoto(string photoUrl)
    {
        PhotoUrl = photoUrl;
        MarkAsUpdated();
    }

    /// <summary>
    /// Remove foto de perfil
    /// </summary>
    public void RemovePhoto()
    {
        PhotoUrl = null;
        MarkAsUpdated();
    }

    /// <summary>
    /// Define visibilidade pública
    /// </summary>
    public void SetPublic(bool isPublic)
    {
        IsPublic = isPublic;
        MarkAsUpdated();
    }

    /// <summary>
    /// Define o slug do currículo
    /// </summary>
    public void SetResumeSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Slug não pode ser vazio", nameof(slug));

        if (slug.Length > 100)
            throw new ArgumentException("Slug deve ter no máximo 100 caracteres", nameof(slug));

        ResumeSlug = slug.ToLowerInvariant().Replace(" ", "-");
        MarkAsUpdated();
    }

    /// <summary>
    /// Gera um slug baseado no nome do usuário
    /// </summary>
    public string GenerateSlug(string userName)
    {
        var baseSlug = userName
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace(".", "")
            .Replace("@", "-")
            .Trim('-');

        // Remove caracteres especiais
        baseSlug = System.Text.RegularExpressions.Regex.Replace(baseSlug, @"[^a-z0-9\-]", "");

        // Adiciona parte do GUID para unicidade
        var uniqueSuffix = Id.ToString("N")[..8];

        return $"{baseSlug}-{uniqueSuffix}";
    }

    /// <summary>
    /// Calcula o percentual de completude do perfil
    /// </summary>
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
}
