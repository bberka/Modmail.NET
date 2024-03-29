using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class mg3_GuildOptionIdReferenceToOtherTablesRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuildTeams_GuildOptions_GuildOptionId",
                table: "GuildTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketBlacklists_GuildOptions_GuildId",
                table: "TicketBlacklists");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_GuildOptions_GuildOptionId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_GuildOptionId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_TicketBlacklists_DiscordUserId_GuildId",
                table: "TicketBlacklists");

            migrationBuilder.DropIndex(
                name: "IX_TicketBlacklists_GuildId",
                table: "TicketBlacklists");

            migrationBuilder.DropIndex(
                name: "IX_GuildTeams_GuildOptionId",
                table: "GuildTeams");

            migrationBuilder.DropColumn(
                name: "GuildOptionId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "TicketBlacklists");

            migrationBuilder.DropColumn(
                name: "GuildOptionId",
                table: "GuildTeams");

            migrationBuilder.AddColumn<ulong>(
                name: "GuildOptionGuildId",
                table: "Tickets",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "GuildOptionGuildId",
                table: "TicketBlacklists",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "GuildOptionGuildId",
                table: "GuildTeams",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_GuildOptionGuildId",
                table: "Tickets",
                column: "GuildOptionGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketBlacklists_DiscordUserId",
                table: "TicketBlacklists",
                column: "DiscordUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketBlacklists_GuildOptionGuildId",
                table: "TicketBlacklists",
                column: "GuildOptionGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildTeams_GuildOptionGuildId",
                table: "GuildTeams",
                column: "GuildOptionGuildId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "IX_TicketBlacklists_DiscordUserId",
                table: "TicketBlacklists");

            migrationBuilder.DropIndex(
                name: "IX_TicketBlacklists_GuildOptionGuildId",
                table: "TicketBlacklists");

            migrationBuilder.DropIndex(
                name: "IX_GuildTeams_GuildOptionGuildId",
                table: "GuildTeams");

            migrationBuilder.DropColumn(
                name: "GuildOptionGuildId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "GuildOptionGuildId",
                table: "TicketBlacklists");

            migrationBuilder.DropColumn(
                name: "GuildOptionGuildId",
                table: "GuildTeams");

            migrationBuilder.AddColumn<ulong>(
                name: "GuildOptionId",
                table: "Tickets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "GuildId",
                table: "TicketBlacklists",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "GuildOptionId",
                table: "GuildTeams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_GuildOptionId",
                table: "Tickets",
                column: "GuildOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketBlacklists_DiscordUserId_GuildId",
                table: "TicketBlacklists",
                columns: new[] { "DiscordUserId", "GuildId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketBlacklists_GuildId",
                table: "TicketBlacklists",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildTeams_GuildOptionId",
                table: "GuildTeams",
                column: "GuildOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_GuildTeams_GuildOptions_GuildOptionId",
                table: "GuildTeams",
                column: "GuildOptionId",
                principalTable: "GuildOptions",
                principalColumn: "GuildId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketBlacklists_GuildOptions_GuildId",
                table: "TicketBlacklists",
                column: "GuildId",
                principalTable: "GuildOptions",
                principalColumn: "GuildId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_GuildOptions_GuildOptionId",
                table: "Tickets",
                column: "GuildOptionId",
                principalTable: "GuildOptions",
                principalColumn: "GuildId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
