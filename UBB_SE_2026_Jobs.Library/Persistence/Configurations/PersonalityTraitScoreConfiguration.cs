using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Persistence.Configurations;

public class PersonalityTraitScoreConfiguration : IEntityTypeConfiguration<PersonalityTraitScore>
{
    public void Configure(EntityTypeBuilder<PersonalityTraitScore> builder)
    {
        builder.ToTable("PersonalityTraitScores");
        builder.HasKey(skill => skill.PersonalityTraitScoreId);

        builder.Property(skill => skill.Trait).HasConversion<string>().HasMaxLength(40);

        // Cascade is configured from PersonalityTestResultConfiguration so the trait score rows
        // disappear with their parent.
        builder.HasIndex("PersonalityTestResultId");
    }
}

