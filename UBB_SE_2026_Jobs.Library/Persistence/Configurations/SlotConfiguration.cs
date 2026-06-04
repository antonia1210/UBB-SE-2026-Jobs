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
            builder.HasKey(s => s.Id);

        builder.HasOne(s => s.Candidate)
            .WithMany()
            .HasForeignKey(s => s.CandidateId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.Recruiter)
            .WithMany()
            .HasForeignKey(s => s.RecruiterId)
            .OnDelete(DeleteBehavior.NoAction);

        // Complex relationship: Slot to Recruiter using composite key
        builder.HasOne(s => s.Recruiter)
            .WithMany()
            .HasForeignKey(s => new { s.RecruiterId, s.RecruiterCompanyId })
            .HasPrincipalKey(r => new { r.UserId, r.CompanyId });
    }
}

