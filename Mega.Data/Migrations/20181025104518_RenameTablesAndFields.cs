using Microsoft.EntityFrameworkCore.Migrations;

namespace Mega.Data.Migrations
{
    public partial class RenameTablesAndFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateDelete",
                table: "TagsDelete",
                newName: "DeletionDate");

            migrationBuilder.RenameColumn(
                name: "TagDeleteId",
                table: "TagsDelete",
                newName: "RemovedTagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeletionDate",
                table: "TagsDelete",
                newName: "DateDelete");

            migrationBuilder.RenameColumn(
                name: "RemovedTagId",
                table: "TagsDelete",
                newName: "TagDeleteId");
        }
    }
}
