using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLife.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTodoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TodoList",
                columns: table => new
                {
                    TID = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FTID = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Desc = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoList", x => x.TID);
                    table.ForeignKey(
                        name: "FK_TodoList_TodoList_FTID",
                        column: x => x.FTID,
                        principalTable: "TodoList",
                        principalColumn: "TID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TodoList_FTID",
                table: "TodoList",
                column: "FTID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TodoList");
        }
    }
}
