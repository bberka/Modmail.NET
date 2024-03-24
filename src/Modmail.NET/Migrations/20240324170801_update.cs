using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_DiscordUserInfos_CloserUserInfoId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_DiscordUserInfos_DiscordUserInfoId",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "DiscordUserId",
                table: "Tickets",
                newName: "OpenerUserId");

            migrationBuilder.RenameColumn(
                name: "CloserUserInfoId",
                table: "Tickets",
                newName: "CloserUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_DiscordUserInfoId",
                table: "Tickets",
                newName: "IX_Tickets_OpenerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_CloserUserInfoId",
                table: "Tickets",
                newName: "IX_Tickets_CloserUserId");

            migrationBuilder.AddColumn<string>(
                name: "BannerUrl",
                table: "GuildOptions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconUrl",
                table: "GuildOptions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoUpdateGuildInformation",
                table: "GuildOptions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "GuildOptions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_DiscordUserInfos_CloserUserId",
                table: "Tickets",
                column: "CloserUserId",
                principalTable: "DiscordUserInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_DiscordUserInfos_OpenerUserId",
                table: "Tickets",
                column: "OpenerUserId",
                principalTable: "DiscordUserInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_DiscordUserInfos_CloserUserId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_DiscordUserInfos_OpenerUserId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "BannerUrl",
                table: "GuildOptions");

            migrationBuilder.DropColumn(
                name: "IconUrl",
                table: "GuildOptions");

            migrationBuilder.DropColumn(
                name: "IsAutoUpdateGuildInformation",
                table: "GuildOptions");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "GuildOptions");

            migrationBuilder.RenameColumn(
                name: "OpenerUserId",
                table: "Tickets",
                newName: "DiscordUserId");

            migrationBuilder.RenameColumn(
                name: "CloserUserId",
                table: "Tickets",
                newName: "CloserUserInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_OpenerUserId",
                table: "Tickets",
                newName: "IX_Tickets_DiscordUserInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_CloserUserId",
                table: "Tickets",
                newName: "IX_Tickets_CloserUserInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_DiscordUserInfos_CloserUserInfoId",
                table: "Tickets",
                column: "CloserUserInfoId",
                principalTable: "DiscordUserInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_DiscordUserInfos_DiscordUserInfoId",
                table: "Tickets",
                column: "DiscordUserId",
                principalTable: "DiscordUserInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
