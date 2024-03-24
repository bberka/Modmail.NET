using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class update2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_DiscordUserInfos_CloserUserId",
                table: "Tickets");

            migrationBuilder.AlterColumn<ulong>(
                name: "CloserUserId",
                table: "Tickets",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<ulong>(
                name: "MessageDiscordId",
                table: "TicketMessages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GuildOptions",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IconUrl",
                table: "GuildOptions",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_DiscordUserInfos_CloserUserId",
                table: "Tickets",
                column: "CloserUserId",
                principalTable: "DiscordUserInfos",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_DiscordUserInfos_CloserUserId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "MessageDiscordId",
                table: "TicketMessages");

            migrationBuilder.AlterColumn<ulong>(
                name: "CloserUserId",
                table: "Tickets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GuildOptions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "IconUrl",
                table: "GuildOptions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_DiscordUserInfos_CloserUserId",
                table: "Tickets",
                column: "CloserUserId",
                principalTable: "DiscordUserInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
