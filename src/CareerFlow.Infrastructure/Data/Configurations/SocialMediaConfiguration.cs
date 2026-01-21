using CareerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerFlow.Infrastructure.Data.Configurations;

public class SocialMediaConfiguration : IEntityTypeConfiguration<SocialMedia>
{
    public void Configure(EntityTypeBuilder<SocialMedia> builder)
    {
        builder.HasKey(sm => sm.Id);

        builder.Property(sm => sm.Platform)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sm => sm.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(sm => new { sm.UserId, sm.Platform })
            .IsUnique();
    }
}