using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class _Ticket_Added_CloserUserInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "CloserUserInfoId",
                table: "Tickets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CloserUserInfoId",
                table: "Tickets",
                column: "CloserUserInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_DiscordUserInfos_CloserUserInfoId",
                table: "Tickets",
                column: "CloserUserInfoId",
                principalTable: "DiscordUserInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_DiscordUserInfos_CloserUserInfoId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_CloserUserInfoId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CloserUserInfoId",
                table: "Tickets");
        }
    }
}
