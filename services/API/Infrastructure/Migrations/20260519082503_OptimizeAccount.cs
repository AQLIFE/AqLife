using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AqLife.Data.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Avator",
                table: "Accounts",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avator",
                table: "Accounts");
        }
    }
}
