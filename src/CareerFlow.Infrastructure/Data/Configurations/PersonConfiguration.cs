using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CareerFlow.Core.Entities;

namespace CareerFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade Person para o EF Core.
/// </summary>
public class PersonConfiguration : BaseEntityConfiguration<Person>
{
    public override void Configure(EntityTypeBuilder<Person> builder)
    {
        base.Configure(builder);

        builder.ToTable("persons");

        // ============================================
        // Relacionamento com User
        // ============================================
        builder.Property(p => p.UserId)
            .HasColumnName("user_id")
            .HasColumnType("uuid")
            .IsRequired();

        // ============================================
        // Dados Pessoais
        // ============================================
        builder.Property(p => p.Phone)
            .HasColumnName("phone")
            .HasColumnType("varchar(20)");

        builder.Property(p => p.City)
            .HasColumnName("city")
            .HasColumnType("varchar(100)");

        builder.Property(p => p.State)
            .HasColumnName("state")
            .HasColumnType("varchar(2)");

        builder.Property(p => p.BirthDate)
            .HasColumnName("birth_date")
            .HasColumnType("date");

        builder.Property(p => p.ProfessionalSummary)
            .HasColumnName("professional_summary")
            .HasColumnType("text");

        builder.Property(p => p.PhotoUrl)
            .HasColumnName("photo_url")
            .HasColumnType("varchar(500)");

        // ============================================
        // Profissão Atual
        // ============================================
        builder.Property(p => p.CurrentPosition)
            .HasColumnName("current_position")
            .HasColumnType("varchar(100)");

        builder.Property(p => p.CurrentCompany)
            .HasColumnName("current_company")
            .HasColumnType("varchar(100)");

        // ============================================
        // Visibilidade
        // ============================================
        builder.Property(p => p.IsPublic)
            .HasColumnName("is_public")
            .HasDefaultValue(true);

        builder.Property(p => p.ResumeSlug)
            .HasColumnName("resume_slug")
            .HasColumnType("varchar(100)");

        // ============================================
        // Índices
        // ============================================
        builder.HasIndex(p => p.UserId)
            .IsUnique()
            .HasDatabaseName("idx_persons_user");

        builder.HasIndex(p => p.ResumeSlug)
            .IsUnique()
            .HasFilter("resume_slug IS NOT NULL")
            .HasDatabaseName("idx_persons_slug");

        builder.HasIndex(p => p.IsPublic)
            .HasDatabaseName("idx_persons_public");
    }
}
