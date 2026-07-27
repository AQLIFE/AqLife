using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLife.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixTheSpellingErrorsInTheFieldsOfTheAccountModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Avator",
                table: "Accounts",
                newName: "Avatar");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Avatar",
                table: "Accounts",
                newName: "Avator");
        }
    }
}
