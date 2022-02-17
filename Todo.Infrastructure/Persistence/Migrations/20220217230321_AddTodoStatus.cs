using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Todo.Infrastructure.Persistence.Migrations
{
    public partial class AddTodoStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TodoStatusId",
                table: "Todo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TodoStatus",
                columns: table => new
                {
                    TodoStatusId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(64)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoStatus", x => x.TodoStatusId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Todo_TodoStatusId",
                table: "Todo",
                column: "TodoStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Todo_TodoStatus_TodoStatusId",
                table: "Todo",
                column: "TodoStatusId",
                principalTable: "TodoStatus",
                principalColumn: "TodoStatusId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todo_TodoStatus_TodoStatusId",
                table: "Todo");

            migrationBuilder.DropTable(
                name: "TodoStatus");

            migrationBuilder.DropIndex(
                name: "IX_Todo_TodoStatusId",
                table: "Todo");

            migrationBuilder.DropColumn(
                name: "TodoStatusId",
                table: "Todo");
        }
    }
}
