using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class ticket_feedback : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketFeedbacks");

            migrationBuilder.AddColumn<string>(
                name: "FeedbackMessage",
                table: "Tickets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FeedbackStar",
                table: "Tickets",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeedbackMessage",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "FeedbackStar",
                table: "Tickets");

            migrationBuilder.CreateTable(
                name: "TicketFeedbacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TicketId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    RegisterDateUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Stars = table.Column<byte>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketFeedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketFeedbacks_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketFeedbacks_TicketId",
                table: "TicketFeedbacks",
                column: "TicketId",
                unique: true);
        }
    }
}
