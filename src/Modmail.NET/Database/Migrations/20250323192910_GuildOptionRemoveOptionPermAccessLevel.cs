using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    /// <inheritdoc />
    public partial class GuildOptionRemoveOptionPermAccessLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManageGuildOptionMinAccessLevel",
                table: "GuildOptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManageGuildOptionMinAccessLevel",
                table: "GuildOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
