namespace CareerFlow.Core.Events;

/// <summary>
/// Evento disparado quando um novo usuário se registra no sistema.
/// </summary>
public sealed class UserRegisteredEvent : DomainEvent
{
    /// <summary>ID do usuário registrado</summary>
    public Guid UserId { get; }

    /// <summary>Email do usuário</summary>
    public string Email { get; }

    /// <summary>Nome do usuário</summary>
    public string Name { get; }

    public UserRegisteredEvent(Guid userId, string email, string name)
    {
        UserId = userId;
        Email = email;
        Name = name;
    }
}
