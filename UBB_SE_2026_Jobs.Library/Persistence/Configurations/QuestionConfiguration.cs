using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UBB_SE_2026_Jobs.Library.Domain.Core;

namespace UBB_SE_2026_Jobs.Library.Persistence.Configurations;

/// <summary>
/// Configuration for the TestQuestion entity.
/// </summary>
public class QuestionConfiguration : IEntityTypeConfiguration<TestQuestion>
    {
        public void Configure(EntityTypeBuilder<TestQuestion> builder)
        {
            builder.HasKey(q => q.Id);

        builder.HasOne(q => q.Test)
            .WithMany(t => t.Questions)
            .HasForeignKey(q => q.TestId)
            .OnDelete(DeleteBehavior.SetNull);

        // Seed questions for the starter tests defined in TestConfiguration.
        // SINGLE_CHOICE answers are the zero-based index of the correct option in OptionsJson.
        builder.HasData(
            // Test 1 — C# Fundamentals
            new TestQuestion { Id = 1, TestId = 1, QuestionText = "Which keyword declares a constant in C#?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "0", OptionsJson = "[\"const\",\"static\",\"readonly\",\"var\"]" },
            new TestQuestion { Id = 2, TestId = 1, QuestionText = "What is the base class of all C# types?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "1", OptionsJson = "[\"System.Type\",\"System.Object\",\"System.Base\",\"System.Root\"]" },
            new TestQuestion { Id = 3, TestId = 1, QuestionText = "Which symbol denotes inheritance in C#?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "2", OptionsJson = "[\"->\",\"=>\",\":\",\"::\"]" },
            // Test 2 — SQL Basics
            new TestQuestion { Id = 4, TestId = 2, QuestionText = "Which statement retrieves rows from a table?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "0", OptionsJson = "[\"SELECT\",\"FETCH\",\"GET\",\"READ\"]" },
            new TestQuestion { Id = 5, TestId = 2, QuestionText = "Which clause filters rows?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "1", OptionsJson = "[\"ORDER BY\",\"WHERE\",\"GROUP BY\",\"LIMIT\"]" },
            new TestQuestion { Id = 6, TestId = 2, QuestionText = "Which JOIN returns only matching rows from both tables?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "0", OptionsJson = "[\"INNER JOIN\",\"LEFT JOIN\",\"RIGHT JOIN\",\"FULL JOIN\"]" },
            // Test 3 — JavaScript Essentials
            new TestQuestion { Id = 7, TestId = 3, QuestionText = "Which keyword declares a block-scoped variable?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "0", OptionsJson = "[\"let\",\"var\",\"def\",\"dim\"]" },
            new TestQuestion { Id = 8, TestId = 3, QuestionText = "What does the === operator compare?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "1", OptionsJson = "[\"Only value\",\"Value and type\",\"Only type\",\"Reference only\"]" },
            new TestQuestion { Id = 9, TestId = 3, QuestionText = "Which method converts a JSON string to an object?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "2", OptionsJson = "[\"JSON.stringify\",\"JSON.encode\",\"JSON.parse\",\"JSON.toObject\"]" });
    }
}

