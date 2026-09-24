using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AqLife.Data.Migrations
{
    /// <inheritdoc />
    public partial class FileModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishAt",
                table: "FileMeta");

            migrationBuilder.DropColumn(
                name: "PublishStatus",
                table: "FileMeta");

            migrationBuilder.CreateTable(
                name: "FileInteractionMetas",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ViewCount = table.Column<int>(type: "int", nullable: false)
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FilePublishMetas",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PublishAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    PublishStatus = table.Column<int>(type: "int", nullable: false)
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileInteractionMetas");

            migrationBuilder.DropTable(
                name: "FilePublishMetas");

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishAt",
                table: "FileMeta",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PublishStatus",
                table: "FileMeta",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
