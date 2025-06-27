using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CategoryDataIncluded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "category_levels",
                columns: new[] { "id", "category", "current_exp", "exp_to_next_level", "level", "needed_exp", "user_id" },
                values: new object[,]
                {
                    { 1, 2, 0, 100, 1, 100, 1 },
                    { 2, 3, 0, 100, 1, 100, 1 },
                    { 3, 1, 0, 100, 1, 100, 1 },
                    { 4, 0, 0, 100, 1, 100, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "category_levels",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "category_levels",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "category_levels",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "category_levels",
                keyColumn: "id",
                keyValue: 4);
        }
    }
}
