using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace foodie_connect_backend.Migrations
{
    /// <inheritdoc />
    public partial class Bumpedmaxlength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "SocialLinks",
                type: "character varying(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "Services",
                type: "character varying(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "Reviews",
                type: "character varying(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DishId",
                table: "Reviews",
                type: "character varying(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HeadId",
                table: "Restaurants",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "AreaId",
                table: "Restaurants",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Restaurants",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "Promotions",
                type: "character varying(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DishId",
                table: "Promotions",
                type: "character varying(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "Dishes",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Dishes",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "CategoriesRestaurantId",
                table: "DishDishCategory",
                type: "character varying(128)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)");

            migrationBuilder.AlterColumn<string>(
                name: "DishesId",
                table: "DishDishCategory",
                type: "character varying(128)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)");

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "DishCategories",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "AvatarId",
                table: "AspNetUsers",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Areas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 11, 7, 8, 42, 7, 231, DateTimeKind.Utc).AddTicks(870),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 11, 7, 7, 44, 54, 404, DateTimeKind.Utc).AddTicks(9421));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Areas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 11, 7, 8, 42, 7, 231, DateTimeKind.Utc).AddTicks(383),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 11, 7, 7, 44, 54, 404, DateTimeKind.Utc).AddTicks(8973));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "SocialLinks",
                type: "character varying(64)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "Services",
                type: "character varying(64)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "Reviews",
                type: "character varying(64)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DishId",
                table: "Reviews",
                type: "character varying(64)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HeadId",
                table: "Restaurants",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "AreaId",
                table: "Restaurants",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Restaurants",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "Promotions",
                type: "character varying(64)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DishId",
                table: "Promotions",
                type: "character varying(64)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "Dishes",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Dishes",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "CategoriesRestaurantId",
                table: "DishDishCategory",
                type: "character varying(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)");

            migrationBuilder.AlterColumn<string>(
                name: "DishesId",
                table: "DishDishCategory",
                type: "character varying(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)");

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantId",
                table: "DishCategories",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "AvatarId",
                table: "AspNetUsers",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Areas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 11, 7, 7, 44, 54, 404, DateTimeKind.Utc).AddTicks(9421),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 11, 7, 8, 42, 7, 231, DateTimeKind.Utc).AddTicks(870));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Areas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 11, 7, 7, 44, 54, 404, DateTimeKind.Utc).AddTicks(8973),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 11, 7, 8, 42, 7, 231, DateTimeKind.Utc).AddTicks(383));
        }
    }
}
