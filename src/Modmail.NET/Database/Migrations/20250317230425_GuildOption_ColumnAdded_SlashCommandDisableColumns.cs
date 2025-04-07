using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    /// <inheritdoc />
    public partial class GuildOption_ColumnAdded_SlashCommandDisableColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisableBlacklistSlashCommands",
                table: "GuildOptions");

            migrationBuilder.DropColumn(
                name: "DisableTicketSlashCommands",
                table: "GuildOptions");
        }
    }
}
