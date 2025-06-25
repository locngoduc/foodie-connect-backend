﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;


#nullable disable


namespace foodie_connect_backend.Migrations
{
   /// <inheritdoc />
   public partial class InitialCreate : Migration
   {
       /// <inheritdoc />
       protected override void Up(MigrationBuilder migrationBuilder)
       {
           migrationBuilder.AlterDatabase()
               .Annotation("Npgsql:PostgresExtension:postgis", ",,");


           migrationBuilder.CreateTable(
               name: "Areas",
               columns: table => new
               {
                   Id = table.Column<Guid>(type: "uuid", nullable: false),
                   FormattedAddress = table.Column<string>(type: "text", nullable: false),
                   StreetAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                   Route = table.Column<string>(type: "text", nullable: true),
                   Intersection = table.Column<string>(type: "text", nullable: true),
                   PoliticalEntity = table.Column<string>(type: "text", nullable: true),
                   Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                   AdministrativeAreaLevel1 = table.Column<string>(type: "text", nullable: true),
                   AdministrativeAreaLevel2 = table.Column<string>(type: "text", nullable: true),
                   AdministrativeAreaLevel3 = table.Column<string>(type: "text", nullable: true),
                   Locality = table.Column<string>(type: "text", nullable: true),
                   Sublocality = table.Column<string>(type: "text", nullable: true),
                   Neighborhood = table.Column<string>(type: "text", nullable: true),
                   Premise = table.Column<string>(type: "text", nullable: true),
                   Subpremise = table.Column<string>(type: "text", nullable: true),
                   PostalCode = table.Column<string>(type: "text", nullable: true),
                   PlusCode = table.Column<string>(type: "text", nullable: true),
                   NaturalFeature = table.Column<string>(type: "text", nullable: true),
                   Airport = table.Column<string>(type: "text", nullable: true),
                   Park = table.Column<string>(type: "text", nullable: true),
                   PointOfInterest = table.Column<string>(type: "text", nullable: true),
                   CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 11, 8, 8, 53, 10, 5, DateTimeKind.Utc).AddTicks(9603)),
                   UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 11, 8, 8, 53, 10, 6, DateTimeKind.Utc).AddTicks(134))
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_Areas", x => x.Id);
               });


           migrationBuilder.CreateTable(
               name: "AspNetRoles",
               columns: table => new
               {
                   Id = table.Column<string>(type: "text", nullable: false),
                   Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                   NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                   ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_AspNetRoles", x => x.Id);
               });


           migrationBuilder.CreateTable(
               name: "AspNetUsers",
               columns: table => new
               {
                   Id = table.Column<string>(type: "text", nullable: false),
                   DisplayName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                   AvatarId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                   CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                   UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                   UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                   NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                   Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                   NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                   EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                   PasswordHash = table.Column<string>(type: "text", nullable: true),
                   SecurityStamp = table.Column<string>(type: "text", nullable: true),
                   ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                   PhoneNumber = table.Column<string>(type: "text", nullable: true),
                   PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                   TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                   LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                   LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                   AccessFailedCount = table.Column<int>(type: "integer", nullable: false),
                   Role = table.Column<string>(type: "character varying(256)", maxLength: 16, nullable: true),
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_AspNetUsers", x => x.Id);
               });


           migrationBuilder.CreateTable(
               name: "Restaurants",
               columns: table => new
               {
                   Id = table.Column<Guid>(type: "uuid", nullable: false),
                   Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                   OpenTime = table.Column<int>(type: "integer", nullable: false),
                   CloseTime = table.Column<int>(type: "integer", nullable: false),
                   Status = table.Column<int>(type: "integer", nullable: false),
                   Phone = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                   Images = table.Column<string[]>(type: "text[]", maxLength: 128, nullable: false, defaultValue: new string[0]),
                   CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                   UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                   AreaId = table.Column<Guid>(type: "uuid", nullable: false),
                   IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                   Location = table.Column<Point>(type: "geography (Point,4326)", nullable: false),
                   HeadId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_Restaurants", x => x.Id);
                   table.ForeignKey(
                       name: "FK_Restaurants_Areas_AreaId",
                       column: x => x.AreaId,
                       principalTable: "Areas",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.SetNull);
               });


           migrationBuilder.CreateTable(
               name: "AspNetRoleClaims",
               columns: table => new
               {
                   Id = table.Column<int>(type: "integer", nullable: false)
                       .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                   RoleId = table.Column<string>(type: "text", nullable: false),
                   ClaimType = table.Column<string>(type: "text", nullable: true),
                   ClaimValue = table.Column<string>(type: "text", nullable: true)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                   table.ForeignKey(
                       name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                       column: x => x.RoleId,
                       principalTable: "AspNetRoles",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
               });


           migrationBuilder.CreateTable(
               name: "AspNetUserClaims",
               columns: table => new
               {
                   Id = table.Column<int>(type: "integer", nullable: false)
                       .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                   UserId = table.Column<string>(type: "text", nullable: false),
                   ClaimType = table.Column<string>(type: "text", nullable: true),
                   ClaimValue = table.Column<string>(type: "text", nullable: true)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                   table.ForeignKey(
                       name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                       column: x => x.UserId,
                       principalTable: "AspNetUsers",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
               });


           migrationBuilder.CreateTable(
               name: "AspNetUserLogins",
               columns: table => new
               {
                   LoginProvider = table.Column<string>(type: "text", nullable: false),
                   ProviderKey = table.Column<string>(type: "text", nullable: false),
                   ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                   UserId = table.Column<string>(type: "text", nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                   table.ForeignKey(
                       name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                       column: x => x.UserId,
                       principalTable: "AspNetUsers",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
               });


           migrationBuilder.CreateTable(
               name: "AspNetUserRoles",
               columns: table => new
               {
                   UserId = table.Column<string>(type: "text", nullable: false),
                   RoleId = table.Column<string>(type: "text", nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                   table.ForeignKey(
                       name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                       column: x => x.RoleId,
                       principalTable: "AspNetRoles",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
                   table.ForeignKey(
                       name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                       column: x => x.UserId,
                       principalTable: "AspNetUsers",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
               });


           migrationBuilder.CreateTable(
               name: "AspNetUserTokens",
               columns: table => new
               {
                   UserId = table.Column<string>(type: "text", nullable: false),
                   LoginProvider = table.Column<string>(type: "text", nullable: false),
                   Name = table.Column<string>(type: "text", nullable: false),
                   Value = table.Column<string>(type: "text", nullable: true)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                   table.ForeignKey(
                       name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                       column: x => x.UserId,
                       principalTable: "AspNetUsers",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
               });


           migrationBuilder.CreateTable(
               name: "DishCategories",
               columns: table => new
               {
                   RestaurantId = table.Column<Guid>(type: "uuid", maxLength: 128, nullable: false),
                   CategoryName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_DishCategories", x => new { x.RestaurantId, x.CategoryName });
                   table.ForeignKey(
                       name: "FK_DishCategories_Restaurants_RestaurantId",
                       column: x => x.RestaurantId,
                       principalTable: "Restaurants",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
               });


           migrationBuilder.CreateTable(
               name: "Dishes",
               columns: table => new
               {
                   Id = table.Column<Guid>(type: "uuid", nullable: false),
                   Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                   ImageId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                   Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                   Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                   CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                   UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                   RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                   IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_Dishes", x => x.Id);
                   table.ForeignKey(
                       name: "FK_Dishes_Restaurants_RestaurantId",
                       column: x => x.RestaurantId,
                       principalTable: "Restaurants",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
               });


           migrationBuilder.CreateTable(
               name: "Services",
               columns: table => new
               {
                   Id = table.Column<Guid>(type: "uuid", nullable: false),
                   Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                   IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                   CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                   UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                   RestaurantId = table.Column<Guid>(type: "uuid", nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_Services", x => x.Id);
                   table.ForeignKey(
                       name: "FK_Services_Restaurants_RestaurantId",
                       column: x => x.RestaurantId,
                       principalTable: "Restaurants",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
               });


           migrationBuilder.CreateTable(
               name: "SocialLinks",
               columns: table => new
               {
                   Id = table.Column<string>(type: "text", nullable: false),
                   RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                   PlatformType = table.Column<int>(type: "integer", nullable: false),
                   Url = table.Column<string>(type: "text", nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_SocialLinks", x => x.Id);
                   table.ForeignKey(
                       name: "FK_SocialLinks_Restaurants_RestaurantId",
                       column: x => x.RestaurantId,
                       principalTable: "Restaurants",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
               });


           migrationBuilder.CreateTable(
               name: "DishDishCategory",
               columns: table => new
               {
                   DishesId = table.Column<Guid>(type: "uuid", nullable: false),
                   CategoriesRestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                   CategoriesCategoryName = table.Column<string>(type: "character varying(32)", nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_DishDishCategory", x => new { x.DishesId, x.CategoriesRestaurantId, x.CategoriesCategoryName });
                   table.ForeignKey(
                       name: "FK_DishDishCategory_DishCategories_CategoriesRestaurantId_Cate~",
                       columns: x => new { x.CategoriesRestaurantId, x.CategoriesCategoryName },
                       principalTable: "DishCategories",
                       principalColumns: new[] { "RestaurantId", "CategoryName" },
                       onDelete: ReferentialAction.Cascade);
                   table.ForeignKey(
                       name: "FK_DishDishCategory_Dishes_DishesId",
                       column: x => x.DishesId,
                       principalTable: "Dishes",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
               });


           migrationBuilder.CreateTable(
               name: "Promotions",
               columns: table => new
               {
                   Id = table.Column<string>(type: "text", nullable: false),
                   Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                   Target = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                   BannerUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                   ExpiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                   CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                   UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                   IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                   RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                   DishId = table.Column<Guid>(type: "uuid", nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_Promotions", x => x.Id);
                   table.ForeignKey(
                       name: "FK_Promotions_Dishes_DishId",
                       column: x => x.DishId,
                       principalTable: "Dishes",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.SetNull);
                   table.ForeignKey(
                       name: "FK_Promotions_Restaurants_RestaurantId",
                       column: x => x.RestaurantId,
                       principalTable: "Restaurants",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
               });


           migrationBuilder.CreateTable(
               name: "Reviews",
               columns: table => new
               {
                   Id = table.Column<Guid>(type: "uuid", nullable: false),
                   DishId = table.Column<Guid>(type: "uuid", nullable: false),
                   RestaurantId = table.Column<Guid>(type: "uuid", nullable: true)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_Reviews", x => x.Id);
                   table.ForeignKey(
                       name: "FK_Reviews_Dishes_DishId",
                       column: x => x.DishId,
                       principalTable: "Dishes",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
                   table.ForeignKey(
                       name: "FK_Reviews_Restaurants_RestaurantId",
                       column: x => x.RestaurantId,
                       principalTable: "Restaurants",
                       principalColumn: "Id");
               });


           migrationBuilder.CreateIndex(
               name: "IX_AspNetRoleClaims_RoleId",
               table: "AspNetRoleClaims",
               column: "RoleId");


           migrationBuilder.CreateIndex(
               name: "RoleNameIndex",
               table: "AspNetRoles",
               column: "NormalizedName",
               unique: true);


           migrationBuilder.CreateIndex(
               name: "IX_AspNetUserClaims_UserId",
               table: "AspNetUserClaims",
               column: "UserId");


           migrationBuilder.CreateIndex(
               name: "IX_AspNetUserLogins_UserId",
               table: "AspNetUserLogins",
               column: "UserId");


           migrationBuilder.CreateIndex(
               name: "IX_AspNetUserRoles_RoleId",
               table: "AspNetUserRoles",
               column: "RoleId");


           migrationBuilder.CreateIndex(
               name: "EmailIndex",
               table: "AspNetUsers",
               column: "NormalizedEmail");


           migrationBuilder.CreateIndex(
               name: "UserNameIndex",
               table: "AspNetUsers",
               column: "NormalizedUserName",
               unique: true);


           migrationBuilder.CreateIndex(
               name: "IX_DishDishCategory_CategoriesRestaurantId_CategoriesCategoryN~",
               table: "DishDishCategory",
               columns: new[] { "CategoriesRestaurantId", "CategoriesCategoryName" });


           migrationBuilder.CreateIndex(
               name: "IX_Dishes_Name_RestaurantId",
               table: "Dishes",
               columns: new[] { "Name", "RestaurantId" },
               unique: true);


           migrationBuilder.CreateIndex(
               name: "IX_Dishes_RestaurantId",
               table: "Dishes",
               column: "RestaurantId");


           migrationBuilder.CreateIndex(
               name: "IX_Promotions_DishId",
               table: "Promotions",
               column: "DishId");


           migrationBuilder.CreateIndex(
               name: "IX_Promotions_RestaurantId",
               table: "Promotions",
               column: "RestaurantId");


           migrationBuilder.CreateIndex(
               name: "IX_Restaurants_AreaId",
               table: "Restaurants",
               column: "AreaId");


           migrationBuilder.CreateIndex(
               name: "IX_Restaurants_Name",
               table: "Restaurants",
               column: "Name",
               unique: true);


           migrationBuilder.CreateIndex(
               name: "IX_Reviews_DishId",
               table: "Reviews",
               column: "DishId");


           migrationBuilder.CreateIndex(
               name: "IX_Reviews_RestaurantId",
               table: "Reviews",
               column: "RestaurantId");


           migrationBuilder.CreateIndex(
               name: "IX_Services_RestaurantId",
               table: "Services",
               column: "RestaurantId");


           migrationBuilder.CreateIndex(
               name: "IX_SocialLinks_RestaurantId",
               table: "SocialLinks",
               column: "RestaurantId");
       }


       /// <inheritdoc />
       protected override void Down(MigrationBuilder migrationBuilder)
       {
           migrationBuilder.DropTable(
               name: "AspNetRoleClaims");


           migrationBuilder.DropTable(
               name: "AspNetUserClaims");


           migrationBuilder.DropTable(
               name: "AspNetUserLogins");


           migrationBuilder.DropTable(
               name: "AspNetUserRoles");


           migrationBuilder.DropTable(
               name: "AspNetUserTokens");


           migrationBuilder.DropTable(
               name: "DishDishCategory");


           migrationBuilder.DropTable(
               name: "Promotions");


           migrationBuilder.DropTable(
               name: "Reviews");


           migrationBuilder.DropTable(
               name: "Services");


           migrationBuilder.DropTable(
               name: "SocialLinks");


           migrationBuilder.DropTable(
               name: "AspNetRoles");


           migrationBuilder.DropTable(
               name: "AspNetUsers");


           migrationBuilder.DropTable(
               name: "DishCategories");


           migrationBuilder.DropTable(
               name: "Dishes");


           migrationBuilder.DropTable(
               name: "Restaurants");


           migrationBuilder.DropTable(
               name: "Areas");
       }
   }
}

