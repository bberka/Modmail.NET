using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class GuildOption_Removed_GreetingAndClosingMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketBlacklists_DiscordUserInfos_DiscordUserId",
                table: "TicketBlacklists");

            migrationBuilder.DropColumn(
                name: "ClosingMessage",
                table: "GuildOptions");

            migrationBuilder.DropColumn(
                name: "GreetingMessage",
                table: "GuildOptions");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketBlacklists_DiscordUserInfos_DiscordUserId",
                table: "TicketBlacklists",
                column: "DiscordUserId",
                principalTable: "DiscordUserInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketBlacklists_DiscordUserInfos_DiscordUserId",
                table: "TicketBlacklists");

            migrationBuilder.AddColumn<string>(
                name: "ClosingMessage",
                table: "GuildOptions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GreetingMessage",
                table: "GuildOptions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketBlacklists_DiscordUserInfos_DiscordUserId",
                table: "TicketBlacklists",
                column: "DiscordUserId",
                principalTable: "DiscordUserInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
