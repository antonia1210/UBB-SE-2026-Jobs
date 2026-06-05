using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UBB_SE_2026_Jobs.Library.Migrations
{
    /// <inheritdoc />
    public partial class LinkTestsToSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "skill_id",
                table: "Tests",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 1,
                column: "skill_id",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 2,
                column: "skill_id",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 3,
                column: "skill_id",
                value: 37);

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 4,
                column: "skill_id",
                value: 8);

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 5,
                column: "skill_id",
                value: 21);

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 6,
                column: "skill_id",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 7,
                column: "skill_id",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "id",
                keyValue: 8,
                column: "skill_id",
                value: 13);

            migrationBuilder.CreateIndex(
                name: "IX_Tests_skill_id",
                table: "Tests",
                column: "skill_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Skills_skill_id",
                table: "Tests",
                column: "skill_id",
                principalTable: "Skills",
                principalColumn: "SkillId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Skills_skill_id",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Tests_skill_id",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "skill_id",
                table: "Tests");
        }
    }
}
