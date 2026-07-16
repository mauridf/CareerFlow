using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CareerFlow.Core.Entities;

namespace CareerFlow.Infrastructure.Data.Configurations;

public class LanguageConfiguration : BaseEntityConfiguration<Language>
{
    public override void Configure(EntityTypeBuilder<Language> builder)
    {
        base.Configure(builder);

        builder.ToTable("languages");

        builder.Property(l => l.PersonId).HasColumnName("person_id").HasColumnType("uuid").IsRequired();
        builder.Property(l => l.LanguageName).HasColumnName("language_name").HasColumnType("varchar(50)").IsRequired();
        builder.Property(l => l.ProficiencyLevel).HasColumnName("proficiency_level").HasColumnType("varchar(30)").HasConversion<string>().IsRequired();
        builder.Property(l => l.IsNative).HasColumnName("is_native").HasDefaultValue(false);
        builder.Property(l => l.ReadingLevel).HasColumnName("reading_level").HasColumnType("varchar(30)");
        builder.Property(l => l.WritingLevel).HasColumnName("writing_level").HasColumnType("varchar(30)");
        builder.Property(l => l.ListeningLevel).HasColumnName("listening_level").HasColumnType("varchar(30)");
        builder.Property(l => l.SpeakingLevel).HasColumnName("speaking_level").HasColumnType("varchar(30)");
        builder.Property(l => l.DisplayOrder).HasColumnName("display_order").HasDefaultValue(0);

        builder.HasIndex(l => l.PersonId).HasDatabaseName("idx_languages_person");
        builder.HasIndex(l => new { l.PersonId, l.LanguageName }).IsUnique().HasDatabaseName("idx_languages_unique");
    }
}
