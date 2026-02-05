using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class Ticket_AssignedUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AssignedUserId",
                table: "Tickets",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AssignedUserId",
                table: "Tickets",
                column: "AssignedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_DiscordUserInfos_AssignedUserId",
                table: "Tickets",
                column: "AssignedUserId",
                principalTable: "DiscordUserInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_DiscordUserInfos_AssignedUserId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_AssignedUserId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "AssignedUserId",
                table: "Tickets");
        }
    }
}
