using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace foodie_connect_backend.Migrations
{
    /// <inheritdoc />
    public partial class RemovedDishCategoryIdinfavorofcompositekey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishDishCategory_DishCategories_CategoriesDishCategoryId",
                table: "DishDishCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DishDishCategory",
                table: "DishDishCategory");

            migrationBuilder.DropIndex(
                name: "IX_DishDishCategory_DishesId",
                table: "DishDishCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DishCategories",
                table: "DishCategories");

            migrationBuilder.DropIndex(
                name: "IX_DishCategories_RestaurantId",
                table: "DishCategories");

            migrationBuilder.DropColumn(
                name: "DishCategoryId",
                table: "DishCategories");

            migrationBuilder.RenameColumn(
                name: "CategoriesDishCategoryId",
                table: "DishDishCategory",
                newName: "CategoriesRestaurantId");

            migrationBuilder.AddColumn<string>(
                name: "CategoriesCategoryName",
                table: "DishDishCategory",
                type: "character varying(32)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Areas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 11, 7, 7, 44, 54, 404, DateTimeKind.Utc).AddTicks(9421),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 11, 7, 7, 37, 19, 507, DateTimeKind.Utc).AddTicks(85));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Areas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 11, 7, 7, 44, 54, 404, DateTimeKind.Utc).AddTicks(8973),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 11, 7, 7, 37, 19, 506, DateTimeKind.Utc).AddTicks(9660));

            migrationBuilder.AddPrimaryKey(
                name: "PK_DishDishCategory",
                table: "DishDishCategory",
                columns: new[] { "DishesId", "CategoriesRestaurantId", "CategoriesCategoryName" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DishCategories",
                table: "DishCategories",
                columns: new[] { "RestaurantId", "CategoryName" });

            migrationBuilder.CreateIndex(
                name: "IX_DishDishCategory_CategoriesRestaurantId_CategoriesCategoryN~",
                table: "DishDishCategory",
                columns: new[] { "CategoriesRestaurantId", "CategoriesCategoryName" });

            migrationBuilder.AddForeignKey(
                name: "FK_DishDishCategory_DishCategories_CategoriesRestaurantId_Cate~",
                table: "DishDishCategory",
                columns: new[] { "CategoriesRestaurantId", "CategoriesCategoryName" },
                principalTable: "DishCategories",
                principalColumns: new[] { "RestaurantId", "CategoryName" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishDishCategory_DishCategories_CategoriesRestaurantId_Cate~",
                table: "DishDishCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DishDishCategory",
                table: "DishDishCategory");

            migrationBuilder.DropIndex(
                name: "IX_DishDishCategory_CategoriesRestaurantId_CategoriesCategoryN~",
                table: "DishDishCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DishCategories",
                table: "DishCategories");

            migrationBuilder.DropColumn(
                name: "CategoriesCategoryName",
                table: "DishDishCategory");

            migrationBuilder.RenameColumn(
                name: "CategoriesRestaurantId",
                table: "DishDishCategory",
                newName: "CategoriesDishCategoryId");

            migrationBuilder.AddColumn<string>(
                name: "DishCategoryId",
                table: "DishCategories",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Areas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 11, 7, 7, 37, 19, 507, DateTimeKind.Utc).AddTicks(85),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 11, 7, 7, 44, 54, 404, DateTimeKind.Utc).AddTicks(9421));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Areas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 11, 7, 7, 37, 19, 506, DateTimeKind.Utc).AddTicks(9660),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 11, 7, 7, 44, 54, 404, DateTimeKind.Utc).AddTicks(8973));

            migrationBuilder.AddPrimaryKey(
                name: "PK_DishDishCategory",
                table: "DishDishCategory",
                columns: new[] { "CategoriesDishCategoryId", "DishesId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DishCategories",
                table: "DishCategories",
                column: "DishCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DishDishCategory_DishesId",
                table: "DishDishCategory",
                column: "DishesId");

            migrationBuilder.CreateIndex(
                name: "IX_DishCategories_RestaurantId",
                table: "DishCategories",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_DishDishCategory_DishCategories_CategoriesDishCategoryId",
                table: "DishDishCategory",
                column: "CategoriesDishCategoryId",
                principalTable: "DishCategories",
                principalColumn: "DishCategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
