using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Persistence.Configurations;

/// <summary>
/// Configuration for the Collaborator entity (many-to-many relationship between events and companies).
/// </summary>
public class CollaboratorConfiguration : IEntityTypeConfiguration<Collaborator>
{
    public void Configure(EntityTypeBuilder<Collaborator> builder)
    {
        builder.HasKey(c => new { c.EventId, c.CompanyId });

        builder.HasOne(c => c.Event)
            .WithMany(e => e.Collaborators)
            .HasForeignKey(c => c.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Company)
            .WithMany(co => co.Collaborators)
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

