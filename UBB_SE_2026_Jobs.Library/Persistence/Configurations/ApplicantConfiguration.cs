using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Persistence.Configurations;

/// <summary>
/// Configuration for the Applicant entity.
/// </summary>
public class ApplicantConfiguration : IEntityTypeConfiguration<Applicant>
{
    public void Configure(EntityTypeBuilder<Applicant> builder)
    {
        builder.HasKey(applicant => applicant.ApplicantId);

        builder.HasOne(applicant => applicant.RecommendedFromCompany)
            .WithMany()
            .HasForeignKey(applicant => applicant.RecommendedFromCompanyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

