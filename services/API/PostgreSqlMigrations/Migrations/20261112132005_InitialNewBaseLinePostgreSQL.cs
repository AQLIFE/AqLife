using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PostgreSqlMigrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialNewBaseLinePostgreSQL : Migration
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
                name: "DevelopmentPlans",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DependencyVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevelopmentPlans", x => x.UID);
                });

            migrationBuilder.CreateTable(
                name: "SystemState",
                columns: table => new
                {
                    UID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SystemKeyHash = table.Column<string>(type: "text", nullable: false),
                    IsInitialized = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemState", x => x.UID);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    AliasName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.UID);
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
                name: "FavoriteItems",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SourceUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CategoryUID = table.Column<Guid>(type: "uuid", nullable: true),
                    CategoryEntityUID = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteItems", x => x.UID);
                    table.ForeignKey(
                        name: "FK_FavoriteItems_Category_CategoryEntityUID",
                        column: x => x.CategoryEntityUID,
                        principalTable: "Category",
                        principalColumn: "UID");
                    table.ForeignKey(
                        name: "FK_FavoriteItems_Category_CategoryUID",
                        column: x => x.CategoryUID,
                        principalTable: "Category",
                        principalColumn: "UID",
                        onDelete: ReferentialAction.SetNull);
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
                    IsTemplate = table.Column<bool>(type: "boolean", nullable: false),
                    CategoryUID = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileMeta", x => x.UID);
                    table.ForeignKey(
                        name: "FK_FileMeta_Category_CategoryUID",
                        column: x => x.CategoryUID,
                        principalTable: "Category",
                        principalColumn: "UID");
                });

            migrationBuilder.CreateTable(
                name: "TodoList",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000001")),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoList", x => x.UID);
                    table.ForeignKey(
                        name: "FK_TodoList_DevelopmentPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "DevelopmentPlans",
                        principalColumn: "UID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteTags",
                columns: table => new
                {
                    FavoriteId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteTags", x => new { x.FavoriteId, x.TagId });
                    table.ForeignKey(
                        name: "FK_FavoriteTags_FavoriteItems_FavoriteId",
                        column: x => x.FavoriteId,
                        principalTable: "FavoriteItems",
                        principalColumn: "UID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "UID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteMedia",
                columns: table => new
                {
                    FavoriteId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteMedia", x => new { x.FavoriteId, x.FileId });
                    table.ForeignKey(
                        name: "FK_FavoriteMedia_FavoriteItems_FavoriteId",
                        column: x => x.FavoriteId,
                        principalTable: "FavoriteItems",
                        principalColumn: "UID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteMedia_FileMeta_FileId",
                        column: x => x.FileId,
                        principalTable: "FileMeta",
                        principalColumn: "UID",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateTable(
                name: "RoadmapNodeLayouts",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    X = table.Column<double>(type: "double precision", nullable: false),
                    Y = table.Column<double>(type: "double precision", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadmapNodeLayouts", x => x.UID);
                    table.ForeignKey(
                        name: "FK_RoadmapNodeLayouts_DevelopmentPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "DevelopmentPlans",
                        principalColumn: "UID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoadmapNodeLayouts_TodoList_UID",
                        column: x => x.UID,
                        principalTable: "TodoList",
                        principalColumn: "UID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TodoDependencies",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    PredecessorId = table.Column<Guid>(type: "uuid", nullable: false),
                    SuccessorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoDependencies", x => x.UID);
                    table.ForeignKey(
                        name: "FK_TodoDependencies_DevelopmentPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "DevelopmentPlans",
                        principalColumn: "UID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TodoDependencies_TodoList_PredecessorId",
                        column: x => x.PredecessorId,
                        principalTable: "TodoList",
                        principalColumn: "UID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TodoDependencies_TodoList_SuccessorId",
                        column: x => x.SuccessorId,
                        principalTable: "TodoList",
                        principalColumn: "UID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "UID", "CategoryDescription", "CategoryName", "ParentId" },
                values: new object[] { new Guid("3b246a0e-ed94-45e0-930d-15643683d50d"), "一个基础分类,不允许删除", "默认分类", null });

            migrationBuilder.InsertData(
                table: "DevelopmentPlans",
                columns: new[] { "UID", "CreatedAt", "DependencyVersion", "Description", "IsArchived", "Name", "UpdatedAt" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0L, null, false, "默认开发计划", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) });

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
                name: "IX_Category_CategoryName",
                table: "Category",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_ParentId",
                table: "Category",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Corpus_CorpusContent",
                table: "Corpus",
                column: "CorpusContent",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteItems_CategoryEntityUID",
                table: "FavoriteItems",
                column: "CategoryEntityUID");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteItems_CategoryUID",
                table: "FavoriteItems",
                column: "CategoryUID");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteMedia_FileId",
                table: "FavoriteMedia",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteTags_TagId",
                table: "FavoriteTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_FileMeta_CategoryUID",
                table: "FileMeta",
                column: "CategoryUID");

            migrationBuilder.CreateIndex(
                name: "IX_FileTags_TagId",
                table: "FileTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapNodeLayouts_PlanId",
                table: "RoadmapNodeLayouts",
                column: "PlanId");

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
                name: "IX_TodoDependencies_PlanId_PredecessorId",
                table: "TodoDependencies",
                columns: new[] { "PlanId", "PredecessorId" });

            migrationBuilder.CreateIndex(
                name: "IX_TodoDependencies_PlanId_SuccessorId",
                table: "TodoDependencies",
                columns: new[] { "PlanId", "SuccessorId" });

            migrationBuilder.CreateIndex(
                name: "IX_TodoDependencies_PredecessorId_SuccessorId",
                table: "TodoDependencies",
                columns: new[] { "PredecessorId", "SuccessorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TodoDependencies_SuccessorId",
                table: "TodoDependencies",
                column: "SuccessorId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoList_PlanId",
                table: "TodoList",
                column: "PlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Corpus");

            migrationBuilder.DropTable(
                name: "FavoriteMedia");

            migrationBuilder.DropTable(
                name: "FavoriteTags");

            migrationBuilder.DropTable(
                name: "FileInteractionMetas");

            migrationBuilder.DropTable(
                name: "FilePublishMetas");

            migrationBuilder.DropTable(
                name: "FileTags");

            migrationBuilder.DropTable(
                name: "RoadmapNodeLayouts");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "SystemState");

            migrationBuilder.DropTable(
                name: "TodoDependencies");

            migrationBuilder.DropTable(
                name: "FavoriteItems");

            migrationBuilder.DropTable(
                name: "FileMeta");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "TodoList");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "DevelopmentPlans");
        }
    }
}
