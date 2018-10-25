using Microsoft.EntityFrameworkCore.Migrations;

namespace Mega.Data.Migrations
{
    public partial class TableRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagsDelete_Tags_TagId",
                table: "TagsDelete");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagsDelete",
                table: "TagsDelete");

            migrationBuilder.RenameTable(
                name: "TagsDelete",
                newName: "RemovedTags");

            migrationBuilder.RenameIndex(
                name: "IX_TagsDelete_TagId",
                table: "RemovedTags",
                newName: "IX_RemovedTags_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RemovedTags",
                table: "RemovedTags",
                column: "RemovedTagId");

            migrationBuilder.AddForeignKey(
                name: "FK_RemovedTags_Tags_TagId",
                table: "RemovedTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "TagId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RemovedTags_Tags_TagId",
                table: "RemovedTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RemovedTags",
                table: "RemovedTags");

            migrationBuilder.RenameTable(
                name: "RemovedTags",
                newName: "TagsDelete");

            migrationBuilder.RenameIndex(
                name: "IX_RemovedTags_TagId",
                table: "TagsDelete",
                newName: "IX_TagsDelete_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagsDelete",
                table: "TagsDelete",
                column: "RemovedTagId");

            migrationBuilder.AddForeignKey(
                name: "FK_TagsDelete_Tags_TagId",
                table: "TagsDelete",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "TagId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
