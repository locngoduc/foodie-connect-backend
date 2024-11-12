using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace foodie_connect_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_Dishes_DishId",
                table: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_Promotions_DishId",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "BannerUrl",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "DishId",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "Target",
                table: "Promotions");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Promotions",
                newName: "EndsAt");

            migrationBuilder.RenameColumn(
                name: "ExpiredAt",
                table: "Promotions",
                newName: "BeginsAt");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Promotions",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.AddColumn<string>(
                name: "BannerId",
                table: "Promotions",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Promotions",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string[]>(
                name: "Targets",
                table: "Promotions",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.CreateTable(
                name: "PromotionDetail",
                columns: table => new
                {
                    PromotionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DishId = table.Column<Guid>(type: "uuid", nullable: false),
                    PromotionalPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionDetail", x => new { x.PromotionId, x.DishId });
                    table.ForeignKey(
                        name: "FK_PromotionDetail_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionDetail_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromotionDetail_DishId",
                table: "PromotionDetail",
                column: "DishId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromotionDetail");

            migrationBuilder.DropColumn(
                name: "BannerId",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "Targets",
                table: "Promotions");

            migrationBuilder.RenameColumn(
                name: "EndsAt",
                table: "Promotions",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "BeginsAt",
                table: "Promotions",
                newName: "ExpiredAt");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Promotions",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<string>(
                name: "BannerUrl",
                table: "Promotions",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Promotions",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "DishId",
                table: "Promotions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Promotions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Target",
                table: "Promotions",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_DishId",
                table: "Promotions",
                column: "DishId");

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_Dishes_DishId",
                table: "Promotions",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
