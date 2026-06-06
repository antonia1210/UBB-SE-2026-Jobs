using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Persistence.Configurations;

/// <summary>
/// Configuration for the Slot entity (scheduled interview slots for candidates).
/// </summary>
public class SlotConfiguration : IEntityTypeConfiguration<Slot>
    {
        public void Configure(EntityTypeBuilder<Slot> builder)
        {
            builder.HasKey(slot => slot.Id);

        builder.HasOne(slot => slot.Candidate)
            .WithMany()
            .HasForeignKey(slot => slot.CandidateId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(slot => slot.Recruiter)
            .WithMany()
            .HasForeignKey(slot => slot.RecruiterId)
            .OnDelete(DeleteBehavior.NoAction);

        // Complex relationship: Slot to Recruiter using composite key
        builder.HasOne(slot => slot.Recruiter)
            .WithMany()
            .HasForeignKey(slot => new { slot.RecruiterId, slot.RecruiterCompanyId })
            .HasPrincipalKey(recruiter => new { recruiter.UserId, recruiter.CompanyId });
    }
}

