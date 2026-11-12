using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostgreSqlMigrations.Migrations
{
    /// <inheritdoc />
    public partial class DependencyOptimization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "UID", "CategoryDescription", "CategoryName", "ParentId" },
                values: new object[] { new Guid("3b246a0e-ed94-45e0-930d-15643683d50d"), "一个基础分类,不允许删除", "默认分类", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "UID",
                keyValue: new Guid("3b246a0e-ed94-45e0-930d-15643683d50d"));
        }
    }
}
