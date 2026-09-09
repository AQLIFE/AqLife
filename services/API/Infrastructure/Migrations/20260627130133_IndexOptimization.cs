using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AqLife.Data.Migrations
{
    /// <inheritdoc />
    public partial class IndexOptimization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Accounts_UID",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Tags_Name",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_SubscriptionPlatform_SubscriptionLink",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_UID",
                table: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "SID",
                table: "Subscriptions",
                newName: "AID");

            migrationBuilder.AlterColumn<string>(
                name: "AliasName",
                table: "Tags",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions",
                column: "UID");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_AID",
                table: "Subscriptions",
                column: "AID");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_SubscriptionPlatform_SubscriptionLink",
                table: "Subscriptions",
                columns: new[] { "SubscriptionPlatform", "SubscriptionLink" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Accounts_AID",
                table: "Subscriptions",
                column: "AID",
                principalTable: "Accounts",
                principalColumn: "UID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Accounts_AID",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Tags_Name",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_AID",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_SubscriptionPlatform_SubscriptionLink",
                table: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "AID",
                table: "Subscriptions",
                newName: "SID");

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "AliasName",
                keyValue: null,
                column: "AliasName",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "AliasName",
                table: "Tags",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions",
                column: "SID");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_SubscriptionPlatform_SubscriptionLink",
                table: "Subscriptions",
                columns: new[] { "SubscriptionPlatform", "SubscriptionLink" });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UID",
                table: "Subscriptions",
                column: "UID");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Accounts_UID",
                table: "Subscriptions",
                column: "UID",
                principalTable: "Accounts",
                principalColumn: "UID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
