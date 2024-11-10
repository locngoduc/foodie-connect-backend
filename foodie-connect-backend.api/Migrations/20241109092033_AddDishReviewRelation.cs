using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace foodie_connect_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddDishReviewRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_UserId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Dishes_DishId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Restaurants_RestaurantId",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews");

            migrationBuilder.RenameTable(
                name: "Reviews",
                newName: "DishReview");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_UserId",
                table: "DishReview",
                newName: "IX_DishReview_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_RestaurantId",
                table: "DishReview",
                newName: "IX_DishReview_RestaurantId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_DishId",
                table: "DishReview",
                newName: "IX_DishReview_DishId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DishReview",
                table: "DishReview",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DishReview_AspNetUsers_UserId",
                table: "DishReview",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DishReview_Dishes_DishId",
                table: "DishReview",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DishReview_Restaurants_RestaurantId",
                table: "DishReview",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishReview_AspNetUsers_UserId",
                table: "DishReview");

            migrationBuilder.DropForeignKey(
                name: "FK_DishReview_Dishes_DishId",
                table: "DishReview");

            migrationBuilder.DropForeignKey(
                name: "FK_DishReview_Restaurants_RestaurantId",
                table: "DishReview");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DishReview",
                table: "DishReview");

            migrationBuilder.RenameTable(
                name: "DishReview",
                newName: "Reviews");

            migrationBuilder.RenameIndex(
                name: "IX_DishReview_UserId",
                table: "Reviews",
                newName: "IX_Reviews_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DishReview_RestaurantId",
                table: "Reviews",
                newName: "IX_Reviews_RestaurantId");

            migrationBuilder.RenameIndex(
                name: "IX_DishReview_DishId",
                table: "Reviews",
                newName: "IX_Reviews_DishId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Dishes_DishId",
                table: "Reviews",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Restaurants_RestaurantId",
                table: "Reviews",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id");
        }
    }
}
