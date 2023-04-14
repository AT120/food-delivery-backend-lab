using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendDAL.Migrations
{
    /// <inheritdoc />
    public partial class FixTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mangers_Restaurants_RestaurantId",
                table: "Mangers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mangers",
                table: "Mangers");

            migrationBuilder.RenameTable(
                name: "Mangers",
                newName: "Managers");

            migrationBuilder.RenameIndex(
                name: "IX_Mangers_RestaurantId",
                table: "Managers",
                newName: "IX_Managers_RestaurantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Managers",
                table: "Managers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Managers_Restaurants_RestaurantId",
                table: "Managers",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Managers_Restaurants_RestaurantId",
                table: "Managers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Managers",
                table: "Managers");

            migrationBuilder.RenameTable(
                name: "Managers",
                newName: "Mangers");

            migrationBuilder.RenameIndex(
                name: "IX_Managers_RestaurantId",
                table: "Mangers",
                newName: "IX_Mangers_RestaurantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mangers",
                table: "Mangers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Mangers_Restaurants_RestaurantId",
                table: "Mangers",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
