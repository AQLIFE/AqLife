using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostgreSqlMigrations.Migrations
{
    /// <inheritdoc />
    public partial class ManuallyoptimizeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteMedia",
                table: "FavoriteMedia");

            migrationBuilder.DropColumn(
                name: "IsCategory",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "FavoriteItems");

            migrationBuilder.RenameColumn(
                name: "Desc",
                table: "TodoList",
                newName: "Title");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryUID",
                table: "FileMeta",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UID",
                table: "FavoriteMedia",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryUID",
                table: "FavoriteItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "FavoriteCategoryId",
                table: "FavoriteItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteMedia",
                table: "FavoriteMedia",
                columns: new[] { "UID", "FavoriteId", "FileId" });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CategoryDescription = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.UID);
                    table.ForeignKey(
                        name: "FK_Category_Category_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Category",
                        principalColumn: "UID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileMeta_CategoryUID",
                table: "FileMeta",
                column: "CategoryUID");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteMedia_FavoriteId",
                table: "FavoriteMedia",
                column: "FavoriteId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteItems_CategoryUID",
                table: "FavoriteItems",
                column: "CategoryUID");

            migrationBuilder.CreateIndex(
                name: "IX_Category_CategoryName",
                table: "Category",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_ParentId",
                table: "Category",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteItems_Category_CategoryUID",
                table: "FavoriteItems",
                column: "CategoryUID",
                principalTable: "Category",
                principalColumn: "UID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileMeta_Category_CategoryUID",
                table: "FileMeta",
                column: "CategoryUID",
                principalTable: "Category",
                principalColumn: "UID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteItems_Category_CategoryUID",
                table: "FavoriteItems");

            migrationBuilder.DropForeignKey(
                name: "FK_FileMeta_Category_CategoryUID",
                table: "FileMeta");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropIndex(
                name: "IX_FileMeta_CategoryUID",
                table: "FileMeta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteMedia",
                table: "FavoriteMedia");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteMedia_FavoriteId",
                table: "FavoriteMedia");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteItems_CategoryUID",
                table: "FavoriteItems");

            migrationBuilder.DropColumn(
                name: "CategoryUID",
                table: "FileMeta");

            migrationBuilder.DropColumn(
                name: "UID",
                table: "FavoriteMedia");

            migrationBuilder.DropColumn(
                name: "CategoryUID",
                table: "FavoriteItems");

            migrationBuilder.DropColumn(
                name: "FavoriteCategoryId",
                table: "FavoriteItems");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "TodoList",
                newName: "Desc");

            migrationBuilder.AddColumn<bool>(
                name: "IsCategory",
                table: "Tags",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "FavoriteItems",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteMedia",
                table: "FavoriteMedia",
                columns: new[] { "FavoriteId", "FileId" });
        }
    }
}
