using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerFlow.Infrastructure.Outbox;

/// <summary>
/// Configuração da entidade OutboxMessage para o EF Core.
/// </summary>
public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");

        // Chave primária
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd()
            .IsRequired();

        // Propriedades
        builder.Property(o => o.Type)
            .HasColumnName("type")
            .HasColumnType("varchar(500)")
            .IsRequired();

        builder.Property(o => o.Content)
            .HasColumnName("content")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(o => o.Headers)
            .HasColumnName("headers")
            .HasColumnType("jsonb")
            .HasDefaultValue("{}");

        builder.Property(o => o.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(20)")
            .HasDefaultValue("pending")
            .IsRequired();

        builder.Property(o => o.RetryCount)
            .HasColumnName("retry_count")
            .HasDefaultValue(0);

        builder.Property(o => o.MaxRetries)
            .HasColumnName("max_retries")
            .HasDefaultValue(5);

        builder.Property(o => o.LastError)
            .HasColumnName("last_error")
            .HasColumnType("text");

        builder.Property(o => o.ErrorStackTrace)
            .HasColumnName("error_stack_trace")
            .HasColumnType("text");

        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(o => o.ProcessedAt)
            .HasColumnName("processed_at")
            .HasColumnType("timestamptz");

        builder.Property(o => o.SentAt)
            .HasColumnName("sent_at")
            .HasColumnType("timestamptz");

        // Índices
        builder.HasIndex(o => new { o.Status, o.CreatedAt })
            .HasFilter("status = 'pending'")
            .HasDatabaseName("idx_outbox_pending");

        builder.HasIndex(o => o.Type)
            .HasDatabaseName("idx_outbox_type");

        builder.HasIndex(o => o.CreatedAt)
            .HasDatabaseName("idx_outbox_created");
    }
}
