namespace CareerFlow.Core.Entities;

/// <summary>
/// Log de atividades do usuário para auditoria.
/// Esta entidade tem setters públicos pois é criada pelo AuditInterceptor.
/// </summary>
public class ActivityLog : Entity<Guid>
{
    /// <summary>ID do usuário que realizou a ação</summary>
    public Guid UserId { get; set; }

    /// <summary>Usuário associado (navigation property)</summary>
    public User? User { get; set; }

    /// <summary>Ação realizada (created, updated, deleted, login, etc.)</summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>Tipo da entidade afetada</summary>
    public string? EntityType { get; set; }

    /// <summary>ID da entidade afetada</summary>
    public Guid? EntityId { get; set; }

    /// <summary>Valores antigos (JSON) - apenas para updates/deletes</summary>
    public string? OldValues { get; set; }

    /// <summary>Valores novos (JSON) - apenas para creates/updates</summary>
    public string? NewValues { get; set; }

    /// <summary>Detalhes adicionais (JSON)</summary>
    public string? Details { get; set; } = "{}";

    /// <summary>IP do usuário</summary>
    public string? IpAddress { get; set; }

    /// <summary>User-Agent do navegador</summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Cria um novo log de atividade
    /// </summary>
    public static ActivityLog Create(
        Guid userId,
        string action,
        string? entityType = null,
        Guid? entityId = null,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null,
        string? userAgent = null)
    {
        return new ActivityLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            Details = "{}",
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
