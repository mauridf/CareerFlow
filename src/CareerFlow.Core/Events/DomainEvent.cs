namespace CareerFlow.Core.Events;

/// <summary>
/// Classe base abstrata para todos os eventos de domínio.
/// Fornece identificação e timestamp automáticos.
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    /// <inheritdoc/>
    public Guid EventId { get; } = Guid.NewGuid();

    /// <inheritdoc/>
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    /// <inheritdoc/>
    public string EventType => GetType().Name;

    /// <summary>
    /// Retorna uma representação do evento para logging
    /// </summary>
    public override string ToString()
    {
        return $"[{EventType}] EventId={EventId} OccurredAt={OccurredAt:O}";
    }
}
