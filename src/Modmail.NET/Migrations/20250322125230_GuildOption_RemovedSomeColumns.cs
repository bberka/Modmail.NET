using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    /// <inheritdoc />
    public partial class GuildOption_RemovedSomeColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowUsersToCloseTickets",
                table: "GuildOptions");

            migrationBuilder.DropColumn(
                name: "DisableBlacklistSlashCommands",
                table: "GuildOptions");

            migrationBuilder.DropColumn(
                name: "DisableTicketSlashCommands",
                table: "GuildOptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowUsersToCloseTickets",
                table: "GuildOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DisableBlacklistSlashCommands",
                table: "GuildOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DisableTicketSlashCommands",
                table: "GuildOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
