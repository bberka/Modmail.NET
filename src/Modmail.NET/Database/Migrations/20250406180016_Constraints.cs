using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    /// <inheritdoc />
    public partial class Constraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MessageContent",
                table: "TicketMessages",
                type: "nvarchar(max)",
                maxLength: 2147483647,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 2147483647);

            migrationBuilder.AddCheckConstraint(
                name: "CK_TicketTypes_Description_MinLength",
                table: "TicketTypes",
                sql: "LEN([Description]) >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TicketTypes_Key_MinLength",
                table: "TicketTypes",
                sql: "LEN([Key]) >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TicketTypes_Name_MinLength",
                table: "TicketTypes",
                sql: "LEN([Name]) >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TicketNotes_Content_MinLength",
                table: "TicketNotes",
                sql: "LEN([Content]) >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TicketMessageAttachments_FileName_MinLength",
                table: "TicketMessageAttachments",
                sql: "LEN([FileName]) >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TicketMessageAttachments_MediaType_MinLength",
                table: "TicketMessageAttachments",
                sql: "LEN([MediaType]) >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TicketMessageAttachments_ProxyUrl_MinLength",
                table: "TicketMessageAttachments",
                sql: "LEN([ProxyUrl]) >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TicketMessageAttachments_Url_MinLength",
                table: "TicketMessageAttachments",
                sql: "LEN([Url]) >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_GuildTeams_Name_MinLength",
                table: "GuildTeams",
                sql: "LEN([Name]) >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_GuildOptions_Name_MinLength",
                table: "GuildOptions",
                sql: "LEN([Name]) >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_GuildOptions_TicketDataDeleteWaitDays_Range",
                table: "GuildOptions",
                sql: "[TicketDataDeleteWaitDays] BETWEEN -1 AND 365");

            migrationBuilder.AddCheckConstraint(
                name: "CK_DiscordUserInfos_Username_MinLength",
                table: "DiscordUserInfos",
                sql: "LEN([Username]) >= 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_TicketTypes_Description_MinLength",
                table: "TicketTypes");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TicketTypes_Key_MinLength",
                table: "TicketTypes");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TicketTypes_Name_MinLength",
                table: "TicketTypes");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TicketNotes_Content_MinLength",
                table: "TicketNotes");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TicketMessageAttachments_FileName_MinLength",
                table: "TicketMessageAttachments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TicketMessageAttachments_MediaType_MinLength",
                table: "TicketMessageAttachments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TicketMessageAttachments_ProxyUrl_MinLength",
                table: "TicketMessageAttachments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TicketMessageAttachments_Url_MinLength",
                table: "TicketMessageAttachments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_GuildTeams_Name_MinLength",
                table: "GuildTeams");

            migrationBuilder.DropCheckConstraint(
                name: "CK_GuildOptions_Name_MinLength",
                table: "GuildOptions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_GuildOptions_TicketDataDeleteWaitDays_Range",
                table: "GuildOptions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_DiscordUserInfos_Username_MinLength",
                table: "DiscordUserInfos");

            migrationBuilder.AlterColumn<string>(
                name: "MessageContent",
                table: "TicketMessages",
                type: "nvarchar(max)",
                maxLength: 2147483647,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 2147483647,
                oldNullable: true);
        }
    }
}
