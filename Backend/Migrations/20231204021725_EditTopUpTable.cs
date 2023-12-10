using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    public partial class EditTopUpTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "TopUps");

            migrationBuilder.AddColumn<DateTime>(
                name: "TransactionDate",
                table: "TopUps",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionDate",
                table: "TopUps");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TopUps",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
