using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;

namespace CareerFlow.Infrastructure.Data.Configurations;

public class EducationConfiguration : BaseEntityConfiguration<Education>
{
    public override void Configure(EntityTypeBuilder<Education> builder)
    {
        base.Configure(builder);

        builder.ToTable("educations");

        builder.Property(e => e.PersonId)
            .HasColumnName("person_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(e => e.Institution)
            .HasColumnName("institution")
            .HasColumnType("varchar(200)")
            .IsRequired();

        builder.Property(e => e.Course)
            .HasColumnName("course")
            .HasColumnType("varchar(200)")
            .IsRequired();

        builder.Property(e => e.EducationLevel)
            .HasColumnName("education_level")
            .HasColumnType("varchar(50)")
            .HasConversion<string>()
            .IsRequired();

        // CORRIGIDO: DefaultValue como string para enum
        builder.Property(e => e.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(30)")
            .HasConversion<string>()
            .HasDefaultValue(EducationStatus.Completed)
            .IsRequired();

        builder.Property(e => e.StartDate)
            .HasColumnName("start_date")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(e => e.EndDate)
            .HasColumnName("end_date")
            .HasColumnType("date");

        builder.Property(e => e.IsCurrent)
            .HasColumnName("is_current")
            .HasDefaultValue(false);

        builder.Property(e => e.Description)
            .HasColumnName("description")
            .HasColumnType("text");

        builder.Property(e => e.Grade)
            .HasColumnName("grade")
            .HasColumnType("varchar(20)");

        builder.Property(e => e.ThesisTitle)
            .HasColumnName("thesis_title")
            .HasColumnType("varchar(300)");

        builder.Property(e => e.DisplayOrder)
            .HasColumnName("display_order")
            .HasDefaultValue(0);

        // Índices
        builder.HasIndex(e => e.PersonId)
            .HasDatabaseName("idx_educations_person");

        builder.HasIndex(e => new { e.PersonId, e.IsCurrent })
            .HasDatabaseName("idx_educations_current");

        builder.HasIndex(e => new { e.PersonId, e.EducationLevel })
            .HasDatabaseName("idx_educations_level");
    }
}
