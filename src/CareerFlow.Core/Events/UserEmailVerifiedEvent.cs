namespace CareerFlow.Core.Events;

/// <summary>
/// Evento disparado quando o email do usuário é verificado.
/// </summary>
public sealed class UserEmailVerifiedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }

    public UserEmailVerifiedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}
