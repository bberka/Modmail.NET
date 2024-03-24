using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class _TicketType_Added_EmbedMessageTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Message",
                table: "TicketTypes",
                newName: "EmbedMessageTitle");

            migrationBuilder.AddColumn<string>(
                name: "EmbedMessageContent",
                table: "TicketTypes",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmbedMessageContent",
                table: "TicketTypes");

            migrationBuilder.RenameColumn(
                name: "EmbedMessageTitle",
                table: "TicketTypes",
                newName: "Message");
        }
    }
}
