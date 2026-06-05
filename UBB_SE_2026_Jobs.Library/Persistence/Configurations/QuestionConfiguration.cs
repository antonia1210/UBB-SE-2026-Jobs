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
            new TestQuestion { Id = 9, TestId = 3, QuestionText = "Which method converts a JSON string to an object?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "2", OptionsJson = "[\"JSON.stringify\",\"JSON.encode\",\"JSON.parse\",\"JSON.toObject\"]" },
            // Test 4 — Python Fundamentals
            new TestQuestion { Id = 10, TestId = 4, QuestionText = "Which keyword defines a function in Python?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "1", OptionsJson = "[\"func\",\"def\",\"function\",\"fn\"]" },
            new TestQuestion { Id = 11, TestId = 4, QuestionText = "What is the output of type([]) in Python?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "0", OptionsJson = "[\"<class 'list'>\",\"<class 'array'>\",\"<class 'tuple'>\",\"<class 'set'>\"]" },
            new TestQuestion { Id = 12, TestId = 4, QuestionText = "Which operator checks membership in a collection?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "2", OptionsJson = "[\"has\",\"contains\",\"in\",\"exists\"]" },
            // Test 5 — Java Fundamentals
            new TestQuestion { Id = 13, TestId = 5, QuestionText = "Which access modifier makes a member visible only within its class?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "3", OptionsJson = "[\"public\",\"protected\",\"package-private\",\"private\"]" },
            new TestQuestion { Id = 14, TestId = 5, QuestionText = "Which Java keyword prevents a method from being overridden?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "0", OptionsJson = "[\"final\",\"static\",\"abstract\",\"sealed\"]" },
            new TestQuestion { Id = 15, TestId = 5, QuestionText = "Which collection guarantees unique elements in Java?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "1", OptionsJson = "[\"ArrayList\",\"HashSet\",\"LinkedList\",\"Stack\"]" },
            // Test 6 — DevOps Basics
            new TestQuestion { Id = 16, TestId = 6, QuestionText = "Which tool is used for container orchestration at scale?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "2", OptionsJson = "[\"Docker\",\"Ansible\",\"Kubernetes\",\"Terraform\"]" },
            new TestQuestion { Id = 17, TestId = 6, QuestionText = "What does CI stand for in software delivery?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "0", OptionsJson = "[\"Continuous Integration\",\"Code Inspection\",\"Container Isolation\",\"Cloud Infrastructure\"]" },
            new TestQuestion { Id = 18, TestId = 6, QuestionText = "Which file format does Docker Compose use?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "1", OptionsJson = "[\"JSON\",\"YAML\",\"TOML\",\"XML\"]" },
            // Test 7 — Data Science Basics
            new TestQuestion { Id = 19, TestId = 7, QuestionText = "Which Python library is used for data manipulation with DataFrames?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "0", OptionsJson = "[\"pandas\",\"numpy\",\"scipy\",\"matplotlib\"]" },
            new TestQuestion { Id = 20, TestId = 7, QuestionText = "What does overfitting mean in machine learning?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "2", OptionsJson = "[\"Model is too simple\",\"Model performs poorly on training data\",\"Model memorises training data and fails on unseen data\",\"Model uses too little data\"]" },
            new TestQuestion { Id = 21, TestId = 7, QuestionText = "Which metric measures the proportion of true positives among predicted positives?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "1", OptionsJson = "[\"Recall\",\"Precision\",\"F1 Score\",\"Accuracy\"]" },
            // Test 8 — UI/UX Fundamentals
            new TestQuestion { Id = 22, TestId = 8, QuestionText = "What does UX stand for?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "0", OptionsJson = "[\"User Experience\",\"User Extension\",\"Uniform Exchange\",\"Usability Exploration\"]" },
            new TestQuestion { Id = 23, TestId = 8, QuestionText = "Which principle states that interfaces should provide feedback to user actions?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "2", OptionsJson = "[\"Affordance\",\"Consistency\",\"Feedback\",\"Simplicity\"]" },
            new TestQuestion { Id = 24, TestId = 8, QuestionText = "What is a wireframe in UI/UX design?", QuestionTypeString = "SINGLE_CHOICE", QuestionScore = 10f, QuestionAnswer = "1", OptionsJson = "[\"A fully styled mockup\",\"A low-fidelity structural blueprint of a UI\",\"A finished product prototype\",\"A user research report\"]" });
    }
}

