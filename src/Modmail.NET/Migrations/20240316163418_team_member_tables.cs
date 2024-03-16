using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class team_member_tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "Tickets");

            migrationBuilder.AddColumn<Guid>(
                name: "GuildOptionId",
                table: "Tickets",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GuildOptionId",
                table: "Tags",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GuildTeams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    GuildOptionId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuildTeams_GuildOptions_GuildOptionId",
                        column: x => x.GuildOptionId,
                        principalTable: "GuildOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuildTeamMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Key = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    GuildTeamId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildTeamMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuildTeamMembers_GuildTeams_GuildTeamId",
                        column: x => x.GuildTeamId,
                        principalTable: "GuildTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_GuildOptionId",
                table: "Tickets",
                column: "GuildOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_GuildOptionId",
                table: "Tags",
                column: "GuildOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildTeamMembers_GuildTeamId",
                table: "GuildTeamMembers",
                column: "GuildTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildTeams_GuildOptionId",
                table: "GuildTeams",
                column: "GuildOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_GuildOptions_GuildOptionId",
                table: "Tags",
                column: "GuildOptionId",
                principalTable: "GuildOptions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_GuildOptions_GuildOptionId",
                table: "Tickets",
                column: "GuildOptionId",
                principalTable: "GuildOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_GuildOptions_GuildOptionId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_GuildOptions_GuildOptionId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "GuildTeamMembers");

            migrationBuilder.DropTable(
                name: "GuildTeams");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_GuildOptionId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tags_GuildOptionId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "GuildOptionId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "GuildOptionId",
                table: "Tags");

            migrationBuilder.AddColumn<ulong>(
                name: "GuildId",
                table: "Tickets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);
        }
    }
}
