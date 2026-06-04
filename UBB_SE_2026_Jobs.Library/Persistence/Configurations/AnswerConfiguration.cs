using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UBB_SE_2026_Jobs.Library.Domain.Core;

namespace UBB_SE_2026_Jobs.Library.Persistence.Configurations;

/// <summary>
/// Configuration for the Answer entity.
/// </summary>
public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.HasKey(a => a.Id);

        builder.HasOne(a => a.TestAttempt)
            .WithMany(ta => ta.Answers)
            .HasForeignKey(a => a.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

