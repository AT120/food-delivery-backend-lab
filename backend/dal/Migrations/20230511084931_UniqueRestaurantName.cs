using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendDAL.Migrations
{
    /// <inheritdoc />
    public partial class UniqueRestaurantName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_Name",
                table: "Restaurants",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Restaurants_Name",
                table: "Restaurants");
        }
    }
}
