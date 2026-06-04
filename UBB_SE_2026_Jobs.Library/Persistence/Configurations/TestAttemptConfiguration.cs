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
            builder.HasKey(ta => ta.Id);

        builder.HasOne(ta => ta.User)
            .WithMany()
            .HasForeignKey(ta => ta.ExternalUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(ta => ta.Answers)
            .WithOne(a => a.TestAttempt)
            .HasForeignKey(a => a.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

