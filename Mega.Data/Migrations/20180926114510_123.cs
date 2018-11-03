using Microsoft.EntityFrameworkCore.Migrations;

namespace Mega.Data.Migrations
{
    public partial class _123 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleTag_Articles_ArticleId",
                table: "ArticleTag");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleTag_Tags_TagId",
                table: "ArticleTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArticleTag",
                table: "ArticleTag");

            migrationBuilder.RenameTable(
                name: "ArticleTag",
                newName: "ArticlesTags");

            migrationBuilder.RenameIndex(
                name: "IX_ArticleTag_TagId",
                table: "ArticlesTags",
                newName: "IX_ArticlesTags_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArticlesTags",
                table: "ArticlesTags",
                columns: new[] { "ArticleId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ArticlesTags_Articles_ArticleId",
                table: "ArticlesTags",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "ArticleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArticlesTags_Tags_TagId",
                table: "ArticlesTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "TagId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticlesTags_Articles_ArticleId",
                table: "ArticlesTags");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticlesTags_Tags_TagId",
                table: "ArticlesTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArticlesTags",
                table: "ArticlesTags");

            migrationBuilder.RenameTable(
                name: "ArticlesTags",
                newName: "ArticleTag");

            migrationBuilder.RenameIndex(
                name: "IX_ArticlesTags_TagId",
                table: "ArticleTag",
                newName: "IX_ArticleTag_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArticleTag",
                table: "ArticleTag",
                columns: new[] { "ArticleId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleTag_Articles_ArticleId",
                table: "ArticleTag",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "ArticleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleTag_Tags_TagId",
                table: "ArticleTag",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "TagId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
