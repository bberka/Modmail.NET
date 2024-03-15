using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class update_ticketoption : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsListenPrivateMessages",
                table: "TicketOptions",
                newName: "IsSensitiveLogging");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "TicketOptions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisterDate",
                table: "TicketOptions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "TicketOptions",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "TicketOptions");

            migrationBuilder.DropColumn(
                name: "RegisterDate",
                table: "TicketOptions");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "TicketOptions");

            migrationBuilder.RenameColumn(
                name: "IsSensitiveLogging",
                table: "TicketOptions",
                newName: "IsListenPrivateMessages");
        }
    }
}
