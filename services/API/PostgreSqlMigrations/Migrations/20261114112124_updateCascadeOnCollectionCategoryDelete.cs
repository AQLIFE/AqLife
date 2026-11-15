using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostgreSqlMigrations.Migrations
{
    /// <inheritdoc />
    public partial class updateCascadeOnCollectionCategoryDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteItems_Category_CategoryUID",
                table: "FavoriteItems");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteItems_Category_CategoryUID",
                table: "FavoriteItems",
                column: "CategoryUID",
                principalTable: "Category",
                principalColumn: "UID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteItems_Category_CategoryUID",
                table: "FavoriteItems");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteItems_Category_CategoryUID",
                table: "FavoriteItems",
                column: "CategoryUID",
                principalTable: "Category",
                principalColumn: "UID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
