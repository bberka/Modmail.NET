using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    /// <inheritdoc />
    public partial class GuildOptionRemovedShowConfirmationOnCloseCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowConfirmationWhenClosingTickets",
                table: "GuildOptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowConfirmationWhenClosingTickets",
                table: "GuildOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
