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
        builder.HasKey(collaborator => new { collaborator.EventId, collaborator.CompanyId });

        builder.HasOne(collaborator => collaborator.Event)
            .WithMany(eventEntity => eventEntity.Collaborators)
            .HasForeignKey(collaborator => collaborator.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(collaborator => collaborator.Company)
            .WithMany(company => company.Collaborators)
            .HasForeignKey(collaborator => collaborator.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

