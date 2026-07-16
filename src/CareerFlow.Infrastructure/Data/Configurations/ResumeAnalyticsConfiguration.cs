using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerFlow.Infrastructure.Data.Configurations;

public class ResumeAnalyticsConfiguration : BaseEntityConfiguration<ResumeAnalytics>
{
    public override void Configure(EntityTypeBuilder<ResumeAnalytics> builder)
    {
        base.Configure(builder);

        builder.ToTable("resume_analytics");

        builder.Property(r => r.PersonId).HasColumnName("person_id").HasColumnType("uuid").IsRequired();
        builder.Property(r => r.TotalViews).HasColumnName("total_views").HasDefaultValue(0);
        builder.Property(r => r.UniqueViews).HasColumnName("unique_views").HasDefaultValue(0);
        builder.Property(r => r.PdfDownloads).HasColumnName("pdf_downloads").HasDefaultValue(0);
        builder.Property(r => r.SharesCount).HasColumnName("shares_count").HasDefaultValue(0);
        builder.Property(r => r.AtsScore).HasColumnName("ats_score");
        builder.Property(r => r.AtsCompatibility).HasColumnName("ats_compatibility");
        builder.Property(r => r.AtsIssues).HasColumnName("ats_issues").HasColumnType("text");
        builder.Property(r => r.AtsSuggestions).HasColumnName("ats_suggestions").HasColumnType("text");
        builder.Property(r => r.AverageViewDurationSeconds).HasColumnName("average_view_duration_seconds");
        builder.Property(r => r.LastViewedAt).HasColumnName("last_viewed_at").HasColumnType("timestamptz");
        builder.Property(r => r.DetectedKeywords).HasColumnName("detected_keywords").HasColumnType("jsonb");
        builder.Property(r => r.MissingKeywords).HasColumnName("missing_keywords").HasColumnType("jsonb");
        builder.Property(r => r.AnalyzedAt).HasColumnName("analyzed_at").HasColumnType("timestamptz");
        builder.Property(r => r.Status).HasColumnName("status").HasColumnType("varchar(30)").HasConversion<string>().HasDefaultValue(ResumeStatus.Draft);
        builder.Property(r => r.PublishedAt).HasColumnName("published_at").HasColumnType("timestamptz");

        builder.HasIndex(r => r.PersonId).IsUnique().HasDatabaseName("idx_resume_analytics_person");
    }
}
