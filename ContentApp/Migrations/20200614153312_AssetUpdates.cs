using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentApp.Migrations
{
    public partial class AssetUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Asset");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOnUtc",
                table: "Asset",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UploadedFileName",
                table: "Asset",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOnUtc",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "UploadedFileName",
                table: "Asset");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Asset",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
