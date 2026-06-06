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
        builder.HasKey(q => q.Id);

        builder.HasOne(q => q.Test)
            .WithMany(t => t.Questions)
            .HasForeignKey(q => q.TestId)
            .OnDelete(DeleteBehavior.SetNull);

        // Seed five mixed-type questions for each static compatibility skill-group test.
        builder.HasData(TestCatalogSeed.Questions);
    }
}
