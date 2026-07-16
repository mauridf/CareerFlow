namespace CareerFlow.Core.Events;

/// <summary>
/// Evento disparado quando um usuário faz login com sucesso.
/// </summary>
public sealed class UserLoggedInEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public string? IpAddress { get; }

    public UserLoggedInEvent(Guid userId, string email, string? ipAddress = null)
    {
        UserId = userId;
        Email = email;
        IpAddress = ipAddress;
    }
}
