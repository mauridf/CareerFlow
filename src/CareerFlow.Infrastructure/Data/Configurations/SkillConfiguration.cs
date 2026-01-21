using CareerFlow.Domain.Entities;
using CareerFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerFlow.Infrastructure.Data.Configurations;

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Type)
            .IsRequired()
            .HasConversion(
                v => v.Name,
                v => SkillType.FromName(v, false));

        builder.Property(s => s.Level)
            .IsRequired()
            .HasConversion(
                v => v.Name,
                v => SkillLevel.FromName(v, false));
    }
}