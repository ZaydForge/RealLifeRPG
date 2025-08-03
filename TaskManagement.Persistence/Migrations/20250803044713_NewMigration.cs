using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TaskManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_category_levels_user_profiles_user_id",
                table: "category_levels");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropIndex(
                name: "ix_category_levels_user_id",
                table: "category_levels");

            migrationBuilder.DropColumn(
                name: "category",
                table: "category_levels");

            migrationBuilder.DropColumn(
                name: "current_exp",
                table: "category_levels");

            migrationBuilder.DropColumn(
                name: "exp_to_next_level",
                table: "category_levels");

            migrationBuilder.DropColumn(
                name: "level",
                table: "category_levels");

            migrationBuilder.DropColumn(
                name: "needed_exp",
                table: "category_levels");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "category_levels",
                newName: "category_name");

            migrationBuilder.CreateTable(
                name: "user_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    current_exp = table.Column<int>(type: "integer", nullable: false),
                    exp_to_next_level = table.Column<int>(type: "integer", nullable: false),
                    needed_exp = table.Column<int>(type: "integer", nullable: false),
                    last_level_up = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_categories_category_levels_category_id",
                        column: x => x.category_id,
                        principalTable: "category_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_categories_user_profiles_user_id",
                        column: x => x.user_id,
                        principalTable: "user_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "category_levels",
                keyColumn: "id",
                keyValue: 1,
                column: "category_name",
                value: 2);

            migrationBuilder.UpdateData(
                table: "category_levels",
                keyColumn: "id",
                keyValue: 2,
                column: "category_name",
                value: 3);

            migrationBuilder.UpdateData(
                table: "category_levels",
                keyColumn: "id",
                keyValue: 4,
                column: "category_name",
                value: 0);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTime(2025, 8, 3, 4, 47, 11, 637, DateTimeKind.Utc).AddTicks(8383));

            migrationBuilder.CreateIndex(
                name: "ix_user_categories_category_id",
                table: "user_categories",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_categories_user_id",
                table: "user_categories",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_categories");

            migrationBuilder.RenameColumn(
                name: "category_name",
                table: "category_levels",
                newName: "user_id");

            migrationBuilder.AddColumn<int>(
                name: "category",
                table: "category_levels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "current_exp",
                table: "category_levels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "exp_to_next_level",
                table: "category_levels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "level",
                table: "category_levels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "needed_exp",
                table: "category_levels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    product_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orders", x => x.id);
                });

            migrationBuilder.UpdateData(
                table: "category_levels",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "category", "current_exp", "exp_to_next_level", "level", "needed_exp", "user_id" },
                values: new object[] { 2, 0, 100, 1, 100, 1 });

            migrationBuilder.UpdateData(
                table: "category_levels",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "category", "current_exp", "exp_to_next_level", "level", "needed_exp", "user_id" },
                values: new object[] { 3, 0, 100, 1, 100, 1 });

            migrationBuilder.UpdateData(
                table: "category_levels",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "category", "current_exp", "exp_to_next_level", "level", "needed_exp" },
                values: new object[] { 1, 0, 100, 1, 100 });

            migrationBuilder.UpdateData(
                table: "category_levels",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "category", "current_exp", "exp_to_next_level", "level", "needed_exp", "user_id" },
                values: new object[] { 0, 0, 100, 1, 100, 1 });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1,
                column: "updated_at",
                value: new DateTime(2025, 8, 2, 10, 55, 53, 577, DateTimeKind.Utc).AddTicks(2779));

            migrationBuilder.CreateIndex(
                name: "ix_category_levels_user_id",
                table: "category_levels",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_category_levels_user_profiles_user_id",
                table: "category_levels",
                column: "user_id",
                principalTable: "user_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
