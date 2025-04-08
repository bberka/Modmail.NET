using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Database.Migrations
{
    /// <inheritdoc />
    public partial class TagsUpdated_AddedColTitle_RenamedShortcutToName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Shortcut",
                table: "Tags",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Tags",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Tags_Title_MinLength",
                table: "Tags",
                sql: "LEN([Title]) >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Tags_Title_MinLength",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Tags");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Tags",
                newName: "Shortcut");
        }
    }
}
