using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendDAL.Migrations
{
    /// <inheritdoc />
    public partial class DishPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Archivied",
                table: "Menus",
                newName: "Archived");

            migrationBuilder.AddColumn<int>(
                name: "DishPrice",
                table: "OrderedDishes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DishPrice",
                table: "OrderedDishes");

            migrationBuilder.RenameColumn(
                name: "Archived",
                table: "Menus",
                newName: "Archivied");
        }
    }
}
