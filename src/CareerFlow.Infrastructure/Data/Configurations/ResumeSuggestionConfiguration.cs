using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CareerFlow.Core.Entities;

namespace CareerFlow.Infrastructure.Data.Configurations;

public class ResumeSuggestionConfiguration : BaseEntityConfiguration<ResumeSuggestion>
{
    public override void Configure(EntityTypeBuilder<ResumeSuggestion> builder)
    {
        base.Configure(builder);

        builder.ToTable("resume_suggestions");

        builder.Property(r => r.PersonId).HasColumnName("person_id").HasColumnType("uuid").IsRequired();
        builder.Property(r => r.Category).HasColumnName("category").HasColumnType("varchar(50)").IsRequired();
        builder.Property(r => r.Title).HasColumnName("title").HasColumnType("varchar(200)").IsRequired();
        builder.Property(r => r.Description).HasColumnName("description").HasColumnType("text");
        builder.Property(r => r.Priority).HasColumnName("priority").HasColumnType("varchar(30)").HasDefaultValue("medium");
        builder.Property(r => r.IsApplied).HasColumnName("is_applied").HasDefaultValue(false);
        builder.Property(r => r.AppliedAt).HasColumnName("applied_at").HasColumnType("timestamptz");

        builder.HasIndex(r => new { r.PersonId, r.IsApplied }).HasDatabaseName("idx_resume_suggestions_person");
    }
}
