using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Database.Migrations
{
    /// <inheritdoc />
    public partial class TicketMessageHistoryTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicketMessageHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageContentBefore = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    MessageContentAfter = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    TicketMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketMessageHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketMessageHistory_TicketMessages_TicketMessageId",
                        column: x => x.TicketMessageId,
                        principalTable: "TicketMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketMessageHistory_TicketMessageId",
                table: "TicketMessageHistory",
                column: "TicketMessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketMessageHistory");
        }
    }
}
