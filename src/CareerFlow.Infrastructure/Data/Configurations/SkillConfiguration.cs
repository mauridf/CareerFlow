using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CareerFlow.Core.Entities;

namespace CareerFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade Skill.
/// </summary>
public class SkillConfiguration : BaseEntityConfiguration<Skill>
{
    public override void Configure(EntityTypeBuilder<Skill> builder)
    {
        base.Configure(builder);

        builder.ToTable("skills");

        // Relacionamento
        builder.Property(s => s.PersonId)
            .HasColumnName("person_id")
            .HasColumnType("uuid")
            .IsRequired();

        // Propriedades
        builder.Property(s => s.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(s => s.Category)
            .HasColumnName("category")
            .HasColumnType("varchar(50)")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(s => s.ProficiencyLevel)
            .HasColumnName("proficiency_level")
            .HasColumnType("varchar(30)")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(s => s.IsPrimary)
            .HasColumnName("is_primary")
            .HasDefaultValue(false);

        builder.Property(s => s.DisplayOrder)
            .HasColumnName("display_order")
            .HasDefaultValue(0);

        // Índices
        builder.HasIndex(s => s.PersonId)
            .HasDatabaseName("idx_skills_person");

        builder.HasIndex(s => new { s.PersonId, s.Category })
            .HasDatabaseName("idx_skills_category");

        builder.HasIndex(s => new { s.PersonId, s.ProficiencyLevel })
            .HasDatabaseName("idx_skills_level");
    }
}
