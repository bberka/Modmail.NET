using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class mg2_GuildTeam_Ping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PingOnNewMessage",
                table: "GuildTeams",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PingOnNewTicket",
                table: "GuildTeams",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PingOnNewMessage",
                table: "GuildTeams");

            migrationBuilder.DropColumn(
                name: "PingOnNewTicket",
                table: "GuildTeams");
        }
    }
}
