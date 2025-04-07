using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    /// <inheritdoc />
    public partial class GuildOption_AddedCol_PermissionLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManageBlacklistMinAccessLevel",
                table: "GuildOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ManageGuildOptionMinAccessLevel",
                table: "GuildOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ManageHangfireMinAccessLevel",
                table: "GuildOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ManageTeamsMinAccessLevel",
                table: "GuildOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ManageTicketMinAccessLevel",
                table: "GuildOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ManageTicketTypeMinAccessLevel",
                table: "GuildOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManageBlacklistMinAccessLevel",
                table: "GuildOptions");

            migrationBuilder.DropColumn(
                name: "ManageGuildOptionMinAccessLevel",
                table: "GuildOptions");

            migrationBuilder.DropColumn(
                name: "ManageHangfireMinAccessLevel",
                table: "GuildOptions");

            migrationBuilder.DropColumn(
                name: "ManageTeamsMinAccessLevel",
                table: "GuildOptions");

            migrationBuilder.DropColumn(
                name: "ManageTicketMinAccessLevel",
                table: "GuildOptions");

            migrationBuilder.DropColumn(
                name: "ManageTicketTypeMinAccessLevel",
                table: "GuildOptions");
        }
    }
}
