using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using CareerFlow.Core.Entities;

namespace CareerFlow.Infrastructure.Data.Interceptors;

/// <summary>
/// Interceptor do EF Core que registra automaticamente
/// logs de auditoria para entidades modificadas.
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        TrackChanges(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        TrackChanges(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void TrackChanges(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added ||
                        e.State == EntityState.Modified ||
                        e.State == EntityState.Deleted)
            .Where(e => e.Entity is not ActivityLog);

        foreach (var entry in entries)
        {
            var entityType = entry.Entity.GetType().Name;
            var entityId = GetEntityId(entry);

            switch (entry.State)
            {
                case EntityState.Added:
                    context.Add(CreateActivityLog("created", entityType, entityId,
                        null, SerializeProperties(entry.CurrentValues)));
                    break;

                case EntityState.Modified:
                    context.Add(CreateActivityLog("updated", entityType, entityId,
                        SerializeProperties(entry.OriginalValues),
                        SerializeProperties(entry.CurrentValues)));
                    break;

                case EntityState.Deleted:
                    context.Add(CreateActivityLog("deleted", entityType, entityId,
                        SerializeProperties(entry.OriginalValues), null));
                    break;
            }
        }
    }

    private static ActivityLog CreateActivityLog(
        string action,
        string entityType,
        Guid? entityId,
        string? oldValues,
        string? newValues)
    {
        return new ActivityLog
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Empty, // Será preenchido pelo ICurrentUserService futuramente
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            Details = "{}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private static string? SerializeProperties(PropertyValues values)
    {
        try
        {
            var properties = values.Properties
                .Where(p => !p.Name.EndsWith("Hash", StringComparison.OrdinalIgnoreCase) &&
                            !p.Name.EndsWith("Secret", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(
                    p => p.Name,
                    p => values[p]?.ToString() ?? "null"
                );

            return System.Text.Json.JsonSerializer.Serialize(properties);
        }
        catch
        {
            return null;
        }
    }

    private static Guid? GetEntityId(EntityEntry entry)
    {
        if (entry.Entity is Entity<Guid> entity)
        {
            return EqualityComparer<Guid>.Default.Equals(entity.Id, default)
                ? null
                : entity.Id;
        }
        return null;
    }
}
