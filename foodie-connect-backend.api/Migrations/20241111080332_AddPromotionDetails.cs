using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace foodie_connect_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotionDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromotionDetail_Dishes_DishId",
                table: "PromotionDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_PromotionDetail_Promotions_PromotionId",
                table: "PromotionDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PromotionDetail",
                table: "PromotionDetail");

            migrationBuilder.RenameTable(
                name: "PromotionDetail",
                newName: "PromotionDetails");

            migrationBuilder.RenameIndex(
                name: "IX_PromotionDetail_DishId",
                table: "PromotionDetails",
                newName: "IX_PromotionDetails_DishId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PromotionDetails",
                table: "PromotionDetails",
                columns: new[] { "PromotionId", "DishId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionDetails_Dishes_DishId",
                table: "PromotionDetails",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionDetails_Promotions_PromotionId",
                table: "PromotionDetails",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromotionDetails_Dishes_DishId",
                table: "PromotionDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PromotionDetails_Promotions_PromotionId",
                table: "PromotionDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PromotionDetails",
                table: "PromotionDetails");

            migrationBuilder.RenameTable(
                name: "PromotionDetails",
                newName: "PromotionDetail");

            migrationBuilder.RenameIndex(
                name: "IX_PromotionDetails_DishId",
                table: "PromotionDetail",
                newName: "IX_PromotionDetail_DishId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PromotionDetail",
                table: "PromotionDetail",
                columns: new[] { "PromotionId", "DishId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionDetail_Dishes_DishId",
                table: "PromotionDetail",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionDetail_Promotions_PromotionId",
                table: "PromotionDetail",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
