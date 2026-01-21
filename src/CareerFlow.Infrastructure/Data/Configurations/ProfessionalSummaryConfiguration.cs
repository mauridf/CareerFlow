using CareerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerFlow.Infrastructure.Data.Configurations;

public class ProfessionalSummaryConfiguration : IEntityTypeConfiguration<ProfessionalSummary>
{
    public void Configure(EntityTypeBuilder<ProfessionalSummary> builder)
    {
        builder.HasKey(ps => ps.Id);

        builder.Property(ps => ps.Summary)
            .IsRequired()
            .HasMaxLength(2000);
    }
}