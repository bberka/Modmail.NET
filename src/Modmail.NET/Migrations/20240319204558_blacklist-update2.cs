using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class blacklistupdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TicketBlacklists_DiscordUserInfoId",
                table: "TicketBlacklists");

            migrationBuilder.DropColumn(
                name: "EndDateUtc",
                table: "TicketBlacklists");

            migrationBuilder.CreateIndex(
                name: "IX_TicketBlacklists_DiscordUserInfoId_GuildOptionId",
                table: "TicketBlacklists",
                columns: new[] { "DiscordUserInfoId", "GuildOptionId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TicketBlacklists_DiscordUserInfoId_GuildOptionId",
                table: "TicketBlacklists");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDateUtc",
                table: "TicketBlacklists",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_TicketBlacklists_DiscordUserInfoId",
                table: "TicketBlacklists",
                column: "DiscordUserInfoId");
        }
    }
}
