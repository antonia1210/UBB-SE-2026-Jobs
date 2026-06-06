using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UBB_SE_2026_Jobs.Library.Domain.Core;

namespace UBB_SE_2026_Jobs.Library.Persistence.Configurations;

/// <summary>
/// Configuration for the TestQuestion entity.
/// </summary>
public class QuestionConfiguration : IEntityTypeConfiguration<TestQuestion>
{
    public void Configure(EntityTypeBuilder<TestQuestion> builder)
    {
        builder.HasKey(question => question.Id);

        builder.HasOne(question => question.Test)
            .WithMany(test => test.Questions)
            .HasForeignKey(question => question.TestId)
            .OnDelete(DeleteBehavior.SetNull);

        // Seed five mixed-type questions for each static compatibility skill-group test.
        builder.HasData(TestCatalogSeed.Questions);
    }
}
