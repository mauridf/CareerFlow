using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CareerFlow.Core.Entities;

namespace CareerFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade User para o Entity Framework Core.
/// </summary>
public class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        // Nome da tabela
        builder.ToTable("users");

        // ============================================
        // Propriedades
        // ============================================

        builder.Property(u => u.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(200)")
            .IsRequired();

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasColumnType("varchar(200)")
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .HasColumnType("varchar(300)")
            .IsRequired();

        // ============================================
        // Autenticação
        // ============================================

        builder.Property(u => u.EmailVerifiedAt)
            .HasColumnName("email_verified_at")
            .HasColumnType("timestamptz");

        builder.Property(u => u.LastLoginAt)
            .HasColumnName("last_login_at")
            .HasColumnType("timestamptz");

        builder.Property(u => u.LastPasswordChangeAt)
            .HasColumnName("last_password_change_at")
            .HasColumnType("timestamptz");

        builder.Property(u => u.FailedLoginAttempts)
            .HasColumnName("failed_login_attempts")
            .HasDefaultValue(0);

        builder.Property(u => u.LockedUntil)
            .HasColumnName("locked_until")
            .HasColumnType("timestamptz");

        builder.Property(u => u.TwoFactorEnabled)
            .HasColumnName("two_factor_enabled")
            .HasDefaultValue(false);

        builder.Property(u => u.TwoFactorSecret)
            .HasColumnName("two_factor_secret")
            .HasColumnType("varchar(100)");

        // ============================================
        // Role e Status
        // ============================================

        builder.Property(u => u.Role)
            .HasColumnName("role")
            .HasColumnType("varchar(30)")
            .HasDefaultValue("user")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(u => u.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(u => u.IsPremium)
            .HasColumnName("is_premium")
            .HasDefaultValue(false);

        builder.Property(u => u.PremiumUntil)
            .HasColumnName("premium_until")
            .HasColumnType("timestamptz");

        // ============================================
        // Preferências (JSONB)
        // ============================================

        builder.Property(u => u.NotificationPreferences)
            .HasColumnName("notification_preferences")
            .HasColumnType("jsonb")
            .HasDefaultValue("{}");

        builder.Property(u => u.ThemePreferences)
            .HasColumnName("theme_preferences")
            .HasColumnType("jsonb")
            .HasDefaultValue("{}");

        // ============================================
        // Soft Delete
        // ============================================

        builder.Property(u => u.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamptz");

        // ============================================
        // Índices
        // ============================================

        // Email único (apenas registros não deletados)
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter("deleted_at IS NULL")
            .HasDatabaseName("idx_users_email");

        // Índice para login (email + campos essenciais)
        builder.HasIndex(u => u.Email)
            .IncludeProperties(u => new { u.Id, u.PasswordHash, u.IsActive, u.LockedUntil })
            .HasDatabaseName("idx_users_login");

        // Índice para usuários premium
        builder.HasIndex(u => new { u.IsPremium, u.PremiumUntil })
            .HasDatabaseName("idx_users_premium");

        // Índice para busca por role
        builder.HasIndex(u => u.Role)
            .HasDatabaseName("idx_users_role");

        // ============================================
        // Relacionamentos
        // ============================================

        // User -> Person (1:1)
        builder.HasOne(u => u.Person)
            .WithOne(p => p.User)
            .HasForeignKey<Person>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> ActivityLogs (1:N)
        builder.HasMany(u => u.ActivityLogs)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
