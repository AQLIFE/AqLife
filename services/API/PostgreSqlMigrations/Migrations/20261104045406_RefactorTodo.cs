using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostgreSqlMigrations.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTodo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoList_TodoList_FTID",
                table: "TodoList");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TodoList",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "TodoList",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PlanId",
                table: "TodoList",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "TodoList",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "TodoList",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

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
                name: "RoadmapNodeLayouts",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    TodoId = table.Column<Guid>(type: "uuid", nullable: false),
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
                        name: "FK_RoadmapNodeLayouts_TodoList_TodoId",
                        column: x => x.TodoId,
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
                table: "DevelopmentPlans",
                columns: new[] { "UID", "CreatedAt", "DependencyVersion", "Description", "IsArchived", "Name", "UpdatedAt" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0L, null, false, "默认开发计划", new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.CreateIndex(
                name: "IX_TodoList_PlanId_FTID_SortOrder",
                table: "TodoList",
                columns: new[] { "PlanId", "FTID", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_TodoList_PlanId_Status",
                table: "TodoList",
                columns: new[] { "PlanId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapNodeLayouts_PlanId",
                table: "RoadmapNodeLayouts",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapNodeLayouts_TodoId",
                table: "RoadmapNodeLayouts",
                column: "TodoId",
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

            migrationBuilder.AddForeignKey(
                name: "FK_TodoList_DevelopmentPlans_PlanId",
                table: "TodoList",
                column: "PlanId",
                principalTable: "DevelopmentPlans",
                principalColumn: "UID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TodoList_TodoList_FTID",
                table: "TodoList",
                column: "FTID",
                principalTable: "TodoList",
                principalColumn: "UID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoList_DevelopmentPlans_PlanId",
                table: "TodoList");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoList_TodoList_FTID",
                table: "TodoList");

            migrationBuilder.DropTable(
                name: "RoadmapNodeLayouts");

            migrationBuilder.DropTable(
                name: "TodoDependencies");

            migrationBuilder.DropTable(
                name: "DevelopmentPlans");

            migrationBuilder.DropIndex(
                name: "IX_TodoList_PlanId_FTID_SortOrder",
                table: "TodoList");

            migrationBuilder.DropIndex(
                name: "IX_TodoList_PlanId_Status",
                table: "TodoList");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "TodoList");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "TodoList");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "TodoList");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "TodoList");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TodoList");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoList_TodoList_FTID",
                table: "TodoList",
                column: "FTID",
                principalTable: "TodoList",
                principalColumn: "UID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
