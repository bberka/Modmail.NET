using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class blacklistupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BlacklistForMinutes",
                table: "TicketBlacklists",
                newName: "GuildId");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDateUtc",
                table: "TicketBlacklists",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_TicketBlacklists_GuildOptionId",
                table: "TicketBlacklists",
                column: "GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketBlacklists_GuildOptions_GuildOptionId",
                table: "TicketBlacklists",
                column: "GuildId",
                principalTable: "GuildOptions",
                principalColumn: "GuildId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketBlacklists_GuildOptions_GuildOptionId",
                table: "TicketBlacklists");

            migrationBuilder.DropIndex(
                name: "IX_TicketBlacklists_GuildOptionId",
                table: "TicketBlacklists");

            migrationBuilder.DropColumn(
                name: "EndDateUtc",
                table: "TicketBlacklists");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "TicketBlacklists",
                newName: "BlacklistForMinutes");
        }
    }
}
