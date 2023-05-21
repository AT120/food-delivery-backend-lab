using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendDAL.Migrations
{
    /// <inheritdoc />
    public partial class ManyMenusPerDish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_Menus_MenuId",
                table: "Dishes");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_MenuId",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "MenuId",
                table: "Dishes");

            migrationBuilder.CreateTable(
                name: "DishMenu",
                columns: table => new
                {
                    DishesId = table.Column<Guid>(type: "uuid", nullable: false),
                    MenusId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishMenu", x => new { x.DishesId, x.MenusId });
                    table.ForeignKey(
                        name: "FK_DishMenu_Dishes_DishesId",
                        column: x => x.DishesId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishMenu_Menus_MenusId",
                        column: x => x.MenusId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DishMenu_MenusId",
                table: "DishMenu",
                column: "MenusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DishMenu");

            migrationBuilder.AddColumn<int>(
                name: "MenuId",
                table: "Dishes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_MenuId",
                table: "Dishes",
                column: "MenuId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_Menus_MenuId",
                table: "Dishes",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
