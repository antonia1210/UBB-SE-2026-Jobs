using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UBB_SE_2026_Jobs.Library.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserIdAliasColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Id",
                value: 1);
        }
    }
}
