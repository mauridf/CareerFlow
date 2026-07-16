using MediatR;

namespace CareerFlow.Core.Events;

/// <summary>
/// Interface base para todos os eventos de domínio.
/// Eventos são imutáveis e representam algo que aconteceu no passado.
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>Identificador único do evento</summary>
    Guid EventId { get; }

    /// <summary>Momento em que o evento ocorreu</summary>
    DateTime OccurredAt { get; }

    /// <summary>Tipo do evento para roteamento</summary>
    string EventType { get; }
}
