using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UBB_SE_2026_Jobs.Library.Domain.Core;

namespace UBB_SE_2026_Jobs.Library.Persistence.Configurations;

/// <summary>
/// Configuration for the Test entity.
/// </summary>
public class TestConfiguration : IEntityTypeConfiguration<Test>
{
    public void Configure(EntityTypeBuilder<Test> builder)
    {
        builder.HasKey(test => test.Id);

        builder.HasMany(test => test.Questions)
            .WithOne(question => question.Test)
            .HasForeignKey(question => question.TestId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(test => test.Skill)
            .WithMany()
            .HasForeignKey(test => test.SkillId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed one test per compatibility skill group. Questions are seeded in
        // QuestionConfiguration from the same catalog.
        builder.HasData(TestCatalogSeed.Tests);
    }
}
