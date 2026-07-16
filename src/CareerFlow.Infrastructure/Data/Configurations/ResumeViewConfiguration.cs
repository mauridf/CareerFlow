using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CareerFlow.Core.Entities;

namespace CareerFlow.Infrastructure.Data.Configurations;

public class ResumeViewConfiguration : BaseEntityConfiguration<ResumeView>
{
    public override void Configure(EntityTypeBuilder<ResumeView> builder)
    {
        base.Configure(builder);

        builder.ToTable("resume_views");

        builder.Property(r => r.PersonId).HasColumnName("person_id").HasColumnType("uuid").IsRequired();
        builder.Property(r => r.IpAddress).HasColumnName("ip_address").HasColumnType("inet");
        builder.Property(r => r.UserAgent).HasColumnName("user_agent").HasColumnType("text");
        builder.Property(r => r.Referrer).HasColumnName("referrer").HasColumnType("varchar(500)");
        builder.Property(r => r.ViewerCountry).HasColumnName("viewer_country").HasColumnType("varchar(100)");
        builder.Property(r => r.ViewerCity).HasColumnName("viewer_city").HasColumnType("varchar(100)");
        builder.Property(r => r.Source).HasColumnName("source").HasColumnType("varchar(50)");
        builder.Property(r => r.ViewDurationSeconds).HasColumnName("view_duration_seconds");
        builder.Property(r => r.PdfDownloaded).HasColumnName("pdf_downloaded").HasDefaultValue(false);

        // Apenas CreatedAt (não atualizável)
        builder.Property(r => r.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").HasDefaultValueSql("NOW()");

        builder.HasIndex(r => new { r.PersonId, r.CreatedAt }).IsDescending(false, true).HasDatabaseName("idx_resume_views_person");
        builder.HasIndex(r => new { r.PersonId, r.Source }).HasDatabaseName("idx_resume_views_source");
    }
}
