using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLife.Data.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "FileMeta",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Extension",
                table: "FileMeta");
        }
    }
}
