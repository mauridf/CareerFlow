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

    /// <summary>Email do usuário (Value Object)</summary>
    public Email Email { get; private set; } = null!;

    /// <summary>Hash da senha (BCrypt)</summary>
    public string PasswordHash { get; private set; } = string.Empty;

    // ============================================
    // Autenticação
    // ============================================

    /// <summary>Quando o email foi verificado</summary>
    public DateTime? EmailVerifiedAt { get; private set; }

    /// <summary>Último login bem-sucedido</summary>
    public DateTime? LastLoginAt { get; private set; }

    /// <summary>Última alteração de senha</summary>
    public DateTime? LastPasswordChangeAt { get; private set; }

    /// <summary>Tentativas de login falhas consecutivas</summary>
    public int FailedLoginAttempts { get; private set; }

    /// <summary>Conta bloqueada até esta data</summary>
    public DateTime? LockedUntil { get; private set; }

    /// <summary>Autenticação de dois fatores habilitada</summary>
    public bool TwoFactorEnabled { get; private set; }

    /// <summary>Segredo TOTP para 2FA</summary>
    public string? TwoFactorSecret { get; private set; }

    // ============================================
    // Role e Status
    // ============================================

    /// <summary>Papel do usuário no sistema</summary>
    public UserRole Role { get; private set; } = UserRole.User;

    /// <summary>Conta ativa</summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>Usuário premium</summary>
    public bool IsPremium { get; private set; }

    /// <summary>Premium válido até</summary>
    public DateTime? PremiumUntil { get; private set; }

    // ============================================
    // Preferências (JSON)
    // ============================================

    /// <summary>Preferências de notificação</summary>
    public string? NotificationPreferences { get; private set; }

    /// <summary>Preferências de tema</summary>
    public string? ThemePreferences { get; private set; }

    // ============================================
    // Soft Delete
    // ============================================

    /// <summary>Data de exclusão lógica (soft delete)</summary>
    public DateTime? DeletedAt { get; private set; }

    // ============================================
    // Construtor privado (EF Core)
    // ============================================
    private User() { }

    // ============================================
    // Factory Method
    // ============================================

    /// <summary>
    /// Cria um novo usuário
    /// </summary>
    public static User Create(string name, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome é obrigatório", nameof(name));

        if (name.Length > 200)
            throw new ArgumentException("Nome deve ter no máximo 200 caracteres", nameof(name));

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Email = new Email(email),
            PasswordHash = passwordHash,
            Role = UserRole.User,
            IsActive = true,
            IsPremium = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        user.AddDomainEvent(new UserRegisteredEvent(user.Id, user.Email.Value, user.Name));

        return user;
    }

    // ============================================
    // Métodos de Comportamento
    // ============================================

    /// <summary>
    /// Marca o email como verificado
    /// </summary>
    public void VerifyEmail()
    {
        EmailVerifiedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    /// <summary>
    /// Registra um login bem-sucedido
    /// </summary>
    public void RecordSuccessfulLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        LockedUntil = null;
        MarkAsUpdated();
    }

    /// <summary>
    /// Registra uma tentativa de login falha
    /// </summary>
    public void RecordFailedLogin()
    {
        FailedLoginAttempts++;

        // Bloqueia após 5 tentativas falhas
        if (FailedLoginAttempts >= 5)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(15);
        }

        MarkAsUpdated();
    }

    /// <summary>
    /// Altera a senha do usuário
    /// </summary>
    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        LastPasswordChangeAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    /// <summary>
    /// Atualiza o nome do usuário
    /// </summary>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome é obrigatório", nameof(name));

        if (name.Length > 200)
            throw new ArgumentException("Nome deve ter no máximo 200 caracteres", nameof(name));

        Name = name.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Atualiza o email do usuário
    /// </summary>
    public void UpdateEmail(string email)
    {
        Email = new Email(email);
        EmailVerifiedAt = null; // Precisa verificar novamente
        MarkAsUpdated();
    }

    /// <summary>
    /// Ativa a conta do usuário
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        LockedUntil = null;
        FailedLoginAttempts = 0;
        MarkAsUpdated();
    }

    /// <summary>
    /// Desativa a conta do usuário
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    /// <summary>
    /// Marca o usuário como excluído (soft delete)
    /// </summary>
    public void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow;
        IsActive = false;
        MarkAsUpdated();
    }

    /// <summary>
    /// Habilita 2FA
    /// </summary>
    public void EnableTwoFactor(string secret)
    {
        TwoFactorEnabled = true;
        TwoFactorSecret = secret;
        MarkAsUpdated();
    }

    /// <summary>
    /// Desabilita 2FA
    /// </summary>
    public void DisableTwoFactor()
    {
        TwoFactorEnabled = false;
        TwoFactorSecret = null;
        MarkAsUpdated();
    }

    /// <summary>
    /// Define o papel do usuário
    /// </summary>
    public void SetRole(UserRole role)
    {
        Role = role;
        MarkAsUpdated();
    }

    /// <summary>
    /// Ativa o plano premium
    /// </summary>
    public void ActivatePremium(DateTime until)
    {
        IsPremium = true;
        PremiumUntil = until;
        if (Role == UserRole.User)
            Role = UserRole.PremiumUser;
        MarkAsUpdated();
    }

    /// <summary>
    /// Desativa o plano premium
    /// </summary>
    public void DeactivatePremium()
    {
        IsPremium = false;
        PremiumUntil = null;
        if (Role == UserRole.PremiumUser)
            Role = UserRole.User;
        MarkAsUpdated();
    }

    /// <summary>
    /// Verifica se a conta está bloqueada
    /// </summary>
    public bool IsLocked()
    {
        return LockedUntil.HasValue && LockedUntil.Value > DateTime.UtcNow;
    }

    /// <summary>
    /// Verifica se o email está verificado
    /// </summary>
    public bool IsEmailVerified()
    {
        return EmailVerifiedAt.HasValue;
    }
}
