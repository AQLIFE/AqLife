using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostgreSqlMigrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgreSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Desc = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Avatar = table.Column<Guid>(type: "uuid", nullable: false),
                    IsValid = table.Column<bool>(type: "boolean", nullable: false),
                    LoginName = table.Column<string>(type: "text", nullable: false),
                    LoginPasswordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.UID);
                });

            migrationBuilder.CreateTable(
                name: "Corpus",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "uuid", nullable: false),
                    CorpusContent = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Corpus", x => x.UID);
                });

            migrationBuilder.CreateTable(
                name: "FileMeta",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Extension = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    FileHash = table.Column<string>(type: "text", nullable: false),
                    UploadTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    IsTemplate = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileMeta", x => x.UID);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    AliasName = table.Column<string>(type: "text", nullable: true),
                    IsCategory = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.UID);
                });

            migrationBuilder.CreateTable(
                name: "TodoList",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "uuid", nullable: false),
                    FTID = table.Column<Guid>(type: "uuid", nullable: true),
                    Desc = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoList", x => x.UID);
                    table.ForeignKey(
                        name: "FK_TodoList_TodoList_FTID",
                        column: x => x.FTID,
                        principalTable: "TodoList",
                        principalColumn: "UID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "uuid", nullable: false),
                    AID = table.Column<Guid>(type: "uuid", nullable: false),
                    AliasName = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    SubscriptionLink = table.Column<string>(type: "text", nullable: false),
                    SubscriptionPlatform = table.Column<string>(type: "text", nullable: false),
                    SubscriptionIcon = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.UID);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Accounts_AID",
                        column: x => x.AID,
                        principalTable: "Accounts",
                        principalColumn: "UID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileInteractionMetas",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "uuid", nullable: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileInteractionMetas", x => x.UID);
                    table.ForeignKey(
                        name: "FK_FileInteractionMetas_FileMeta_UID",
                        column: x => x.UID,
                        principalTable: "FileMeta",
                        principalColumn: "UID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FilePublishMetas",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "uuid", nullable: false),
                    PublishAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PublishStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilePublishMetas", x => x.UID);
                    table.ForeignKey(
                        name: "FK_FilePublishMetas_FileMeta_UID",
                        column: x => x.UID,
                        principalTable: "FileMeta",
                        principalColumn: "UID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileTags",
                columns: table => new
                {
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileTags", x => new { x.FileId, x.TagId });
                    table.ForeignKey(
                        name: "FK_FileTags_FileMeta_FileId",
                        column: x => x.FileId,
                        principalTable: "FileMeta",
                        principalColumn: "UID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "UID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_LoginName",
                table: "Accounts",
                column: "LoginName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Name",
                table: "Accounts",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Corpus_CorpusContent",
                table: "Corpus",
                column: "CorpusContent",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileTags_TagId",
                table: "FileTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_AID",
                table: "Subscriptions",
                column: "AID");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_SubscriptionPlatform_SubscriptionLink",
                table: "Subscriptions",
                columns: new[] { "SubscriptionPlatform", "SubscriptionLink" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TodoList_FTID",
                table: "TodoList",
                column: "FTID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Corpus");

            migrationBuilder.DropTable(
                name: "FileInteractionMetas");

            migrationBuilder.DropTable(
                name: "FilePublishMetas");

            migrationBuilder.DropTable(
                name: "FileTags");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "TodoList");

            migrationBuilder.DropTable(
                name: "FileMeta");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
