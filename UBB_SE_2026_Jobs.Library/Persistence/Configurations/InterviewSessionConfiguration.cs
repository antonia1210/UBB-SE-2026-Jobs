using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UBB_SE_2026_Jobs.Library.Domain.Core;

namespace UBB_SE_2026_Jobs.Library.Persistence.Configurations;

/// <summary>
/// Configuration for the InterviewSession entity (actual interview sessions between candidates and recruiters).
/// </summary>
public class InterviewSessionConfiguration : IEntityTypeConfiguration<InterviewSession>
{
    public void Configure(EntityTypeBuilder<InterviewSession> builder)
        {
            builder.ToTable("interview_sessions");
            builder.HasKey(interviewSession => interviewSession.Id);

        builder.HasOne(interviewSession => interviewSession.User)
            .WithMany()
            .HasForeignKey(interviewSession => interviewSession.ExternalUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

