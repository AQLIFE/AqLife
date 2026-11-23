using AqLife.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostgreSqlMigrations.Migrations
{
    [DbContext(typeof(AppStorage))]
    [Migration("20261123120000_RestrictFileCategoryDelete")]
    public partial class RestrictFileCategoryDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileMeta_Category_CategoryUID",
                table: "FileMeta");

            migrationBuilder.AddForeignKey(
                name: "FK_FileMeta_Category_CategoryUID",
                table: "FileMeta",
                column: "CategoryUID",
                principalTable: "Category",
                principalColumn: "UID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileMeta_Category_CategoryUID",
                table: "FileMeta");

            migrationBuilder.AddForeignKey(
                name: "FK_FileMeta_Category_CategoryUID",
                table: "FileMeta",
                column: "CategoryUID",
                principalTable: "Category",
                principalColumn: "UID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}