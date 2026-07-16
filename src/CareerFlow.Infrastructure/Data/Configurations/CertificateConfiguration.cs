using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CareerFlow.Core.Entities;

namespace CareerFlow.Infrastructure.Data.Configurations;

public class CertificateConfiguration : BaseEntityConfiguration<Certificate>
{
    public override void Configure(EntityTypeBuilder<Certificate> builder)
    {
        base.Configure(builder);

        builder.ToTable("certificates");

        builder.Property(c => c.PersonId).HasColumnName("person_id").HasColumnType("uuid").IsRequired();
        builder.Property(c => c.Title).HasColumnName("title").HasColumnType("varchar(300)").IsRequired();
        builder.Property(c => c.Issuer).HasColumnName("issuer").HasColumnType("varchar(200)").IsRequired();
        builder.Property(c => c.IssueDate).HasColumnName("issue_date").HasColumnType("date").IsRequired();
        builder.Property(c => c.ExpirationDate).HasColumnName("expiration_date").HasColumnType("date");
        builder.Property(c => c.CertificateId).HasColumnName("certificate_id").HasColumnType("varchar(100)");
        builder.Property(c => c.CredentialId).HasColumnName("credential_id").HasColumnType("varchar(100)");
        builder.Property(c => c.CredentialUrl).HasColumnName("credential_url").HasColumnType("varchar(500)");
        builder.Property(c => c.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(c => c.IsVerified).HasColumnName("is_verified").HasDefaultValue(false);
        builder.Property(c => c.DisplayOrder).HasColumnName("display_order").HasDefaultValue(0);

        builder.HasIndex(c => c.PersonId).HasDatabaseName("idx_certificates_person");
        builder.HasIndex(c => new { c.PersonId, c.Issuer }).HasDatabaseName("idx_certificates_issuer");
    }
}
