using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UBB_SE_2026_Jobs.Library.Domain.Core;

namespace UBB_SE_2026_Jobs.Library.Persistence.Configurations;

/// <summary>
/// Configuration for the TestAttempt entity.
/// </summary>
public class TestAttemptConfiguration : IEntityTypeConfiguration<TestAttempt>
    {
        public void Configure(EntityTypeBuilder<TestAttempt> builder)
        {
            builder.HasKey(testAttempt => testAttempt.Id);

        builder.HasOne(testAttempt => testAttempt.User)
            .WithMany()
            .HasForeignKey(testAttempt => testAttempt.ExternalUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(testAttempt => testAttempt.Answers)
            .WithOne(answer => answer.TestAttempt)
            .HasForeignKey(answer => answer.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

