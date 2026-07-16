using CareerFlow.Core.Entities;
using CareerFlow.Infrastructure.Data.Configurations;
using CareerFlow.Infrastructure.Data.Interceptors;
using CareerFlow.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CareerFlow.Infrastructure.Data;

/// <summary>
/// Contexto principal do Entity Framework Core para o CareerFlow.
/// Gerencia todas as entidades, configurações e interceptors.
/// </summary>
public class CareerFlowDbContext : DbContext
{
    // ============================================
    // DbSets (Tabelas)
    // ============================================

    public DbSet<User> Users => Set<User>();
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<SocialNetwork> SocialNetworks => Set<SocialNetwork>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Experience> Experiences => Set<Experience>();
    public DbSet<Education> Educations => Set<Education>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<ResumeView> ResumeViews => Set<ResumeView>();
    public DbSet<ResumeAnalytics> ResumeAnalytics => Set<ResumeAnalytics>();
    public DbSet<ResumeSuggestion> ResumeSuggestions => Set<ResumeSuggestion>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    // ============================================
    // Interceptors
    // ============================================
    private readonly AuditInterceptor? _auditInterceptor;
    private readonly DomainEventInterceptor? _domainEventInterceptor;

    /// <summary>
    /// Construtor para injeção de dependência
    /// </summary>
    public CareerFlowDbContext(
        DbContextOptions<CareerFlowDbContext> options,
        AuditInterceptor? auditInterceptor = null,
        DomainEventInterceptor? domainEventInterceptor = null)
        : base(options)
    {
        _auditInterceptor = auditInterceptor;
        _domainEventInterceptor = domainEventInterceptor;
    }

    /// <summary>
    /// Configuração do modelo e entidades
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CareerFlowDbContext).Assembly);
        ConfigureGlobalSettings(modelBuilder);
    }

    /// <summary>
    /// Configuração de interceptors e comportamentos
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var interceptors = new List<IInterceptor>();

        if (_auditInterceptor != null)
            interceptors.Add(_auditInterceptor);

        if (_domainEventInterceptor != null)
            interceptors.Add(_domainEventInterceptor);

        if (interceptors.Count > 0)
            optionsBuilder.AddInterceptors(interceptors.ToArray());
    }

    /// <summary>
    /// Configurações globais para todas as entidades
    /// </summary>
    private static void ConfigureGlobalSettings(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties()
                .Where(p => p.ClrType == typeof(string)))
            {
                property.SetMaxLength(500);
            }

            foreach (var property in entityType.GetProperties()
                .Where(p => p.ClrType == typeof(DateTime)))
            {
                property.SetColumnType("timestamptz");
            }
        }
    }

    /// <summary>
    /// Salva mudanças e dispara eventos de domínio
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }

    /// <summary>
    /// Atualiza campos de auditoria (CreatedAt, UpdatedAt)
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is Core.Entities.Entity<Guid> entity)
            {
                if (entry.State == EntityState.Added)
                {
                    entity.SetId(Guid.NewGuid());
                }

                if (entry.State == EntityState.Modified)
                {
                    entity.MarkAsUpdated();
                }
            }
        }
    }
}
