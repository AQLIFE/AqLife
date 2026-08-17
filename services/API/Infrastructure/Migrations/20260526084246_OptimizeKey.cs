using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLife.Data.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TID",
                table: "TodoList",
                newName: "UID");

            migrationBuilder.RenameColumn(
                name: "Uuid",
                table: "FileMeta",
                newName: "UID");

            migrationBuilder.RenameColumn(
                name: "Gid",
                table: "Corpus",
                newName: "UID");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_AliasName",
                table: "Subscriptions",
                column: "AliasName");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Name",
                table: "Accounts",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_AliasName",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_Name",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "UID",
                table: "TodoList",
                newName: "TID");

            migrationBuilder.RenameColumn(
                name: "UID",
                table: "FileMeta",
                newName: "Uuid");

            migrationBuilder.RenameColumn(
                name: "UID",
                table: "Corpus",
                newName: "Gid");
        }
    }
}
