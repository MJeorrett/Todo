using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Todo.Infrastructure.Persistence.Migrations
{
    public partial class AddAuditableEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Todo",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Todo",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Todo");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "Todo");
        }
    }
}
