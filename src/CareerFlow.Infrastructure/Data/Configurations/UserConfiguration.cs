using CareerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerFlow.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Phone)
            .HasMaxLength(20);

        builder.Property(u => u.City)
            .HasMaxLength(100);

        builder.Property(u => u.State)
            .HasMaxLength(2);

        builder.Property(u => u.PhotoPath)
            .HasMaxLength(500);

        // Relacionamentos
        builder.HasOne(u => u.ProfessionalSummary)
            .WithOne(ps => ps.User)
            .HasForeignKey<ProfessionalSummary>(ps => ps.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.SocialMedias)
            .WithOne(sm => sm.User)
            .HasForeignKey(sm => sm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Skills)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.ProfessionalExperiences)
            .WithOne(pe => pe.User)
            .HasForeignKey(pe => pe.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.AcademicBackgrounds)
            .WithOne(ab => ab.User)
            .HasForeignKey(ab => ab.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Certificates)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Languages)
            .WithOne(l => l.User)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}