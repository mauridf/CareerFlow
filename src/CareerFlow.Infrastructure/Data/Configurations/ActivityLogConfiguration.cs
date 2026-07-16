using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CareerFlow.Core.Entities;

namespace CareerFlow.Infrastructure.Data.Configurations;

public class ActivityLogConfiguration : BaseEntityConfiguration<ActivityLog>
{
    public override void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        base.Configure(builder);

        builder.ToTable("activity_logs");

        builder.Property(a => a.UserId).HasColumnName("user_id").HasColumnType("uuid").IsRequired();
        builder.Property(a => a.Action).HasColumnName("action").HasColumnType("varchar(100)").IsRequired();
        builder.Property(a => a.EntityType).HasColumnName("entity_type").HasColumnType("varchar(50)");
        builder.Property(a => a.EntityId).HasColumnName("entity_id").HasColumnType("uuid");
        builder.Property(a => a.OldValues).HasColumnName("old_values").HasColumnType("jsonb");
        builder.Property(a => a.NewValues).HasColumnName("new_values").HasColumnType("jsonb");
        builder.Property(a => a.Details).HasColumnName("details").HasColumnType("jsonb").HasDefaultValue("{}");
        builder.Property(a => a.IpAddress).HasColumnName("ip_address").HasColumnType("inet");
        builder.Property(a => a.UserAgent).HasColumnName("user_agent").HasColumnType("text");

        builder.HasIndex(a => new { a.UserId, a.CreatedAt }).IsDescending(false, true).HasDatabaseName("idx_activity_logs_user");
        builder.HasIndex(a => new { a.UserId, a.Action }).HasDatabaseName("idx_activity_logs_action");
    }
}
