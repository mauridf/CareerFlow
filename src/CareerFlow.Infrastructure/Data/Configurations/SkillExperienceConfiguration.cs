using CareerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerFlow.Infrastructure.Data.Configurations;

public class SkillExperienceConfiguration : IEntityTypeConfiguration<SkillExperience>
{
    public void Configure(EntityTypeBuilder<SkillExperience> builder)
    {
        builder.HasKey(se => se.Id);

        // Configurar índice único para evitar duplicatas
        builder.HasIndex(se => new { se.SkillId, se.ProfessionalExperienceId })
            .IsUnique();

        builder.HasOne(se => se.Skill)
            .WithMany(s => s.SkillExperiences)
            .HasForeignKey(se => se.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(se => se.ProfessionalExperience)
            .WithMany(pe => pe.SkillExperiences)
            .HasForeignKey(se => se.ProfessionalExperienceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}