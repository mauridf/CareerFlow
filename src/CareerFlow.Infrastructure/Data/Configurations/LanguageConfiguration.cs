using CareerFlow.Domain.Entities;
using CareerFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerFlow.Infrastructure.Data.Configurations;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.Level)
            .IsRequired()
            .HasConversion(
                v => v.Name,
                v => LanguageLevel.FromName(v, false));
    }
}