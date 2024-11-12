using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace foodie_connect_backend.Migrations
{
    public partial class TimeOnlyUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpenTime",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "CloseTime",
                table: "Restaurants");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "OpenTime",
                table: "Restaurants",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(9, 0)); // Default to 9:00 AM

            migrationBuilder.AddColumn<TimeOnly>(
                name: "CloseTime",
                table: "Restaurants",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(17, 0)); // Default to 5:00 PM
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpenTime",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "CloseTime",
                table: "Restaurants");

            migrationBuilder.AddColumn<int>(
                name: "OpenTime",
                table: "Restaurants",
                type: "integer",
                nullable: false,
                defaultValue: 900); // 9:00 AM

            migrationBuilder.AddColumn<int>(
                name: "CloseTime",
                table: "Restaurants",
                type: "integer",
                nullable: false,
                defaultValue: 1700); // 5:00 PM
        }
    }
}