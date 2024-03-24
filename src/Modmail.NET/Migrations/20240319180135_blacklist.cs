using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class blacklist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegisterDate",
                table: "TicketMessageAttachments");

            migrationBuilder.RenameColumn(
                name: "RegisterDate",
                table: "Tickets",
                newName: "RegisterDateUtc");

            migrationBuilder.RenameColumn(
                name: "LastMessageDate",
                table: "Tickets",
                newName: "LastMessageDateUtc");

            migrationBuilder.RenameColumn(
                name: "ClosedDate",
                table: "Tickets",
                newName: "ClosedDateUtc");

            migrationBuilder.RenameColumn(
                name: "RegisterDate",
                table: "TicketNotes",
                newName: "RegisterDateUtc");

            migrationBuilder.RenameColumn(
                name: "RegisterDate",
                table: "TicketFeedbacks",
                newName: "RegisterDateUtc");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "GuildTeams",
                newName: "UpdateDateUtc");

            migrationBuilder.RenameColumn(
                name: "RegisterDate",
                table: "GuildTeams",
                newName: "RegisterDateUtc");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "GuildTeamMembers",
                newName: "UpdateDateUtc");

            migrationBuilder.RenameColumn(
                name: "RegisterDate",
                table: "GuildTeamMembers",
                newName: "RegisterDateUtc");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "GuildOptions",
                newName: "UpdateDateUtc");

            migrationBuilder.RenameColumn(
                name: "RegisterDate",
                table: "GuildOptions",
                newName: "RegisterDateUtc");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "DiscordUserInfos",
                newName: "UpdateDateUtc");

            migrationBuilder.RenameColumn(
                name: "RegisterDate",
                table: "DiscordUserInfos",
                newName: "RegisterDateUtc");

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisterDateUtc",
                table: "TicketMessages",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "TicketBlacklists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: true),
                    DiscordUserInfoId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    BlacklistForMinutes = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketBlacklists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketBlacklists_DiscordUserInfos_DiscordUserInfoId",
                        column: x => x.DiscordUserInfoId,
                        principalTable: "DiscordUserInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketBlacklists_DiscordUserInfoId",
                table: "TicketBlacklists",
                column: "DiscordUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketBlacklists");

            migrationBuilder.DropColumn(
                name: "RegisterDateUtc",
                table: "TicketMessages");

            migrationBuilder.RenameColumn(
                name: "RegisterDateUtc",
                table: "Tickets",
                newName: "RegisterDate");

            migrationBuilder.RenameColumn(
                name: "LastMessageDateUtc",
                table: "Tickets",
                newName: "LastMessageDate");

            migrationBuilder.RenameColumn(
                name: "ClosedDateUtc",
                table: "Tickets",
                newName: "ClosedDate");

            migrationBuilder.RenameColumn(
                name: "RegisterDateUtc",
                table: "TicketNotes",
                newName: "RegisterDate");

            migrationBuilder.RenameColumn(
                name: "RegisterDateUtc",
                table: "TicketFeedbacks",
                newName: "RegisterDate");

            migrationBuilder.RenameColumn(
                name: "UpdateDateUtc",
                table: "GuildTeams",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "RegisterDateUtc",
                table: "GuildTeams",
                newName: "RegisterDate");

            migrationBuilder.RenameColumn(
                name: "UpdateDateUtc",
                table: "GuildTeamMembers",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "RegisterDateUtc",
                table: "GuildTeamMembers",
                newName: "RegisterDate");

            migrationBuilder.RenameColumn(
                name: "UpdateDateUtc",
                table: "GuildOptions",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "RegisterDateUtc",
                table: "GuildOptions",
                newName: "RegisterDate");

            migrationBuilder.RenameColumn(
                name: "UpdateDateUtc",
                table: "DiscordUserInfos",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "RegisterDateUtc",
                table: "DiscordUserInfos",
                newName: "RegisterDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisterDate",
                table: "TicketMessageAttachments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
