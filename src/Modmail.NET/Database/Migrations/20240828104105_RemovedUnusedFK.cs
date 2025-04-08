using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class RemovedUnusedFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscordUserInfos_TicketBlacklists_TicketBlacklistId",
                table: "DiscordUserInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_GuildTeams_GuildOptions_GuildOptionGuildId",
                table: "GuildTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketBlacklists_GuildOptions_GuildOptionGuildId",
                table: "TicketBlacklists");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_GuildOptions_GuildOptionGuildId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_GuildOptionGuildId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_TicketBlacklists_GuildOptionGuildId",
                table: "TicketBlacklists");

            migrationBuilder.DropIndex(
                name: "IX_GuildTeams_GuildOptionGuildId",
                table: "GuildTeams");

            migrationBuilder.DropIndex(
                name: "IX_DiscordUserInfos_TicketBlacklistId",
                table: "DiscordUserInfos");

            migrationBuilder.DropColumn(
                name: "GuildOptionGuildId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "GuildOptionGuildId",
                table: "TicketBlacklists");

            migrationBuilder.DropColumn(
                name: "GuildOptionGuildId",
                table: "GuildTeams");

            migrationBuilder.DropColumn(
                name: "TicketBlacklistId",
                table: "DiscordUserInfos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "GuildOptionGuildId",
                table: "Tickets",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GuildOptionGuildId",
                table: "TicketBlacklists",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GuildOptionGuildId",
                table: "GuildTeams",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TicketBlacklistId",
                table: "DiscordUserInfos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_GuildOptionGuildId",
                table: "Tickets",
                column: "GuildOptionGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketBlacklists_GuildOptionGuildId",
                table: "TicketBlacklists",
                column: "GuildOptionGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildTeams_GuildOptionGuildId",
                table: "GuildTeams",
                column: "GuildOptionGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordUserInfos_TicketBlacklistId",
                table: "DiscordUserInfos",
                column: "TicketBlacklistId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscordUserInfos_TicketBlacklists_TicketBlacklistId",
                table: "DiscordUserInfos",
                column: "TicketBlacklistId",
                principalTable: "TicketBlacklists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GuildTeams_GuildOptions_GuildOptionGuildId",
                table: "GuildTeams",
                column: "GuildOptionGuildId",
                principalTable: "GuildOptions",
                principalColumn: "GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketBlacklists_GuildOptions_GuildOptionGuildId",
                table: "TicketBlacklists",
                column: "GuildOptionGuildId",
                principalTable: "GuildOptions",
                principalColumn: "GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_GuildOptions_GuildOptionGuildId",
                table: "Tickets",
                column: "GuildOptionGuildId",
                principalTable: "GuildOptions",
                principalColumn: "GuildId");
        }
    }
}
