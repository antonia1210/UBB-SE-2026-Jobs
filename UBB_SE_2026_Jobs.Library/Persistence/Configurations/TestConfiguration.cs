using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UBB_SE_2026_Jobs.Library.Domain.Core;

namespace UBB_SE_2026_Jobs.Library.Persistence.Configurations;

/// <summary>
/// Configuration for the Test entity.
/// </summary>
public class TestConfiguration : IEntityTypeConfiguration<Test>
    {
        public void Configure(EntityTypeBuilder<Test> builder)
        {
            builder.HasKey(t => t.Id);

        builder.HasMany(t => t.Questions)
            .WithOne(q => q.Test)
            .HasForeignKey(q => q.TestId)
            .OnDelete(DeleteBehavior.SetNull);

        // Seed starter tests so candidates have something to take on a fresh database.
        // Their questions are seeded in QuestionConfiguration.
        builder.HasData(
            new Test { Id = 1, Title = "C# Fundamentals", Category = "Programming", CreatedAt = new DateTime(2026, 1, 1) },
            new Test { Id = 2, Title = "SQL Basics", Category = "Databases", CreatedAt = new DateTime(2026, 1, 1) },
            new Test { Id = 3, Title = "JavaScript Essentials", Category = "Web Development", CreatedAt = new DateTime(2026, 1, 1) });
    }
}

