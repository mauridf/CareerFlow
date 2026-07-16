using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CareerFlow.Core.Entities;

namespace CareerFlow.Infrastructure.Data.Configurations;

public class SocialNetworkConfiguration : BaseEntityConfiguration<SocialNetwork>
{
    public override void Configure(EntityTypeBuilder<SocialNetwork> builder)
    {
        base.Configure(builder);

        builder.ToTable("social_networks");

        builder.Property(s => s.PersonId).HasColumnName("person_id").HasColumnType("uuid").IsRequired();
        builder.Property(s => s.NetworkType).HasColumnName("network_type").HasColumnType("varchar(30)").HasConversion<string>().IsRequired();
        builder.Property(s => s.Url).HasColumnName("url").HasColumnType("varchar(500)").IsRequired();
        builder.Property(s => s.DisplayOrder).HasColumnName("display_order").HasDefaultValue(0);

        builder.HasIndex(s => s.PersonId).HasDatabaseName("idx_social_networks_person");
        builder.HasIndex(s => new { s.PersonId, s.NetworkType }).IsUnique().HasDatabaseName("idx_social_networks_unique");
    }
}
