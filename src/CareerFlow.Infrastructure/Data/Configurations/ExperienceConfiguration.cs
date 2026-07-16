using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CareerFlow.Core.Entities;

namespace CareerFlow.Infrastructure.Data.Configurations;

public class ExperienceConfiguration : BaseEntityConfiguration<Experience>
{
    public override void Configure(EntityTypeBuilder<Experience> builder)
    {
        base.Configure(builder);

        builder.ToTable("experiences");

        builder.Property(e => e.PersonId).HasColumnName("person_id").HasColumnType("uuid").IsRequired();
        builder.Property(e => e.CompanyName).HasColumnName("company_name").HasColumnType("varchar(200)").IsRequired();
        builder.Property(e => e.Position).HasColumnName("position").HasColumnType("varchar(200)").IsRequired();
        builder.Property(e => e.StartDate).HasColumnName("start_date").HasColumnType("date").IsRequired();
        builder.Property(e => e.EndDate).HasColumnName("end_date").HasColumnType("date");
        builder.Property(e => e.IsCurrent).HasColumnName("is_current").HasDefaultValue(false);
        builder.Property(e => e.Description).HasColumnName("description").HasColumnType("text");
        builder.Property(e => e.SkillsUsed).HasColumnName("skills_used").HasColumnType("uuid[]");
        builder.Property(e => e.City).HasColumnName("city").HasColumnType("varchar(100)");
        builder.Property(e => e.State).HasColumnName("state").HasColumnType("varchar(2)");
        builder.Property(e => e.Country).HasColumnName("country").HasColumnType("varchar(100)").HasDefaultValue("Brasil");
        builder.Property(e => e.EmploymentType).HasColumnName("employment_type").HasColumnType("varchar(30)").HasConversion<string>();
        builder.Property(e => e.DisplayOrder).HasColumnName("display_order").HasDefaultValue(0);

        builder.HasIndex(e => e.PersonId).HasDatabaseName("idx_experiences_person");
        builder.HasIndex(e => new { e.PersonId, e.IsCurrent }).HasDatabaseName("idx_experiences_current");
        builder.HasIndex(e => new { e.PersonId, e.StartDate }).IsDescending(false, true).HasDatabaseName("idx_experiences_dates");
    }
}
