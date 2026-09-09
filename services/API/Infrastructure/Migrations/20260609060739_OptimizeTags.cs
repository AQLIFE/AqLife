using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AqLife.Data.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AliasName",
                table: "Tags",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AliasName",
                table: "Tags");
        }
    }
}
