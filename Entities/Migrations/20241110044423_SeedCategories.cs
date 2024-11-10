using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryID", "Name" },
                values: new object[,]
                {
            { 1, "Electronics" },
            { 2, "Clothing" },
            { 3, "Groceries" },
            { 4, "Cell Phones" },
            { 5, "Furnitures" },
            { 6, "Gadgets" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6 });
        }
    }
}
