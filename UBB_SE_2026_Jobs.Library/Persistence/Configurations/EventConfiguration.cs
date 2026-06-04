using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Persistence.Configurations;

/// <summary>
/// Configuration for the Event entity.
/// </summary>
public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(e => e.Id);

        builder.HasMany(e => e.Collaborators)
            .WithOne(c => c.Event)
            .HasForeignKey(c => c.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

