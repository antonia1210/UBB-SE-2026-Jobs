using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UBB_SE_2026_Jobs.Library.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceCandidateTestsWithSkillGroupTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "options_json", "question_text" },
                values: new object[] { "[\"HTML\",\"C#\",\"React\",\"SQL\"]", "Which skill best fits the UI Markup group for Frontend Developer?" });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string" },
                values: new object[] { "[\"HTML\",\"CSS\",\"C#\",\"React\"]", "[0,1]", "Select the two skills that belong to UI Markup.", "MULTIPLE_CHOICE" });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string" },
                values: new object[] { null, "true", "HTML is part of the UI Markup skill group.", "TRUE_FALSE" });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { null, "HTML", "Type the primary skill tested for UI Markup.", "TEXT", 1 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { null, "UI Markup", "Type the exact skill group name for this Frontend Developer test.", "TEXT", 1 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "options_json", "question_text" },
                values: new object[] { "[\"JavaScript\",\"C#\",\"React\",\"SQL\"]", "Which skill best fits the JavaScript group for Frontend Developer?" });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"JavaScript\",\"TypeScript\",\"C#\",\"React\"]", "[0,1]", "Select the two skills that belong to JavaScript.", "MULTIPLE_CHOICE", 2 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 8,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { null, "true", "JavaScript is part of the JavaScript skill group.", "TRUE_FALSE", 2 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { null, "JavaScript", "Type the primary skill tested for JavaScript.", "TEXT", 2 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 10,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { null, "JavaScript", "Type the exact skill group name for this Frontend Developer test.", "TEXT", 2 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 11,
                columns: new[] { "options_json", "question_text", "test_id" },
                values: new object[] { "[\"React\",\"C#\",\"SQL\",\"Testing\"]", "Which skill best fits the Frontend Framework group for Frontend Developer?", 3 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 12,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"React\",\"Angular\",\"C#\",\"SQL\"]", "[0,1]", "Select the two skills that belong to Frontend Framework.", "MULTIPLE_CHOICE", 3 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 13,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { null, "true", "React is part of the Frontend Framework skill group.", "TRUE_FALSE", 3 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 14,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { null, "React", "Type the primary skill tested for Frontend Framework.", "TEXT", 3 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 15,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { null, "Frontend Framework", "Type the exact skill group name for this Frontend Developer test.", "TEXT", 3 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 16,
                columns: new[] { "options_json", "question_answer", "question_text", "test_id" },
                values: new object[] { "[\"Git\",\"C#\",\"React\",\"SQL\"]", "0", "Which skill best fits the Version Control group for Frontend Developer?", 4 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 17,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"Git\",\"GitHub\",\"C#\",\"React\"]", "[0,1]", "Select the two skills that belong to Version Control.", "MULTIPLE_CHOICE", 4 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 18,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { null, "true", "Git is part of the Version Control skill group.", "TRUE_FALSE", 4 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 19,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { null, "Git", "Type the primary skill tested for Version Control.", "TEXT", 4 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 20,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { null, "Version Control", "Type the exact skill group name for this Frontend Developer test.", "TEXT", 4 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 21,
                columns: new[] { "options_json", "question_answer", "question_text", "test_id" },
                values: new object[] { "[\"Jest\",\"C#\",\"React\",\"SQL\"]", "0", "Which skill best fits the Testing group for Frontend Developer?", 5 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 22,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"Jest\",\"Cypress\",\"C#\",\"React\"]", "[0,1]", "Select the two skills that belong to Testing.", "MULTIPLE_CHOICE", 5 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 23,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { null, "true", "Jest is part of the Testing skill group.", "TRUE_FALSE", 5 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 24,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { null, "Jest", "Type the primary skill tested for Testing.", "TEXT", 5 });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "id", "options_json", "position_id", "question_answer", "question_score", "question_text", "question_type_string", "test_id" },
                values: new object[,]
                {
                    { 25, null, null, "Testing", 10f, "Type the exact skill group name for this Frontend Developer test.", "TEXT", 5 },
                    { 26, "[\"Webpack\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Build Tools group for Frontend Developer?", "SINGLE_CHOICE", 6 },
                    { 27, "[\"Webpack\",\"Vite\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Build Tools.", "MULTIPLE_CHOICE", 6 },
                    { 28, null, null, "true", 10f, "Webpack is part of the Build Tools skill group.", "TRUE_FALSE", 6 },
                    { 29, null, null, "Webpack", 10f, "Type the primary skill tested for Build Tools.", "TEXT", 6 },
                    { 30, null, null, "Build Tools", 10f, "Type the exact skill group name for this Frontend Developer test.", "TEXT", 6 },
                    { 31, "[\"Figma\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Design Collaboration group for Frontend Developer?", "SINGLE_CHOICE", 7 },
                    { 32, "[\"Figma\",\"Adobe XD\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Design Collaboration.", "MULTIPLE_CHOICE", 7 },
                    { 33, null, null, "true", 10f, "Figma is part of the Design Collaboration skill group.", "TRUE_FALSE", 7 },
                    { 34, null, null, "Figma", 10f, "Type the primary skill tested for Design Collaboration.", "TEXT", 7 },
                    { 35, null, null, "Design Collaboration", 10f, "Type the exact skill group name for this Frontend Developer test.", "TEXT", 7 },
                    { 36, "[\"Java\",\"React\",\"SQL\",\"Testing\"]", null, "0", 10f, "Which skill best fits the Backend Language group for Backend Developer?", "SINGLE_CHOICE", 8 },
                    { 37, "[\"Java\",\"C#\",\"React\",\"SQL\"]", null, "[0,1]", 10f, "Select the two skills that belong to Backend Language.", "MULTIPLE_CHOICE", 8 },
                    { 38, null, null, "true", 10f, "Java is part of the Backend Language skill group.", "TRUE_FALSE", 8 },
                    { 39, null, null, "Java", 10f, "Type the primary skill tested for Backend Language.", "TEXT", 8 },
                    { 40, null, null, "Backend Language", 10f, "Type the exact skill group name for this Backend Developer test.", "TEXT", 8 }
                });

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "category", "skill_id", "title" },
                values: new object[] { "Frontend Developer", 33, "Frontend Developer - UI Markup" });

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "category", "skill_id", "title" },
                values: new object[] { "Frontend Developer", 37, "Frontend Developer - JavaScript" });

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "category", "skill_id", "title" },
                values: new object[] { "Frontend Developer", 2, "Frontend Developer - Frontend Framework" });

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "category", "skill_id", "title" },
                values: new object[] { "Frontend Developer", 39, "Frontend Developer - Version Control" });

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "category", "skill_id", "title" },
                values: new object[] { "Frontend Developer", 41, "Frontend Developer - Testing" });

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "category", "skill_id", "title" },
                values: new object[] { "Frontend Developer", 43, "Frontend Developer - Build Tools" });

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "category", "skill_id", "title" },
                values: new object[] { "Frontend Developer", 12, "Frontend Developer - Design Collaboration" });

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 8,
                columns: new[] { "category", "skill_id", "title" },
                values: new object[] { "Backend Developer", 21, "Backend Developer - Backend Language" });

            migrationBuilder.InsertData(
                table: "Tests",
                columns: new[] { "id", "category", "created_at", "skill_id", "title" },
                values: new object[,]
                {
                    { 9, "Backend Developer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 22, "Backend Developer - Web Framework" },
                    { 10, "Backend Developer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Backend Developer - Database Management" },
                    { 11, "Backend Developer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 54, "Backend Developer - API Design" },
                    { 12, "Backend Developer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 39, "Backend Developer - Version Control" },
                    { 13, "Backend Developer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 57, "Backend Developer - Testing" },
                    { 14, "UI/UX Designer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 12, "UI/UX Designer - Design Tools" },
                    { 15, "UI/UX Designer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 63, "UI/UX Designer - Prototyping" },
                    { 16, "UI/UX Designer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 66, "UI/UX Designer - User Research" },
                    { 17, "UI/UX Designer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 69, "UI/UX Designer - Visual Design" },
                    { 18, "UI/UX Designer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 47, "UI/UX Designer - Handoff" },
                    { 19, "UI/UX Designer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 73, "UI/UX Designer - Analytics" },
                    { 20, "DevOps Engineer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, "DevOps Engineer - Containerization" },
                    { 21, "DevOps Engineer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, "DevOps Engineer - Orchestration" },
                    { 22, "DevOps Engineer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 79, "DevOps Engineer - CI/CD" },
                    { 23, "DevOps Engineer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 32, "DevOps Engineer - Cloud Platform" },
                    { 24, "DevOps Engineer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 85, "DevOps Engineer - Infrastructure as Code" },
                    { 25, "DevOps Engineer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 88, "DevOps Engineer - Monitoring" },
                    { 26, "Project Manager", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 91, "Project Manager - Methodologies" },
                    { 27, "Project Manager", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 94, "Project Manager - Project Tools" },
                    { 28, "Project Manager", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 97, "Project Manager - Risk Management" },
                    { 29, "Project Manager", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 99, "Project Manager - Communication" },
                    { 30, "Project Manager", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 102, "Project Manager - Budgeting" },
                    { 31, "Data Analyst", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Data Analyst - Query Language" },
                    { 32, "Data Analyst", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 106, "Data Analyst - Data Visualization" },
                    { 33, "Data Analyst", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, "Data Analyst - Programming" },
                    { 34, "Data Analyst", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 110, "Data Analyst - Statistical Analysis" },
                    { 35, "Data Analyst", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 113, "Data Analyst - Spreadsheets" },
                    { 36, "Data Analyst", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, "Data Analyst - Data Cleaning" },
                    { 37, "Cybersecurity Specialist", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 116, "Cybersecurity Specialist - Network Security" },
                    { 38, "Cybersecurity Specialist", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 120, "Cybersecurity Specialist - Penetration Testing" },
                    { 39, "Cybersecurity Specialist", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 123, "Cybersecurity Specialist - SIEM & Monitoring" },
                    { 40, "Cybersecurity Specialist", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 126, "Cybersecurity Specialist - Cryptography" },
                    { 41, "Cybersecurity Specialist", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 130, "Cybersecurity Specialist - Compliance & Standards" },
                    { 42, "Cybersecurity Specialist", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 134, "Cybersecurity Specialist - Incident Response" },
                    { 43, "AI/ML Engineer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 137, "AI/ML Engineer - ML Frameworks" },
                    { 44, "AI/ML Engineer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, "AI/ML Engineer - Programming" },
                    { 45, "AI/ML Engineer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 141, "AI/ML Engineer - Mathematics" },
                    { 46, "AI/ML Engineer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, "AI/ML Engineer - Data Engineering" },
                    { 47, "AI/ML Engineer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, "AI/ML Engineer - Model Deployment" },
                    { 48, "AI/ML Engineer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 149, "AI/ML Engineer - NLP / Computer Vision" }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "id", "options_json", "position_id", "question_answer", "question_score", "question_text", "question_type_string", "test_id" },
                values: new object[,]
                {
                    { 41, "[\"Spring Boot\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Web Framework group for Backend Developer?", "SINGLE_CHOICE", 9 },
                    { 42, "[\"Spring Boot\",\"ASP.NET\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Web Framework.", "MULTIPLE_CHOICE", 9 },
                    { 43, null, null, "true", 10f, "Spring Boot is part of the Web Framework skill group.", "TRUE_FALSE", 9 },
                    { 44, null, null, "Spring Boot", 10f, "Type the primary skill tested for Web Framework.", "TEXT", 9 },
                    { 45, null, null, "Web Framework", 10f, "Type the exact skill group name for this Backend Developer test.", "TEXT", 9 },
                    { 46, "[\"SQL\",\"C#\",\"React\",\"Testing\"]", null, "0", 10f, "Which skill best fits the Database Management group for Backend Developer?", "SINGLE_CHOICE", 10 },
                    { 47, "[\"SQL\",\"PostgreSQL\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Database Management.", "MULTIPLE_CHOICE", 10 },
                    { 48, null, null, "true", 10f, "SQL is part of the Database Management skill group.", "TRUE_FALSE", 10 },
                    { 49, null, null, "SQL", 10f, "Type the primary skill tested for Database Management.", "TEXT", 10 },
                    { 50, null, null, "Database Management", 10f, "Type the exact skill group name for this Backend Developer test.", "TEXT", 10 },
                    { 51, "[\"REST\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the API Design group for Backend Developer?", "SINGLE_CHOICE", 11 },
                    { 52, "[\"REST\",\"GraphQL\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to API Design.", "MULTIPLE_CHOICE", 11 },
                    { 53, null, null, "true", 10f, "REST is part of the API Design skill group.", "TRUE_FALSE", 11 },
                    { 54, null, null, "REST", 10f, "Type the primary skill tested for API Design.", "TEXT", 11 },
                    { 55, null, null, "API Design", 10f, "Type the exact skill group name for this Backend Developer test.", "TEXT", 11 },
                    { 56, "[\"Git\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Version Control group for Backend Developer?", "SINGLE_CHOICE", 12 },
                    { 57, "[\"Git\",\"GitHub\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Version Control.", "MULTIPLE_CHOICE", 12 },
                    { 58, null, null, "true", 10f, "Git is part of the Version Control skill group.", "TRUE_FALSE", 12 },
                    { 59, null, null, "Git", 10f, "Type the primary skill tested for Version Control.", "TEXT", 12 },
                    { 60, null, null, "Version Control", 10f, "Type the exact skill group name for this Backend Developer test.", "TEXT", 12 },
                    { 61, "[\"JUnit\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Testing group for Backend Developer?", "SINGLE_CHOICE", 13 },
                    { 62, "[\"JUnit\",\"NUnit\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Testing.", "MULTIPLE_CHOICE", 13 },
                    { 63, null, null, "true", 10f, "JUnit is part of the Testing skill group.", "TRUE_FALSE", 13 },
                    { 64, null, null, "JUnit", 10f, "Type the primary skill tested for Testing.", "TEXT", 13 },
                    { 65, null, null, "Testing", 10f, "Type the exact skill group name for this Backend Developer test.", "TEXT", 13 },
                    { 66, "[\"Figma\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Design Tools group for UI/UX Designer?", "SINGLE_CHOICE", 14 },
                    { 67, "[\"Figma\",\"Adobe XD\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Design Tools.", "MULTIPLE_CHOICE", 14 },
                    { 68, null, null, "true", 10f, "Figma is part of the Design Tools skill group.", "TRUE_FALSE", 14 },
                    { 69, null, null, "Figma", 10f, "Type the primary skill tested for Design Tools.", "TEXT", 14 },
                    { 70, null, null, "Design Tools", 10f, "Type the exact skill group name for this UI/UX Designer test.", "TEXT", 14 },
                    { 71, "[\"Figma Prototyping\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Prototyping group for UI/UX Designer?", "SINGLE_CHOICE", 15 },
                    { 72, "[\"Figma Prototyping\",\"Marvel\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Prototyping.", "MULTIPLE_CHOICE", 15 },
                    { 73, null, null, "true", 10f, "Figma Prototyping is part of the Prototyping skill group.", "TRUE_FALSE", 15 },
                    { 74, null, null, "Figma Prototyping", 10f, "Type the primary skill tested for Prototyping.", "TEXT", 15 },
                    { 75, null, null, "Prototyping", 10f, "Type the exact skill group name for this UI/UX Designer test.", "TEXT", 15 },
                    { 76, "[\"Interviews\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the User Research group for UI/UX Designer?", "SINGLE_CHOICE", 16 },
                    { 77, "[\"Interviews\",\"Surveys\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to User Research.", "MULTIPLE_CHOICE", 16 },
                    { 78, null, null, "true", 10f, "Interviews is part of the User Research skill group.", "TRUE_FALSE", 16 },
                    { 79, null, null, "Interviews", 10f, "Type the primary skill tested for User Research.", "TEXT", 16 },
                    { 80, null, null, "User Research", 10f, "Type the exact skill group name for this UI/UX Designer test.", "TEXT", 16 },
                    { 81, "[\"Typography\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Visual Design group for UI/UX Designer?", "SINGLE_CHOICE", 17 },
                    { 82, "[\"Typography\",\"Color Theory\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Visual Design.", "MULTIPLE_CHOICE", 17 },
                    { 83, null, null, "true", 10f, "Typography is part of the Visual Design skill group.", "TRUE_FALSE", 17 },
                    { 84, null, null, "Typography", 10f, "Type the primary skill tested for Visual Design.", "TEXT", 17 },
                    { 85, null, null, "Visual Design", 10f, "Type the exact skill group name for this UI/UX Designer test.", "TEXT", 17 },
                    { 86, "[\"Zeplin\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Handoff group for UI/UX Designer?", "SINGLE_CHOICE", 18 },
                    { 87, "[\"Zeplin\",\"Figma\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Handoff.", "MULTIPLE_CHOICE", 18 },
                    { 88, null, null, "true", 10f, "Zeplin is part of the Handoff skill group.", "TRUE_FALSE", 18 },
                    { 89, null, null, "Zeplin", 10f, "Type the primary skill tested for Handoff.", "TEXT", 18 },
                    { 90, null, null, "Handoff", 10f, "Type the exact skill group name for this UI/UX Designer test.", "TEXT", 18 },
                    { 91, "[\"Google Analytics\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Analytics group for UI/UX Designer?", "SINGLE_CHOICE", 19 },
                    { 92, "[\"Google Analytics\",\"Hotjar\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Analytics.", "MULTIPLE_CHOICE", 19 },
                    { 93, null, null, "true", 10f, "Google Analytics is part of the Analytics skill group.", "TRUE_FALSE", 19 },
                    { 94, null, null, "Google Analytics", 10f, "Type the primary skill tested for Analytics.", "TEXT", 19 },
                    { 95, null, null, "Analytics", 10f, "Type the exact skill group name for this UI/UX Designer test.", "TEXT", 19 },
                    { 96, "[\"Docker\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Containerization group for DevOps Engineer?", "SINGLE_CHOICE", 20 },
                    { 97, "[\"Docker\",\"Podman\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Containerization.", "MULTIPLE_CHOICE", 20 },
                    { 98, null, null, "true", 10f, "Docker is part of the Containerization skill group.", "TRUE_FALSE", 20 },
                    { 99, null, null, "Docker", 10f, "Type the primary skill tested for Containerization.", "TEXT", 20 },
                    { 100, null, null, "Containerization", 10f, "Type the exact skill group name for this DevOps Engineer test.", "TEXT", 20 },
                    { 101, "[\"Kubernetes\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Orchestration group for DevOps Engineer?", "SINGLE_CHOICE", 21 },
                    { 102, "[\"Kubernetes\",\"Docker Swarm\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Orchestration.", "MULTIPLE_CHOICE", 21 },
                    { 103, null, null, "true", 10f, "Kubernetes is part of the Orchestration skill group.", "TRUE_FALSE", 21 },
                    { 104, null, null, "Kubernetes", 10f, "Type the primary skill tested for Orchestration.", "TEXT", 21 },
                    { 105, null, null, "Orchestration", 10f, "Type the exact skill group name for this DevOps Engineer test.", "TEXT", 21 },
                    { 106, "[\"Jenkins\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the CI/CD group for DevOps Engineer?", "SINGLE_CHOICE", 22 },
                    { 107, "[\"Jenkins\",\"GitHub Actions\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to CI/CD.", "MULTIPLE_CHOICE", 22 },
                    { 108, null, null, "true", 10f, "Jenkins is part of the CI/CD skill group.", "TRUE_FALSE", 22 },
                    { 109, null, null, "Jenkins", 10f, "Type the primary skill tested for CI/CD.", "TEXT", 22 },
                    { 110, null, null, "CI/CD", 10f, "Type the exact skill group name for this DevOps Engineer test.", "TEXT", 22 },
                    { 111, "[\"AWS\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Cloud Platform group for DevOps Engineer?", "SINGLE_CHOICE", 23 },
                    { 112, "[\"AWS\",\"Azure\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Cloud Platform.", "MULTIPLE_CHOICE", 23 },
                    { 113, null, null, "true", 10f, "AWS is part of the Cloud Platform skill group.", "TRUE_FALSE", 23 },
                    { 114, null, null, "AWS", 10f, "Type the primary skill tested for Cloud Platform.", "TEXT", 23 },
                    { 115, null, null, "Cloud Platform", 10f, "Type the exact skill group name for this DevOps Engineer test.", "TEXT", 23 },
                    { 116, "[\"Terraform\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Infrastructure as Code group for DevOps Engineer?", "SINGLE_CHOICE", 24 },
                    { 117, "[\"Terraform\",\"Ansible\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Infrastructure as Code.", "MULTIPLE_CHOICE", 24 },
                    { 118, null, null, "true", 10f, "Terraform is part of the Infrastructure as Code skill group.", "TRUE_FALSE", 24 },
                    { 119, null, null, "Terraform", 10f, "Type the primary skill tested for Infrastructure as Code.", "TEXT", 24 },
                    { 120, null, null, "Infrastructure as Code", 10f, "Type the exact skill group name for this DevOps Engineer test.", "TEXT", 24 },
                    { 121, "[\"Prometheus\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Monitoring group for DevOps Engineer?", "SINGLE_CHOICE", 25 },
                    { 122, "[\"Prometheus\",\"Grafana\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Monitoring.", "MULTIPLE_CHOICE", 25 },
                    { 123, null, null, "true", 10f, "Prometheus is part of the Monitoring skill group.", "TRUE_FALSE", 25 },
                    { 124, null, null, "Prometheus", 10f, "Type the primary skill tested for Monitoring.", "TEXT", 25 },
                    { 125, null, null, "Monitoring", 10f, "Type the exact skill group name for this DevOps Engineer test.", "TEXT", 25 },
                    { 126, "[\"Scrum\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Methodologies group for Project Manager?", "SINGLE_CHOICE", 26 },
                    { 127, "[\"Scrum\",\"Kanban\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Methodologies.", "MULTIPLE_CHOICE", 26 },
                    { 128, null, null, "true", 10f, "Scrum is part of the Methodologies skill group.", "TRUE_FALSE", 26 },
                    { 129, null, null, "Scrum", 10f, "Type the primary skill tested for Methodologies.", "TEXT", 26 },
                    { 130, null, null, "Methodologies", 10f, "Type the exact skill group name for this Project Manager test.", "TEXT", 26 },
                    { 131, "[\"Jira\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Project Tools group for Project Manager?", "SINGLE_CHOICE", 27 },
                    { 132, "[\"Jira\",\"Trello\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Project Tools.", "MULTIPLE_CHOICE", 27 },
                    { 133, null, null, "true", 10f, "Jira is part of the Project Tools skill group.", "TRUE_FALSE", 27 },
                    { 134, null, null, "Jira", 10f, "Type the primary skill tested for Project Tools.", "TEXT", 27 },
                    { 135, null, null, "Project Tools", 10f, "Type the exact skill group name for this Project Manager test.", "TEXT", 27 },
                    { 136, "[\"Risk Assessment\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Risk Management group for Project Manager?", "SINGLE_CHOICE", 28 },
                    { 137, "[\"Risk Assessment\",\"Mitigation Planning\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Risk Management.", "MULTIPLE_CHOICE", 28 },
                    { 138, null, null, "true", 10f, "Risk Assessment is part of the Risk Management skill group.", "TRUE_FALSE", 28 },
                    { 139, null, null, "Risk Assessment", 10f, "Type the primary skill tested for Risk Management.", "TEXT", 28 },
                    { 140, null, null, "Risk Management", 10f, "Type the exact skill group name for this Project Manager test.", "TEXT", 28 },
                    { 141, "[\"Stakeholder Management\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Communication group for Project Manager?", "SINGLE_CHOICE", 29 },
                    { 142, "[\"Stakeholder Management\",\"Reporting\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Communication.", "MULTIPLE_CHOICE", 29 },
                    { 143, null, null, "true", 10f, "Stakeholder Management is part of the Communication skill group.", "TRUE_FALSE", 29 },
                    { 144, null, null, "Stakeholder Management", 10f, "Type the primary skill tested for Communication.", "TEXT", 29 },
                    { 145, null, null, "Communication", 10f, "Type the exact skill group name for this Project Manager test.", "TEXT", 29 },
                    { 146, "[\"Cost Estimation\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Budgeting group for Project Manager?", "SINGLE_CHOICE", 30 },
                    { 147, "[\"Cost Estimation\",\"Budget Tracking\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Budgeting.", "MULTIPLE_CHOICE", 30 },
                    { 148, null, null, "true", 10f, "Cost Estimation is part of the Budgeting skill group.", "TRUE_FALSE", 30 },
                    { 149, null, null, "Cost Estimation", 10f, "Type the primary skill tested for Budgeting.", "TEXT", 30 },
                    { 150, null, null, "Budgeting", 10f, "Type the exact skill group name for this Project Manager test.", "TEXT", 30 },
                    { 151, "[\"SQL\",\"C#\",\"React\",\"Testing\"]", null, "0", 10f, "Which skill best fits the Query Language group for Data Analyst?", "SINGLE_CHOICE", 31 },
                    { 152, "[\"SQL\",\"PostgreSQL\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Query Language.", "MULTIPLE_CHOICE", 31 },
                    { 153, null, null, "true", 10f, "SQL is part of the Query Language skill group.", "TRUE_FALSE", 31 },
                    { 154, null, null, "SQL", 10f, "Type the primary skill tested for Query Language.", "TEXT", 31 },
                    { 155, null, null, "Query Language", 10f, "Type the exact skill group name for this Data Analyst test.", "TEXT", 31 },
                    { 156, "[\"Power BI\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Data Visualization group for Data Analyst?", "SINGLE_CHOICE", 32 },
                    { 157, "[\"Power BI\",\"Tableau\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Data Visualization.", "MULTIPLE_CHOICE", 32 },
                    { 158, null, null, "true", 10f, "Power BI is part of the Data Visualization skill group.", "TRUE_FALSE", 32 },
                    { 159, null, null, "Power BI", 10f, "Type the primary skill tested for Data Visualization.", "TEXT", 32 },
                    { 160, null, null, "Data Visualization", 10f, "Type the exact skill group name for this Data Analyst test.", "TEXT", 32 },
                    { 161, "[\"Python\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Programming group for Data Analyst?", "SINGLE_CHOICE", 33 },
                    { 162, "[\"Python\",\"R\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Programming.", "MULTIPLE_CHOICE", 33 },
                    { 163, null, null, "true", 10f, "Python is part of the Programming skill group.", "TRUE_FALSE", 33 },
                    { 164, null, null, "Python", 10f, "Type the primary skill tested for Programming.", "TEXT", 33 },
                    { 165, null, null, "Programming", 10f, "Type the exact skill group name for this Data Analyst test.", "TEXT", 33 },
                    { 166, "[\"Descriptive Statistics\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Statistical Analysis group for Data Analyst?", "SINGLE_CHOICE", 34 },
                    { 167, "[\"Descriptive Statistics\",\"Regression\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Statistical Analysis.", "MULTIPLE_CHOICE", 34 },
                    { 168, null, null, "true", 10f, "Descriptive Statistics is part of the Statistical Analysis skill group.", "TRUE_FALSE", 34 },
                    { 169, null, null, "Descriptive Statistics", 10f, "Type the primary skill tested for Statistical Analysis.", "TEXT", 34 },
                    { 170, null, null, "Statistical Analysis", 10f, "Type the exact skill group name for this Data Analyst test.", "TEXT", 34 },
                    { 171, "[\"Excel\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Spreadsheets group for Data Analyst?", "SINGLE_CHOICE", 35 },
                    { 172, "[\"Excel\",\"Google Sheets\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Spreadsheets.", "MULTIPLE_CHOICE", 35 },
                    { 173, null, null, "true", 10f, "Excel is part of the Spreadsheets skill group.", "TRUE_FALSE", 35 },
                    { 174, null, null, "Excel", 10f, "Type the primary skill tested for Spreadsheets.", "TEXT", 35 },
                    { 175, null, null, "Spreadsheets", 10f, "Type the exact skill group name for this Data Analyst test.", "TEXT", 35 },
                    { 176, "[\"Pandas\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Data Cleaning group for Data Analyst?", "SINGLE_CHOICE", 36 },
                    { 177, "[\"Pandas\",\"OpenRefine\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Data Cleaning.", "MULTIPLE_CHOICE", 36 },
                    { 178, null, null, "true", 10f, "Pandas is part of the Data Cleaning skill group.", "TRUE_FALSE", 36 },
                    { 179, null, null, "Pandas", 10f, "Type the primary skill tested for Data Cleaning.", "TEXT", 36 },
                    { 180, null, null, "Data Cleaning", 10f, "Type the exact skill group name for this Data Analyst test.", "TEXT", 36 },
                    { 181, "[\"Firewalls\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Network Security group for Cybersecurity Specialist?", "SINGLE_CHOICE", 37 },
                    { 182, "[\"Firewalls\",\"VPN\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Network Security.", "MULTIPLE_CHOICE", 37 },
                    { 183, null, null, "true", 10f, "Firewalls is part of the Network Security skill group.", "TRUE_FALSE", 37 },
                    { 184, null, null, "Firewalls", 10f, "Type the primary skill tested for Network Security.", "TEXT", 37 },
                    { 185, null, null, "Network Security", 10f, "Type the exact skill group name for this Cybersecurity Specialist test.", "TEXT", 37 },
                    { 186, "[\"Metasploit\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Penetration Testing group for Cybersecurity Specialist?", "SINGLE_CHOICE", 38 },
                    { 187, "[\"Metasploit\",\"Burp Suite\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Penetration Testing.", "MULTIPLE_CHOICE", 38 },
                    { 188, null, null, "true", 10f, "Metasploit is part of the Penetration Testing skill group.", "TRUE_FALSE", 38 },
                    { 189, null, null, "Metasploit", 10f, "Type the primary skill tested for Penetration Testing.", "TEXT", 38 },
                    { 190, null, null, "Penetration Testing", 10f, "Type the exact skill group name for this Cybersecurity Specialist test.", "TEXT", 38 },
                    { 191, "[\"Splunk\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the SIEM & Monitoring group for Cybersecurity Specialist?", "SINGLE_CHOICE", 39 },
                    { 192, "[\"Splunk\",\"IBM QRadar\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to SIEM & Monitoring.", "MULTIPLE_CHOICE", 39 },
                    { 193, null, null, "true", 10f, "Splunk is part of the SIEM & Monitoring skill group.", "TRUE_FALSE", 39 },
                    { 194, null, null, "Splunk", 10f, "Type the primary skill tested for SIEM & Monitoring.", "TEXT", 39 },
                    { 195, null, null, "SIEM & Monitoring", 10f, "Type the exact skill group name for this Cybersecurity Specialist test.", "TEXT", 39 },
                    { 196, "[\"AES\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Cryptography group for Cybersecurity Specialist?", "SINGLE_CHOICE", 40 },
                    { 197, "[\"AES\",\"RSA\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Cryptography.", "MULTIPLE_CHOICE", 40 },
                    { 198, null, null, "true", 10f, "AES is part of the Cryptography skill group.", "TRUE_FALSE", 40 },
                    { 199, null, null, "AES", 10f, "Type the primary skill tested for Cryptography.", "TEXT", 40 },
                    { 200, null, null, "Cryptography", 10f, "Type the exact skill group name for this Cybersecurity Specialist test.", "TEXT", 40 },
                    { 201, "[\"ISO 27001\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Compliance & Standards group for Cybersecurity Specialist?", "SINGLE_CHOICE", 41 },
                    { 202, "[\"ISO 27001\",\"GDPR\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Compliance & Standards.", "MULTIPLE_CHOICE", 41 },
                    { 203, null, null, "true", 10f, "ISO 27001 is part of the Compliance & Standards skill group.", "TRUE_FALSE", 41 },
                    { 204, null, null, "ISO 27001", 10f, "Type the primary skill tested for Compliance & Standards.", "TEXT", 41 },
                    { 205, null, null, "Compliance & Standards", 10f, "Type the exact skill group name for this Cybersecurity Specialist test.", "TEXT", 41 },
                    { 206, "[\"Forensics\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Incident Response group for Cybersecurity Specialist?", "SINGLE_CHOICE", 42 },
                    { 207, "[\"Forensics\",\"Malware Analysis\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Incident Response.", "MULTIPLE_CHOICE", 42 },
                    { 208, null, null, "true", 10f, "Forensics is part of the Incident Response skill group.", "TRUE_FALSE", 42 },
                    { 209, null, null, "Forensics", 10f, "Type the primary skill tested for Incident Response.", "TEXT", 42 },
                    { 210, null, null, "Incident Response", 10f, "Type the exact skill group name for this Cybersecurity Specialist test.", "TEXT", 42 },
                    { 211, "[\"TensorFlow\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the ML Frameworks group for AI/ML Engineer?", "SINGLE_CHOICE", 43 },
                    { 212, "[\"TensorFlow\",\"PyTorch\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to ML Frameworks.", "MULTIPLE_CHOICE", 43 },
                    { 213, null, null, "true", 10f, "TensorFlow is part of the ML Frameworks skill group.", "TRUE_FALSE", 43 },
                    { 214, null, null, "TensorFlow", 10f, "Type the primary skill tested for ML Frameworks.", "TEXT", 43 },
                    { 215, null, null, "ML Frameworks", 10f, "Type the exact skill group name for this AI/ML Engineer test.", "TEXT", 43 },
                    { 216, "[\"Python\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Programming group for AI/ML Engineer?", "SINGLE_CHOICE", 44 },
                    { 217, "[\"Python\",\"R\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Programming.", "MULTIPLE_CHOICE", 44 },
                    { 218, null, null, "true", 10f, "Python is part of the Programming skill group.", "TRUE_FALSE", 44 },
                    { 219, null, null, "Python", 10f, "Type the primary skill tested for Programming.", "TEXT", 44 },
                    { 220, null, null, "Programming", 10f, "Type the exact skill group name for this AI/ML Engineer test.", "TEXT", 44 },
                    { 221, "[\"Linear Algebra\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Mathematics group for AI/ML Engineer?", "SINGLE_CHOICE", 45 },
                    { 222, "[\"Linear Algebra\",\"Calculus\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Mathematics.", "MULTIPLE_CHOICE", 45 },
                    { 223, null, null, "true", 10f, "Linear Algebra is part of the Mathematics skill group.", "TRUE_FALSE", 45 },
                    { 224, null, null, "Linear Algebra", 10f, "Type the primary skill tested for Mathematics.", "TEXT", 45 },
                    { 225, null, null, "Mathematics", 10f, "Type the exact skill group name for this AI/ML Engineer test.", "TEXT", 45 },
                    { 226, "[\"Pandas\",\"C#\",\"React\",\"Testing\"]", null, "0", 10f, "Which skill best fits the Data Engineering group for AI/ML Engineer?", "SINGLE_CHOICE", 46 },
                    { 227, "[\"Pandas\",\"NumPy\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Data Engineering.", "MULTIPLE_CHOICE", 46 },
                    { 228, null, null, "true", 10f, "Pandas is part of the Data Engineering skill group.", "TRUE_FALSE", 46 },
                    { 229, null, null, "Pandas", 10f, "Type the primary skill tested for Data Engineering.", "TEXT", 46 },
                    { 230, null, null, "Data Engineering", 10f, "Type the exact skill group name for this AI/ML Engineer test.", "TEXT", 46 },
                    { 231, "[\"Docker\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the Model Deployment group for AI/ML Engineer?", "SINGLE_CHOICE", 47 },
                    { 232, "[\"Docker\",\"FastAPI\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to Model Deployment.", "MULTIPLE_CHOICE", 47 },
                    { 233, null, null, "true", 10f, "Docker is part of the Model Deployment skill group.", "TRUE_FALSE", 47 },
                    { 234, null, null, "Docker", 10f, "Type the primary skill tested for Model Deployment.", "TEXT", 47 },
                    { 235, null, null, "Model Deployment", 10f, "Type the exact skill group name for this AI/ML Engineer test.", "TEXT", 47 },
                    { 236, "[\"Hugging Face\",\"C#\",\"React\",\"SQL\"]", null, "0", 10f, "Which skill best fits the NLP / Computer Vision group for AI/ML Engineer?", "SINGLE_CHOICE", 48 },
                    { 237, "[\"Hugging Face\",\"OpenCV\",\"C#\",\"React\"]", null, "[0,1]", 10f, "Select the two skills that belong to NLP / Computer Vision.", "MULTIPLE_CHOICE", 48 },
                    { 238, null, null, "true", 10f, "Hugging Face is part of the NLP / Computer Vision skill group.", "TRUE_FALSE", 48 },
                    { 239, null, null, "Hugging Face", 10f, "Type the primary skill tested for NLP / Computer Vision.", "TEXT", 48 },
                    { 240, null, null, "NLP / Computer Vision", 10f, "Type the exact skill group name for this AI/ML Engineer test.", "TEXT", 48 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 116);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 117);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 118);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 119);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 120);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 121);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 125);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 126);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 127);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 128);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 129);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 130);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 131);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 132);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 133);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 134);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 135);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 136);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 137);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 138);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 139);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 140);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 141);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 142);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 143);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 144);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 145);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 146);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 147);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 148);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 149);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 150);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 151);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 152);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 153);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 154);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 155);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 156);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 157);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 158);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 159);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 160);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 161);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 162);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 163);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 164);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 165);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 166);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 167);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 168);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 169);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 170);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 171);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 172);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 173);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 174);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 175);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 176);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 177);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 178);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 179);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 180);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 181);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 182);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 183);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 184);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 185);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 186);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 187);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 188);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 189);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 190);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 191);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 192);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 193);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 194);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 195);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 196);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 197);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 198);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 199);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 200);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 201);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 202);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 203);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 204);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 205);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 206);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 207);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 208);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 209);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 210);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 211);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 212);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 213);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 214);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 215);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 216);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 217);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 218);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 219);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 220);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 221);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 222);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 223);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 224);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 225);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 226);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 227);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 228);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 229);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 230);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 231);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 232);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 233);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 234);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 235);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 236);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 237);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 238);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 239);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 240);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 48);

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "options_json", "question_text" },
                values: new object[] { "[\"const\",\"static\",\"readonly\",\"var\"]", "Which keyword declares a constant in C#?" });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string" },
                values: new object[] { "[\"System.Type\",\"System.Object\",\"System.Base\",\"System.Root\"]", "1", "What is the base class of all C# types?", "SINGLE_CHOICE" });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string" },
                values: new object[] { "[\"->\",\"=>\",\":\",\"::\"]", "2", "Which symbol denotes inheritance in C#?", "SINGLE_CHOICE" });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"SELECT\",\"FETCH\",\"GET\",\"READ\"]", "0", "Which statement retrieves rows from a table?", "SINGLE_CHOICE", 2 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"ORDER BY\",\"WHERE\",\"GROUP BY\",\"LIMIT\"]", "1", "Which clause filters rows?", "SINGLE_CHOICE", 2 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "options_json", "question_text" },
                values: new object[] { "[\"INNER JOIN\",\"LEFT JOIN\",\"RIGHT JOIN\",\"FULL JOIN\"]", "Which JOIN returns only matching rows from both tables?" });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"let\",\"var\",\"def\",\"dim\"]", "0", "Which keyword declares a block-scoped variable?", "SINGLE_CHOICE", 3 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 8,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"Only value\",\"Value and type\",\"Only type\",\"Reference only\"]", "1", "What does the === operator compare?", "SINGLE_CHOICE", 3 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"JSON.stringify\",\"JSON.encode\",\"JSON.parse\",\"JSON.toObject\"]", "2", "Which method converts a JSON string to an object?", "SINGLE_CHOICE", 3 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 10,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"func\",\"def\",\"function\",\"fn\"]", "1", "Which keyword defines a function in Python?", "SINGLE_CHOICE", 4 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 11,
                columns: new[] { "options_json", "question_text", "test_id" },
                values: new object[] { "[\"<class 'list'>\",\"<class 'array'>\",\"<class 'tuple'>\",\"<class 'set'>\"]", "What is the output of type([]) in Python?", 4 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 12,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"has\",\"contains\",\"in\",\"exists\"]", "2", "Which operator checks membership in a collection?", "SINGLE_CHOICE", 4 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 13,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"public\",\"protected\",\"package-private\",\"private\"]", "3", "Which access modifier makes a member visible only within its class?", "SINGLE_CHOICE", 5 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 14,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"final\",\"static\",\"abstract\",\"sealed\"]", "0", "Which Java keyword prevents a method from being overridden?", "SINGLE_CHOICE", 5 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 15,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"ArrayList\",\"HashSet\",\"LinkedList\",\"Stack\"]", "1", "Which collection guarantees unique elements in Java?", "SINGLE_CHOICE", 5 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 16,
                columns: new[] { "options_json", "question_answer", "question_text", "test_id" },
                values: new object[] { "[\"Docker\",\"Ansible\",\"Kubernetes\",\"Terraform\"]", "2", "Which tool is used for container orchestration at scale?", 6 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 17,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"Continuous Integration\",\"Code Inspection\",\"Container Isolation\",\"Cloud Infrastructure\"]", "0", "What does CI stand for in software delivery?", "SINGLE_CHOICE", 6 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 18,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"JSON\",\"YAML\",\"TOML\",\"XML\"]", "1", "Which file format does Docker Compose use?", "SINGLE_CHOICE", 6 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 19,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"pandas\",\"numpy\",\"scipy\",\"matplotlib\"]", "0", "Which Python library is used for data manipulation with DataFrames?", "SINGLE_CHOICE", 7 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 20,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"Model is too simple\",\"Model performs poorly on training data\",\"Model memorises training data and fails on unseen data\",\"Model uses too little data\"]", "2", "What does overfitting mean in machine learning?", "SINGLE_CHOICE", 7 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 21,
                columns: new[] { "options_json", "question_answer", "question_text", "test_id" },
                values: new object[] { "[\"Recall\",\"Precision\",\"F1 Score\",\"Accuracy\"]", "1", "Which metric measures the proportion of true positives among predicted positives?", 7 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 22,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"User Experience\",\"User Extension\",\"Uniform Exchange\",\"Usability Exploration\"]", "0", "What does UX stand for?", "SINGLE_CHOICE", 8 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 23,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"Affordance\",\"Consistency\",\"Feedback\",\"Simplicity\"]", "2", "Which principle states that interfaces should provide feedback to user actions?", "SINGLE_CHOICE", 8 });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "id",
                keyValue: 24,
                columns: new[] { "options_json", "question_answer", "question_text", "question_type_string", "test_id" },
                values: new object[] { "[\"A fully styled mockup\",\"A low-fidelity structural blueprint of a UI\",\"A finished product prototype\",\"A user research report\"]", "1", "What is a wireframe in UI/UX design?", "SINGLE_CHOICE", 8 });

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "category", "skill_id", "title" },
                values: new object[] { "Programming", 1, "C# Fundamentals" });

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "category", "skill_id", "title" },
                values: new object[] { "Databases", 3, "SQL Basics" });

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "category", "skill_id", "title" },
                values: new object[] { "Web Development", 37, "JavaScript Essentials" });

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "category", "skill_id", "title" },
                values: new object[] { "Programming", 8, "Python Fundamentals" });

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "category", "skill_id", "title" },
                values: new object[] { "Programming", 21, "Java Fundamentals" });

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "category", "skill_id", "title" },
                values: new object[] { "Operations", 6, "DevOps Basics" });

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "category", "skill_id", "title" },
                values: new object[] { "Data Science", 10, "Data Science Basics" });

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 8,
                columns: new[] { "category", "skill_id", "title" },
                values: new object[] { "Design", 13, "UI/UX Fundamentals" });
        }
    }
}
