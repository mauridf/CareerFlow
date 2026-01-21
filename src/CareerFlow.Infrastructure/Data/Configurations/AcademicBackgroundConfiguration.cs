using CareerFlow.Domain.Entities;
using CareerFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerFlow.Infrastructure.Data.Configurations;

public class AcademicBackgroundConfiguration : IEntityTypeConfiguration<AcademicBackground>
{
    public void Configure(EntityTypeBuilder<AcademicBackground> builder)
    {
        builder.HasKey(ab => ab.Id);

        builder.Property(ab => ab.Institution)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ab => ab.CourseName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ab => ab.Level)
            .IsRequired()
            .HasConversion(
                v => v.Name,
                v => EducationLevel.FromName(v, false));

        builder.Property(ab => ab.DiplomaPath)
            .HasMaxLength(500);
    }
}