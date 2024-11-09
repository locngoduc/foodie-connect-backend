using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace foodie_connect_backend.Migrations
{
    /// <inheritdoc />
    public partial class VariousFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_Dishes_DishId",
                table: "Promotions");

            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_Areas_AreaId",
                table: "Restaurants");

            migrationBuilder.DropIndex(
                name: "IX_Restaurants_Name",
                table: "Restaurants");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_Name_RestaurantId",
                table: "Dishes");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Services",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Restaurants",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "Restaurants",
                type: "geography",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography (Point,4326)");

            migrationBuilder.AlterColumn<string[]>(
                name: "Images",
                table: "Restaurants",
                type: "text[]",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldMaxLength: 128,
                oldDefaultValue: new string[0]);

            migrationBuilder.AlterColumn<string>(
                name: "Target",
                table: "Promotions",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Promotions",
                type: "uuid USING \"Id\"::uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Dishes",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "ImageId",
                table: "Dishes",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Areas",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 11, 8, 8, 53, 10, 6, DateTimeKind.Utc).AddTicks(134));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Areas",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 11, 8, 8, 53, 10, 5, DateTimeKind.Utc).AddTicks(9603));

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Areas",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_Dishes_DishId",
                table: "Promotions",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_Areas_AreaId",
                table: "Restaurants",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_Dishes_DishId",
                table: "Promotions");

            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_Areas_AreaId",
                table: "Restaurants");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Services",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Restaurants",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "Restaurants",
                type: "geography (Point,4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography");

            migrationBuilder.AlterColumn<string[]>(
                name: "Images",
                table: "Restaurants",
                type: "text[]",
                maxLength: 128,
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Target",
                table: "Promotions",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Promotions",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Dishes",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "ImageId",
                table: "Dishes",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Areas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 11, 8, 8, 53, 10, 6, DateTimeKind.Utc).AddTicks(134),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Areas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 11, 8, 8, 53, 10, 5, DateTimeKind.Utc).AddTicks(9603),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Areas",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_Name",
                table: "Restaurants",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_Name_RestaurantId",
                table: "Dishes",
                columns: new[] { "Name", "RestaurantId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_Dishes_DishId",
                table: "Promotions",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_Areas_AreaId",
                table: "Restaurants",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
