using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class someChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTime(2025, 8, 18, 11, 47, 36, 182, DateTimeKind.Utc).AddTicks(3886));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTime(2025, 8, 18, 10, 35, 31, 971, DateTimeKind.Utc).AddTicks(3428));
        }
    }
}
