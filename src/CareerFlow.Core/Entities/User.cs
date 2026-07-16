using CareerFlow.Core.Enums;
using CareerFlow.Core.Events;
using CareerFlow.Core.ValueObjects;

namespace CareerFlow.Core.Entities;

/// <summary>
/// Entidade que representa um usuário do sistema.
/// Gerencia autenticação, autorização e status da conta.
/// </summary>
public class User : AggregateRoot<Guid>
{
    // ============================================
    // Propriedades
    // ============================================

    /// <summary>Nome completo do usuário</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Email do usuário</summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>Hash da senha (BCrypt)</summary>
    public string PasswordHash { get; private set; } = string.Empty;

    // ============================================
    // Autenticação
    // ============================================

    public DateTime? EmailVerifiedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public DateTime? LastPasswordChangeAt { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockedUntil { get; private set; }
    public bool TwoFactorEnabled { get; private set; }
    public string? TwoFactorSecret { get; private set; }

    // ============================================
    // Role e Status (usando Enums)
    // ============================================

    public UserRole Role { get; private set; } = UserRole.User;
    public bool IsActive { get; private set; } = true;
    public bool IsPremium { get; private set; }
    public DateTime? PremiumUntil { get; private set; }

    // ============================================
    // Preferências (JSON)
    // ============================================

    public string? NotificationPreferences { get; private set; }
    public string? ThemePreferences { get; private set; }

    // ============================================
    // Soft Delete
    // ============================================

    public DateTime? DeletedAt { get; private set; }

    // ============================================
    // Navegação
    // ============================================

    public Person? Person { get; private set; }
    public ICollection<ActivityLog> ActivityLogs { get; private set; } = new List<ActivityLog>();

    // ============================================
    // Construtor privado (EF Core)
    // ============================================
    private User() { }

    // ============================================
    // Factory Method
    // ============================================

    /// <summary>
    /// Cria um novo usuário com validações de domínio
    /// </summary>
    public static User Create(string name, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome é obrigatório");

        if (name.Length > 200)
            throw new DomainException("Nome deve ter no máximo 200 caracteres");

        // Valida o email usando o Value Object
        var emailVO = new Email(email);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Email = emailVO.Value,
            PasswordHash = passwordHash,
            Role = UserRole.User,
            IsActive = false, // Aguarda verificação de email
            IsPremium = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        user.AddDomainEvent(new UserRegisteredEvent(user.Id, user.Email, user.Name));

        return user;
    }

    // ============================================
    // Métodos de Comportamento
    // ============================================

    public void VerifyEmail()
    {
        EmailVerifiedAt = DateTime.UtcNow;
        IsActive = true;
        MarkAsUpdated();
        AddDomainEvent(new UserEmailVerifiedEvent(Id, Email));
    }

    public void RecordSuccessfulLogin(string? ipAddress = null)
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        LockedUntil = null;
        MarkAsUpdated();
        AddDomainEvent(new UserLoggedInEvent(Id, Email, ipAddress));
    }

    public void RecordFailedLogin()
    {
        FailedLoginAttempts++;

        if (FailedLoginAttempts >= 5)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(15);
        }

        MarkAsUpdated();
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        LastPasswordChangeAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        LockedUntil = null;
        MarkAsUpdated();
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome é obrigatório");

        if (name.Length > 200)
            throw new DomainException("Nome deve ter no máximo 200 caracteres");

        Name = name.Trim();
        MarkAsUpdated();
    }

    public void UpdateEmail(string newEmail)
    {
        var emailVO = new Email(newEmail);
        Email = emailVO.Value;
        EmailVerifiedAt = null;
        MarkAsUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        LockedUntil = null;
        FailedLoginAttempts = 0;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    public void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow;
        IsActive = false;
        MarkAsUpdated();
    }

    public void EnableTwoFactor(string secret)
    {
        TwoFactorEnabled = true;
        TwoFactorSecret = secret;
        MarkAsUpdated();
    }

    public void DisableTwoFactor()
    {
        TwoFactorEnabled = false;
        TwoFactorSecret = null;
        MarkAsUpdated();
    }

    public void SetRole(UserRole role)
    {
        Role = role;
        MarkAsUpdated();
    }

    public void ActivatePremium(DateTime until)
    {
        IsPremium = true;
        PremiumUntil = until;

        if (Role == UserRole.User)
            Role = UserRole.PremiumUser;

        MarkAsUpdated();
    }

    public void DeactivatePremium()
    {
        IsPremium = false;
        PremiumUntil = null;

        if (Role == UserRole.PremiumUser)
            Role = UserRole.User;

        MarkAsUpdated();
    }

    public bool IsLocked()
    {
        return LockedUntil.HasValue && LockedUntil.Value > DateTime.UtcNow;
    }

    public bool IsEmailVerified()
    {
        return EmailVerifiedAt.HasValue;
    }

    public bool CanLogin()
    {
        return IsActive && !IsLocked() && !DeletedAt.HasValue;
    }
}
