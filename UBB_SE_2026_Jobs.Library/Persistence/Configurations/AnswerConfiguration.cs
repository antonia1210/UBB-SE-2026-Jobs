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
            builder.HasKey(answer => answer.Id);

        builder.HasOne(answer => answer.TestAttempt)
            .WithMany(testAttempt => testAttempt.Answers)
            .HasForeignKey(answer => answer.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

