using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FixShoppingCartRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_Products_ProductCode1",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCarts_ProductCode1",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "ProductCode1",
                table: "ShoppingCarts");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_ProductCode",
                table: "ShoppingCarts",
                column: "ProductCode");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_Products_ProductCode",
                table: "ShoppingCarts",
                column: "ProductCode",
                principalTable: "Products",
                principalColumn: "ProductCode",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_Products_ProductCode",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCarts_ProductCode",
                table: "ShoppingCarts");

            migrationBuilder.AddColumn<string>(
                name: "ProductCode1",
                table: "ShoppingCarts",
                type: "character varying(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_ProductCode1",
                table: "ShoppingCarts",
                column: "ProductCode1");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_Products_ProductCode1",
                table: "ShoppingCarts",
                column: "ProductCode1",
                principalTable: "Products",
                principalColumn: "ProductCode");
        }
    }
}
