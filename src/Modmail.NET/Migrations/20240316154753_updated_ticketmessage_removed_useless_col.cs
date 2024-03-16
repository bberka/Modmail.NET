using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class updated_ticketmessage_removed_useless_col : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "TicketMessages");

            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "TicketMessages");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "TicketMessages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "ChannelId",
                table: "TicketMessages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "GuildId",
                table: "TicketMessages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "MessageId",
                table: "TicketMessages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);
        }
    }
}
