using Microsoft.EntityFrameworkCore.Migrations;

namespace Mega.Data.Migrations
{
    public partial class hasforeignkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "Uri",
                table: "Tags",
                newName: "TagKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TagKey",
                table: "Tags",
                newName: "Uri");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Articles",
                nullable: true);
        }
    }
}
