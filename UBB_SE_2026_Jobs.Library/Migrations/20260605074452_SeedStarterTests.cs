using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UBB_SE_2026_Jobs.Library.Migrations
{
    /// <inheritdoc />
    public partial class SeedStarterTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tests",
                columns: new[] { "id", "category", "created_at", "title" },
                values: new object[,]
                {
                    { 1, "Programming", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "C# Fundamentals" },
                    { 2, "Databases", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SQL Basics" },
                    { 3, "Web Development", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "JavaScript Essentials" }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "id", "options_json", "position_id", "question_answer", "question_score", "question_text", "question_type_string", "test_id" },
                values: new object[,]
                {
                    { 1, "[\"const\",\"static\",\"readonly\",\"var\"]", null, "0", 10f, "Which keyword declares a constant in C#?", "SINGLE_CHOICE", 1 },
                    { 2, "[\"System.Type\",\"System.Object\",\"System.Base\",\"System.Root\"]", null, "1", 10f, "What is the base class of all C# types?", "SINGLE_CHOICE", 1 },
                    { 3, "[\"->\",\"=>\",\":\",\"::\"]", null, "2", 10f, "Which symbol denotes inheritance in C#?", "SINGLE_CHOICE", 1 },
                    { 4, "[\"SELECT\",\"FETCH\",\"GET\",\"READ\"]", null, "0", 10f, "Which statement retrieves rows from a table?", "SINGLE_CHOICE", 2 },
                    { 5, "[\"ORDER BY\",\"WHERE\",\"GROUP BY\",\"LIMIT\"]", null, "1", 10f, "Which clause filters rows?", "SINGLE_CHOICE", 2 },
                    { 6, "[\"INNER JOIN\",\"LEFT JOIN\",\"RIGHT JOIN\",\"FULL JOIN\"]", null, "0", 10f, "Which JOIN returns only matching rows from both tables?", "SINGLE_CHOICE", 2 },
                    { 7, "[\"let\",\"var\",\"def\",\"dim\"]", null, "0", 10f, "Which keyword declares a block-scoped variable?", "SINGLE_CHOICE", 3 },
                    { 8, "[\"Only value\",\"Value and type\",\"Only type\",\"Reference only\"]", null, "1", 10f, "What does the === operator compare?", "SINGLE_CHOICE", 3 },
                    { 9, "[\"JSON.stringify\",\"JSON.encode\",\"JSON.parse\",\"JSON.toObject\"]", null, "2", 10f, "Which method converts a JSON string to an object?", "SINGLE_CHOICE", 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 3);
        }
    }
}
