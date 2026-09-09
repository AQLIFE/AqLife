using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AqLife.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "SubscriptionIcon",
                table: "Subscriptions",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "SubscriptionIcon",
                keyValue: null,
                column: "SubscriptionIcon",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "SubscriptionIcon",
                table: "Subscriptions",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");
        }
    }
}
