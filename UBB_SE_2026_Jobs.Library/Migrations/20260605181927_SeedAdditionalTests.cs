using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UBB_SE_2026_Jobs.Library.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdditionalTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tests",
                columns: new[] { "id", "category", "created_at", "title" },
                values: new object[,]
                {
                    { 4, "Programming", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Python Fundamentals" },
                    { 5, "Programming", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Java Fundamentals" },
                    { 6, "Operations", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "DevOps Basics" },
                    { 7, "Data Science", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Data Science Basics" },
                    { 8, "Design", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "UI/UX Fundamentals" }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "id", "options_json", "position_id", "question_answer", "question_score", "question_text", "question_type_string", "test_id" },
                values: new object[,]
                {
                    { 10, "[\"func\",\"def\",\"function\",\"fn\"]", null, "1", 10f, "Which keyword defines a function in Python?", "SINGLE_CHOICE", 4 },
                    { 11, "[\"<class 'list'>\",\"<class 'array'>\",\"<class 'tuple'>\",\"<class 'set'>\"]", null, "0", 10f, "What is the output of type([]) in Python?", "SINGLE_CHOICE", 4 },
                    { 12, "[\"has\",\"contains\",\"in\",\"exists\"]", null, "2", 10f, "Which operator checks membership in a collection?", "SINGLE_CHOICE", 4 },
                    { 13, "[\"public\",\"protected\",\"package-private\",\"private\"]", null, "3", 10f, "Which access modifier makes a member visible only within its class?", "SINGLE_CHOICE", 5 },
                    { 14, "[\"final\",\"static\",\"abstract\",\"sealed\"]", null, "0", 10f, "Which Java keyword prevents a method from being overridden?", "SINGLE_CHOICE", 5 },
                    { 15, "[\"ArrayList\",\"HashSet\",\"LinkedList\",\"Stack\"]", null, "1", 10f, "Which collection guarantees unique elements in Java?", "SINGLE_CHOICE", 5 },
                    { 16, "[\"Docker\",\"Ansible\",\"Kubernetes\",\"Terraform\"]", null, "2", 10f, "Which tool is used for container orchestration at scale?", "SINGLE_CHOICE", 6 },
                    { 17, "[\"Continuous Integration\",\"Code Inspection\",\"Container Isolation\",\"Cloud Infrastructure\"]", null, "0", 10f, "What does CI stand for in software delivery?", "SINGLE_CHOICE", 6 },
                    { 18, "[\"JSON\",\"YAML\",\"TOML\",\"XML\"]", null, "1", 10f, "Which file format does Docker Compose use?", "SINGLE_CHOICE", 6 },
                    { 19, "[\"pandas\",\"numpy\",\"scipy\",\"matplotlib\"]", null, "0", 10f, "Which Python library is used for data manipulation with DataFrames?", "SINGLE_CHOICE", 7 },
                    { 20, "[\"Model is too simple\",\"Model performs poorly on training data\",\"Model memorises training data and fails on unseen data\",\"Model uses too little data\"]", null, "2", 10f, "What does overfitting mean in machine learning?", "SINGLE_CHOICE", 7 },
                    { 21, "[\"Recall\",\"Precision\",\"F1 Score\",\"Accuracy\"]", null, "1", 10f, "Which metric measures the proportion of true positives among predicted positives?", "SINGLE_CHOICE", 7 },
                    { 22, "[\"User Experience\",\"User Extension\",\"Uniform Exchange\",\"Usability Exploration\"]", null, "0", 10f, "What does UX stand for?", "SINGLE_CHOICE", 8 },
                    { 23, "[\"Affordance\",\"Consistency\",\"Feedback\",\"Simplicity\"]", null, "2", 10f, "Which principle states that interfaces should provide feedback to user actions?", "SINGLE_CHOICE", 8 },
                    { 24, "[\"A fully styled mockup\",\"A low-fidelity structural blueprint of a UI\",\"A finished product prototype\",\"A user research report\"]", null, "1", 10f, "What is a wireframe in UI/UX design?", "SINGLE_CHOICE", 8 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 8);
        }
    }
}
