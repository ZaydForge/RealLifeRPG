using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DescriptionIsNUll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTime(2025, 8, 10, 14, 17, 46, 476, DateTimeKind.Utc).AddTicks(9176));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTime(2025, 8, 3, 4, 47, 11, 637, DateTimeKind.Utc).AddTicks(8383));
        }
    }
}
