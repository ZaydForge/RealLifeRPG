using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OtpEnhancemenents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_ot_ps_user_ot_ps_user_ot_ps_id",
                table: "user_ot_ps");

            migrationBuilder.DropIndex(
                name: "ix_user_ot_ps_user_ot_ps_id",
                table: "user_ot_ps");

            migrationBuilder.DropColumn(
                name: "user_ot_ps_id",
                table: "user_ot_ps");

            migrationBuilder.AddColumn<int>(
                name: "attempt_count",
                table: "user_ot_ps",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "is_used",
                table: "user_ot_ps",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_attempt_at",
                table: "user_ot_ps",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "purpose",
                table: "user_ot_ps",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTime(2025, 8, 20, 6, 31, 32, 995, DateTimeKind.Utc).AddTicks(1302));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "attempt_count",
                table: "user_ot_ps");

            migrationBuilder.DropColumn(
                name: "is_used",
                table: "user_ot_ps");

            migrationBuilder.DropColumn(
                name: "last_attempt_at",
                table: "user_ot_ps");

            migrationBuilder.DropColumn(
                name: "purpose",
                table: "user_ot_ps");

            migrationBuilder.AddColumn<int>(
                name: "user_ot_ps_id",
                table: "user_ot_ps",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTime(2025, 8, 18, 11, 47, 36, 182, DateTimeKind.Utc).AddTicks(3886));

            migrationBuilder.CreateIndex(
                name: "ix_user_ot_ps_user_ot_ps_id",
                table: "user_ot_ps",
                column: "user_ot_ps_id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_ot_ps_user_ot_ps_user_ot_ps_id",
                table: "user_ot_ps",
                column: "user_ot_ps_id",
                principalTable: "user_ot_ps",
                principalColumn: "id");
        }
    }
}
