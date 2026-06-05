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

        // Seed one test per job role category so every candidate type has relevant content.
        // Questions are seeded in QuestionConfiguration.
        builder.HasData(
            new Test { Id = 1, Title = "C# Fundamentals",       Category = "Programming",    CreatedAt = new DateTime(2026, 1, 1) },
            new Test { Id = 2, Title = "SQL Basics",            Category = "Databases",      CreatedAt = new DateTime(2026, 1, 1) },
            new Test { Id = 3, Title = "JavaScript Essentials", Category = "Web Development", CreatedAt = new DateTime(2026, 1, 1) },
            new Test { Id = 4, Title = "Python Fundamentals",   Category = "Programming",    CreatedAt = new DateTime(2026, 1, 1) },
            new Test { Id = 5, Title = "Java Fundamentals",     Category = "Programming",    CreatedAt = new DateTime(2026, 1, 1) },
            new Test { Id = 6, Title = "DevOps Basics",         Category = "Operations",     CreatedAt = new DateTime(2026, 1, 1) },
            new Test { Id = 7, Title = "Data Science Basics",   Category = "Data Science",   CreatedAt = new DateTime(2026, 1, 1) },
            new Test { Id = 8, Title = "UI/UX Fundamentals",    Category = "Design",         CreatedAt = new DateTime(2026, 1, 1) });
    }
}

