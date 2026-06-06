using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Persistence.Configurations;

/// <summary>
/// Configuration for the Recruiter entity.
/// </summary>
public class RecruiterConfiguration : IEntityTypeConfiguration<Recruiter>
{
    public void Configure(EntityTypeBuilder<Recruiter> builder)
    {
        builder.HasKey(recruiter => new { recruiter.CompanyId, recruiter.UserId });

        builder.HasOne(recruiter => recruiter.Company)
            .WithMany()
            .HasForeignKey(recruiter => recruiter.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(recruiter => recruiter.User)
            .WithMany()
            .HasForeignKey(recruiter => recruiter.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

