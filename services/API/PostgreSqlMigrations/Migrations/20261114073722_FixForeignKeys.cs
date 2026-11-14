using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostgreSqlMigrations.Migrations
{
    /// <inheritdoc />
    public partial class FixForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteItems_Category_CategoryEntityUID",
                table: "FavoriteItems");

            migrationBuilder.DropForeignKey(
                name: "FK_FileMeta_Category_CategoryUID",
                table: "FileMeta");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteItems_CategoryEntityUID",
                table: "FavoriteItems");

            migrationBuilder.DropColumn(
                name: "CategoryEntityUID",
                table: "FavoriteItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryUID",
                table: "FileMeta",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FileMeta_Category_CategoryUID",
                table: "FileMeta",
                column: "CategoryUID",
                principalTable: "Category",
                principalColumn: "UID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileMeta_Category_CategoryUID",
                table: "FileMeta");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryUID",
                table: "FileMeta",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryEntityUID",
                table: "FavoriteItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteItems_CategoryEntityUID",
                table: "FavoriteItems",
                column: "CategoryEntityUID");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteItems_Category_CategoryEntityUID",
                table: "FavoriteItems",
                column: "CategoryEntityUID",
                principalTable: "Category",
                principalColumn: "UID");

            migrationBuilder.AddForeignKey(
                name: "FK_FileMeta_Category_CategoryUID",
                table: "FileMeta",
                column: "CategoryUID",
                principalTable: "Category",
                principalColumn: "UID");
        }
    }
}
