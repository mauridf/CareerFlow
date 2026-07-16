using CareerFlow.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CareerFlow.Infrastructure.Data.Interceptors;

/// <summary>
/// Interceptor do EF Core que registra automaticamente
/// logs de auditoria para entidades modificadas.
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// Intercepta antes de salvar as mudanças para capturar
    /// os valores antigos e novos das entidades alteradas.
    /// </summary>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        TrackChanges(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Versão assíncrona do interceptor
    /// </summary>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        TrackChanges(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Rastreia mudanças e cria ActivityLogs
    /// </summary>
    private void TrackChanges(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added ||
                        e.State == EntityState.Modified ||
                        e.State == EntityState.Deleted)
            .Where(e => e.Entity is not ActivityLog); // Não auditar a própria auditoria

        foreach (var entry in entries)
        {
            var entityType = entry.Entity.GetType().Name;
            var entityId = GetEntityId(entry);

            switch (entry.State)
            {
                case EntityState.Added:
                    CreateActivityLog(context, entry, "created", entityType, entityId);
                    break;

                case EntityState.Modified:
                    CreateActivityLog(context, entry, "updated", entityType, entityId);
                    break;

                case EntityState.Deleted:
                    CreateActivityLog(context, entry, "deleted", entityType, entityId);
                    break;
            }
        }
    }

    /// <summary>
    /// Cria um registro de ActivityLog
    /// </summary>
    private void CreateActivityLog(
        DbContext context,
        EntityEntry entry,
        string action,
        string entityType,
        Guid? entityId)
    {
        // Obtém o UserId do contexto atual (se disponível)
        // Nota: isso será configurado via ICurrentUserService futuramente
        var userId = GetCurrentUserId();

        if (userId == null) return; // Sem usuário, não audita

        var activityLog = new ActivityLog
        {
            UserId = userId.Value,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = action == "updated" || action == "deleted"
                ? SerializeProperties(entry.OriginalValues)
                : null,
            NewValues = action == "created" || action == "updated"
                ? SerializeProperties(entry.CurrentValues)
                : null,
            Details = "{}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Add(activityLog);
    }

    /// <summary>
    /// Serializa propriedades da entidade para JSON
    /// </summary>
    private static string? SerializeProperties(IPropertyValues values)
    {
        try
        {
            var properties = values.Properties
                .Where(p => !p.Name.EndsWith("Hash") && // Não loga senhas
                            !p.Name.EndsWith("Secret"))  // Não loga segredos
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

    /// <summary>
    /// Obtém o ID da entidade a partir do entry
    /// </summary>
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

    /// <summary>
    /// Obtém o ID do usuário atual (placeholder)
    /// Será substituído pela implementação real via ICurrentUserService
    /// </summary>
    private static Guid? GetCurrentUserId()
    {
        // Placeholder: retorna um GUID fixo para desenvolvimento
        // Em produção, será injetado via ICurrentUserService
        return Guid.Parse("00000000-0000-0000-0000-000000000001");
    }
}
