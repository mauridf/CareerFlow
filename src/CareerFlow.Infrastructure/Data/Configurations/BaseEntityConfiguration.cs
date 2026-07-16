using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CareerFlow.Core.Entities;

namespace CareerFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração base para todas as entidades que herdam de Entity<Guid>.
/// </summary>
public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity<Guid>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Chave primária
        builder.HasKey(e => e.Id);

        // ID gerado pelo banco (UUID)
        builder.Property(e => e.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd()
            .IsRequired();

        // Timestamps
        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        // Índice padrão por CreatedAt (para consultas ordenadas)
        builder.HasIndex(e => e.CreatedAt);
    }
}
