using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    /// <inheritdoc />
    public partial class GuildOption_AddedColumn_AlwaysAnonymous : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AlwaysAnonymous",
                table: "GuildOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlwaysAnonymous",
                table: "GuildOptions");
        }
    }
}
