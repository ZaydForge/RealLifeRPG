using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AchievementChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_unlocked",
                table: "titles");

            migrationBuilder.DropColumn(
                name: "is_unlocked",
                table: "achievements");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTime(2025, 8, 18, 10, 35, 31, 971, DateTimeKind.Utc).AddTicks(3428));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_unlocked",
                table: "titles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_unlocked",
                table: "achievements",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTime(2025, 8, 16, 11, 14, 46, 40, DateTimeKind.Utc).AddTicks(2209));
        }
    }
}
