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
        builder.HasKey(a => a.ApplicantId);

        builder.HasOne(a => a.RecommendedFromCompany)
            .WithMany()
            .HasForeignKey(a => a.RecommendedFromCompanyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

