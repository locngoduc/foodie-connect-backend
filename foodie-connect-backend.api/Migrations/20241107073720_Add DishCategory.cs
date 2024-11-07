using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace foodie_connect_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddDishCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DishesCategories");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.RenameColumn(
                name: "Platform",
                table: "SocialLinks",
                newName: "PlatformType");

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "SocialLinks",
                type: "character varying(64)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "Services",
                type: "character varying(64)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "Reviews",
                type: "character varying(64)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DishId",
                table: "Reviews",
                type: "character varying(64)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HeadId",
                table: "Restaurants",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "AreaId",
                table: "Restaurants",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Restaurants",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "Promotions",
                type: "character varying(64)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DishId",
                table: "Promotions",
                type: "character varying(64)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "Dishes",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Dishes",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Dishes",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Areas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 11, 7, 7, 37, 19, 507, DateTimeKind.Utc).AddTicks(85),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 11, 3, 4, 48, 34, 958, DateTimeKind.Utc).AddTicks(7448));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Areas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 11, 7, 7, 37, 19, 506, DateTimeKind.Utc).AddTicks(9660),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 11, 3, 4, 48, 34, 958, DateTimeKind.Utc).AddTicks(7059));

            migrationBuilder.CreateTable(
                name: "DishCategories",
                columns: table => new
                {
                    DishCategoryId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    RestaurantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CategoryName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishCategories", x => x.DishCategoryId);
                    table.ForeignKey(
                        name: "FK_DishCategories_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DishDishCategory",
                columns: table => new
                {
                    CategoriesDishCategoryId = table.Column<string>(type: "character varying(64)", nullable: false),
                    DishesId = table.Column<string>(type: "character varying(64)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishDishCategory", x => new { x.CategoriesDishCategoryId, x.DishesId });
                    table.ForeignKey(
                        name: "FK_DishDishCategory_DishCategories_CategoriesDishCategoryId",
                        column: x => x.CategoriesDishCategoryId,
                        principalTable: "DishCategories",
                        principalColumn: "DishCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishDishCategory_Dishes_DishesId",
                        column: x => x.DishesId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DishCategories_RestaurantId",
                table: "DishCategories",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_DishDishCategory_DishesId",
                table: "DishDishCategory",
                column: "DishesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DishDishCategory");

            migrationBuilder.DropTable(
                name: "DishCategories");

            migrationBuilder.RenameColumn(
                name: "PlatformType",
                table: "SocialLinks",
                newName: "Platform");

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "SocialLinks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "Services",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "Reviews",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DishId",
                table: "Reviews",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HeadId",
                table: "Restaurants",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "AreaId",
                table: "Restaurants",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Restaurants",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "Promotions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DishId",
                table: "Promotions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "Dishes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Dishes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Dishes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Areas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 11, 3, 4, 48, 34, 958, DateTimeKind.Utc).AddTicks(7448),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 11, 7, 7, 37, 19, 507, DateTimeKind.Utc).AddTicks(85));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Areas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 11, 3, 4, 48, 34, 958, DateTimeKind.Utc).AddTicks(7059),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 11, 7, 7, 37, 19, 506, DateTimeKind.Utc).AddTicks(9660));

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DishesCategories",
                columns: table => new
                {
                    DishId = table.Column<string>(type: "text", nullable: false),
                    CategoryId = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishesCategories", x => new { x.DishId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_DishesCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishesCategories_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DishesCategories_CategoryId",
                table: "DishesCategories",
                column: "CategoryId");
        }
    }
}
