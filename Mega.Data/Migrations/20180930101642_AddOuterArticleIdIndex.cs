using Microsoft.EntityFrameworkCore.Migrations;

namespace Mega.Data.Migrations
{
    public partial class AddOuterArticleIdIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OuterArticleId",
                table: "Articles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_OuterArticleId",
                table: "Articles",
                column: "OuterArticleId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Articles_OuterArticleId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "OuterArticleId",
                table: "Articles");
        }
    }
}
