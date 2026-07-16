namespace CareerFlow.Core.Entities;

/// <summary>
/// Classe base para todas as entidades do domínio.
/// Todas as entidades possuem Id, CreatedAt e UpdatedAt.
/// </summary>
public abstract class Entity<TId> : IEquatable<Entity<TId>>
{
    /// <summary>Identificador único da entidade</summary>
    public TId Id { get; set; } = default!;

    /// <summary>Data de criação do registro</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Data da última atualização</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Atualiza a data de modificação
    /// </summary>
    public void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Define o ID da entidade (usado pelo EF Core ou geração manual)
    /// </summary>
    public void SetId(TId id)
    {
        if (EqualityComparer<TId>.Default.Equals(Id, default!) ||
            EqualityComparer<TId>.Default.Equals(Id, default(TId)!))
        {
            Id = id;
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (EqualityComparer<TId>.Default.Equals(Id, default!) ||
            EqualityComparer<TId>.Default.Equals(other.Id, default!))
            return false;

        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public bool Equals(Entity<TId>? other)
    {
        return Equals((object?)other);
    }

    public override int GetHashCode()
    {
        return Id?.GetHashCode() ?? 0;
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !Equals(left, right);
    }
}
