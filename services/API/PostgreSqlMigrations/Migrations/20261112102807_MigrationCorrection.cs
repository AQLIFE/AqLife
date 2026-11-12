using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostgreSqlMigrations.Migrations
{
    /// <inheritdoc />
    public partial class MigrationCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 删除旧外键（不存在也不会报错）
            migrationBuilder.Sql("""
        ALTER TABLE "TodoDependencies"
        DROP CONSTRAINT IF EXISTS "FK_TodoDependencies_TodoList_TodoEntityUID";

        ALTER TABLE "TodoDependencies"
        DROP CONSTRAINT IF EXISTS "FK_TodoDependencies_TodoList_TodoEntityUID1";

        ALTER TABLE "TodoList"
        DROP CONSTRAINT IF EXISTS "FK_TodoList_DevelopmentPlans_PlanUID";

        ALTER TABLE "TodoList"
        DROP CONSTRAINT IF EXISTS "FK_TodoList_RoadmapNodeLayouts_LayoutEntityUID";
        """);

            // 删除旧索引
            migrationBuilder.Sql("""
        DROP INDEX IF EXISTS "IX_TodoList_LayoutEntityUID";
        DROP INDEX IF EXISTS "IX_TodoList_PlanUID";
        DROP INDEX IF EXISTS "IX_TodoDependencies_TodoEntityUID";
        DROP INDEX IF EXISTS "IX_TodoDependencies_TodoEntityUID1";
        """);

            // 删除旧字段
            migrationBuilder.Sql("""
        ALTER TABLE "TodoList"
        DROP COLUMN IF EXISTS "LayoutEntityUID";

        ALTER TABLE "TodoList"
        DROP COLUMN IF EXISTS "PlanUID";

        ALTER TABLE "TodoDependencies"
        DROP COLUMN IF EXISTS "TodoEntityUID";

        ALTER TABLE "TodoDependencies"
        DROP COLUMN IF EXISTS "TodoEntityUID1";
        """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LayoutEntityUID",
                table: "TodoList",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PlanUID",
                table: "TodoList",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TodoEntityUID",
                table: "TodoDependencies",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TodoEntityUID1",
                table: "TodoDependencies",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TodoList_LayoutEntityUID",
                table: "TodoList",
                column: "LayoutEntityUID");

            migrationBuilder.CreateIndex(
                name: "IX_TodoList_PlanUID",
                table: "TodoList",
                column: "PlanUID");

            migrationBuilder.CreateIndex(
                name: "IX_TodoDependencies_TodoEntityUID",
                table: "TodoDependencies",
                column: "TodoEntityUID");

            migrationBuilder.CreateIndex(
                name: "IX_TodoDependencies_TodoEntityUID1",
                table: "TodoDependencies",
                column: "TodoEntityUID1");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoDependencies_TodoList_TodoEntityUID",
                table: "TodoDependencies",
                column: "TodoEntityUID",
                principalTable: "TodoList",
                principalColumn: "UID");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoDependencies_TodoList_TodoEntityUID1",
                table: "TodoDependencies",
                column: "TodoEntityUID1",
                principalTable: "TodoList",
                principalColumn: "UID");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoList_DevelopmentPlans_PlanUID",
                table: "TodoList",
                column: "PlanUID",
                principalTable: "DevelopmentPlans",
                principalColumn: "UID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TodoList_RoadmapNodeLayouts_LayoutEntityUID",
                table: "TodoList",
                column: "LayoutEntityUID",
                principalTable: "RoadmapNodeLayouts",
                principalColumn: "UID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
