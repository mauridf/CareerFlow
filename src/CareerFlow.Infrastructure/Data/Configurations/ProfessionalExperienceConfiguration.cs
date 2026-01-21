using CareerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerFlow.Infrastructure.Data.Configurations;

public class ProfessionalExperienceConfiguration : IEntityTypeConfiguration<ProfessionalExperience>
{
    public void Configure(EntityTypeBuilder<ProfessionalExperience> builder)
    {
        builder.HasKey(pe => pe.Id);

        builder.Property(pe => pe.Company)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pe => pe.Position)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pe => pe.Responsibilities)
            .IsRequired()
            .HasMaxLength(2000);
    }
}