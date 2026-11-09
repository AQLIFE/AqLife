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
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryUID",
                table: "FavoriteItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteMedia",
                table: "FavoriteMedia",
                column: "UID");

            // The SQL default is only needed to assign unique IDs to rows that predate this column.
            migrationBuilder.Sql("ALTER TABLE \"FavoriteMedia\" ALTER COLUMN \"UID\" DROP DEFAULT;");

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

            // Preserve the old dual-purpose category tags as independent Category rows.
            // Refuse to migrate inconsistent files with multiple legacy categories rather
            // than silently discarding one of their category assignments.
            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT file_tag."FileId"
                        FROM "FileTags" AS file_tag
                        INNER JOIN "Tags" AS tag ON tag."UID" = file_tag."TagId"
                        WHERE tag."IsCategory" = TRUE
                        GROUP BY file_tag."FileId"
                        HAVING COUNT(*) > 1
                    ) THEN
                        RAISE EXCEPTION 'A file has multiple legacy category tags; resolve these assignments before applying the Category migration.';
                    END IF;
                END $$;

                INSERT INTO "Category" ("UID", "CategoryName", "CategoryDescription", "ParentId")
                SELECT tag."UID", tag."Name", '', NULL
                FROM "Tags" AS tag
                WHERE tag."IsCategory" = TRUE;

                UPDATE "FileMeta" AS file
                SET "CategoryUID" = file_tag."TagId"
                FROM "FileTags" AS file_tag
                INNER JOIN "Tags" AS tag ON tag."UID" = file_tag."TagId"
                WHERE file_tag."FileId" = file."UID"
                  AND tag."IsCategory" = TRUE;

                DELETE FROM "FileTags" AS file_tag
                USING "Tags" AS tag
                WHERE file_tag."TagId" = tag."UID"
                  AND tag."IsCategory" = TRUE;

                -- Favorite ContentType values become Category names. Reuse a category
                -- already created from a legacy category tag when the names match.
                INSERT INTO "Category" ("UID", "CategoryName", "CategoryDescription", "ParentId")
                SELECT gen_random_uuid(), favorite."ContentType", '', NULL
                FROM "FavoriteItems" AS favorite
                WHERE NULLIF(BTRIM(favorite."ContentType"), '') IS NOT NULL
                  AND NOT EXISTS (
                      SELECT 1
                      FROM "Category" AS category
                      WHERE category."CategoryName" = favorite."ContentType"
                  )
                GROUP BY favorite."ContentType";

                UPDATE "FavoriteItems" AS favorite
                SET "CategoryUID" = category."UID"
                FROM "Category" AS category
                WHERE favorite."ContentType" = category."CategoryName";
                """);

            migrationBuilder.DropColumn(
                name: "IsCategory",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "FavoriteItems");

            migrationBuilder.CreateIndex(
                name: "IX_FileMeta_CategoryUID",
                table: "FileMeta",
                column: "CategoryUID");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteMedia_FavoriteId",
                table: "FavoriteMedia",
                column: "FavoriteId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteMedia_FavoriteId_FileId",
                table: "FavoriteMedia",
                columns: new[] { "FavoriteId", "FileId" },
                unique: true);

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
                onDelete: ReferentialAction.SetNull);

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

            // Restore the legacy representation before dropping the independent categories.
            migrationBuilder.Sql("""
                UPDATE "Tags" AS tag
                SET "IsCategory" = TRUE
                FROM "Category" AS category
                WHERE tag."UID" = category."UID";

                INSERT INTO "FileTags" ("FileId", "TagId")
                SELECT file."UID", file."CategoryUID"
                FROM "FileMeta" AS file
                INNER JOIN "Tags" AS tag ON tag."UID" = file."CategoryUID"
                ON CONFLICT ("FileId", "TagId") DO NOTHING;

                UPDATE "FavoriteItems" AS favorite
                SET "ContentType" = category."CategoryName"
                FROM "Category" AS category
                WHERE favorite."CategoryUID" = category."UID";
                """);

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
                name: "IX_FavoriteMedia_FavoriteId_FileId",
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

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "TodoList",
                newName: "Desc");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteMedia",
                table: "FavoriteMedia",
                columns: new[] { "FavoriteId", "FileId" });
        }
    }
}
