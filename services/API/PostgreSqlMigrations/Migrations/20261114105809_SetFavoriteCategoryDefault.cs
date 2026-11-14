using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostgreSqlMigrations.Migrations
{
    /// <inheritdoc />
    public partial class SetFavoriteCategoryDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryUID",
                table: "FavoriteItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("3b246a0e-ed94-45e0-930d-15643683d50d"),
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryUID",
                table: "FavoriteItems",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("3b246a0e-ed94-45e0-930d-15643683d50d"));
        }
    }
}
