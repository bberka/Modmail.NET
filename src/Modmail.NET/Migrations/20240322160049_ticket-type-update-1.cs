using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class tickettypeupdate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "TicketTypes");

            migrationBuilder.DropColumn(
                name: "ChannelNameTemplate",
                table: "TicketTypes");

            migrationBuilder.DropColumn(
                name: "ColorHexCode",
                table: "TicketTypes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "CategoryId",
                table: "TicketTypes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChannelNameTemplate",
                table: "TicketTypes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ColorHexCode",
                table: "TicketTypes",
                type: "TEXT",
                nullable: true);
        }
    }
}
