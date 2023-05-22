using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendDAL.Migrations
{
    /// <inheritdoc />
    public partial class MenuCanBeArchived : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Archivied",
                table: "Menus",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Archivied",
                table: "Menus");
        }
    }
}
