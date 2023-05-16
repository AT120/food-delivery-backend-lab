using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthDAL.Migrations
{
    /// <inheritdoc />
    public partial class RenameKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cooks_AspNetUsers_Id",
                table: "Cooks");

            migrationBuilder.DropForeignKey(
                name: "FK_Couriers_AspNetUsers_Id",
                table: "Couriers");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AspNetUsers_Id",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Managers_AspNetUsers_Id",
                table: "Managers");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Managers",
                newName: "BaseUserId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Customers",
                newName: "BaseUserId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Couriers",
                newName: "BaseUserId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Cooks",
                newName: "BaseUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cooks_AspNetUsers_BaseUserId",
                table: "Cooks",
                column: "BaseUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Couriers_AspNetUsers_BaseUserId",
                table: "Couriers",
                column: "BaseUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_AspNetUsers_BaseUserId",
                table: "Customers",
                column: "BaseUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Managers_AspNetUsers_BaseUserId",
                table: "Managers",
                column: "BaseUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cooks_AspNetUsers_BaseUserId",
                table: "Cooks");

            migrationBuilder.DropForeignKey(
                name: "FK_Couriers_AspNetUsers_BaseUserId",
                table: "Couriers");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AspNetUsers_BaseUserId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Managers_AspNetUsers_BaseUserId",
                table: "Managers");

            migrationBuilder.RenameColumn(
                name: "BaseUserId",
                table: "Managers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BaseUserId",
                table: "Customers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BaseUserId",
                table: "Couriers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BaseUserId",
                table: "Cooks",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cooks_AspNetUsers_Id",
                table: "Cooks",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Couriers_AspNetUsers_Id",
                table: "Couriers",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_AspNetUsers_Id",
                table: "Customers",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Managers_AspNetUsers_Id",
                table: "Managers",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
