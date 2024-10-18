using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace foodie_connect_backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HeadsHRoles");

            migrationBuilder.DropTable(
                name: "UsersURoles");

            migrationBuilder.DropTable(
                name: "HRoles");

            migrationBuilder.DropTable(
                name: "URoles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "URoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_URoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeadsHRoles",
                columns: table => new
                {
                    Headid = table.Column<int>(type: "integer", nullable: false),
                    Hroleid = table.Column<int>(type: "integer", nullable: false),
                    HeadId = table.Column<string>(type: "text", nullable: true),
                    Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeadsHRoles", x => new { x.Headid, x.Hroleid });
                    table.ForeignKey(
                        name: "FK_HeadsHRoles_HRoles_Hroleid",
                        column: x => x.Hroleid,
                        principalTable: "HRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeadsHRoles_Heads_HeadId",
                        column: x => x.HeadId,
                        principalTable: "Heads",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UsersURoles",
                columns: table => new
                {
                    Userid = table.Column<int>(type: "integer", nullable: false),
                    Uroleid = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersURoles", x => new { x.Userid, x.Uroleid });
                    table.ForeignKey(
                        name: "FK_UsersURoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UsersURoles_URoles_Uroleid",
                        column: x => x.Uroleid,
                        principalTable: "URoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HeadsHRoles_HeadId",
                table: "HeadsHRoles",
                column: "HeadId");

            migrationBuilder.CreateIndex(
                name: "IX_HeadsHRoles_Hroleid",
                table: "HeadsHRoles",
                column: "Hroleid");

            migrationBuilder.CreateIndex(
                name: "IX_UsersURoles_Uroleid",
                table: "UsersURoles",
                column: "Uroleid");

            migrationBuilder.CreateIndex(
                name: "IX_UsersURoles_UserId",
                table: "UsersURoles",
                column: "UserId");
        }
    }
}
