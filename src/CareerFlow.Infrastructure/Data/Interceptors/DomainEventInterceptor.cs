using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Events;
using CareerFlow.Infrastructure.Outbox;

namespace CareerFlow.Infrastructure.Data.Interceptors;

/// <summary>
/// Interceptor que captura Domain Events e os converte em Outbox Messages.
/// Implementa o padrão "Transaction Log Tailing" para Event Sourcing.
/// </summary>
public class DomainEventInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context != null)
        {
            await DispatchDomainEventsAsync(eventData.Context, cancellationToken);
        }

        return result;
    }

    /// <summary>
    /// Despacha os eventos de domínio pendentes para o Outbox
    /// </summary>
    private async Task DispatchDomainEventsAsync(DbContext context, CancellationToken cancellationToken)
    {
        // Obtém todas as entidades que possuem domain events
        var aggregateRoots = context.ChangeTracker
            .Entries<AggregateRoot<Guid>>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        foreach (var aggregate in aggregateRoots)
        {
            var domainEvents = aggregate.DomainEvents.ToList();
            aggregate.ClearDomainEvents();

            foreach (var domainEvent in domainEvents)
            {
                // Converte o domain event em Outbox Message
                var outboxMessage = OutboxMessage.Create(
                    domainEvent,
                    domainEvent.GetType().Name);

                context.Add(outboxMessage);
            }
        }

        // Salva as mensagens do Outbox na mesma transação
        if (aggregateRoots.Count > 0)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
