using CareerFlow.Domain.Common;
using CareerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CareerFlow.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<ProfessionalSummary> ProfessionalSummaries { get; set; } = null!;
    public DbSet<SocialMedia> SocialMedias { get; set; } = null!;
    public DbSet<Skill> Skills { get; set; } = null!;
    public DbSet<AcademicBackground> AcademicBackgrounds { get; set; } = null!;
    public DbSet<Certificate> Certificates { get; set; } = null!;
    public DbSet<Language> Languages { get; set; } = null!;
    public DbSet<ProfessionalExperience> ProfessionalExperiences { get; set; } = null!;
    public DbSet<SkillExperience> SkillExperiences { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas as configurações das entidades
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Atualizar UpdatedAt antes de salvar
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && (
                e.State == EntityState.Added ||
                e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            var entity = (BaseEntity)entityEntry.Entity;
            entity.UpdatedAt = DateTime.UtcNow;

            if (entityEntry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
        }
    }
}