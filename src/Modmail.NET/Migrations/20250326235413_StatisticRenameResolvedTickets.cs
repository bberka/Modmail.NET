using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    /// <inheritdoc />
    public partial class StatisticRenameResolvedTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvgTicketResolvedSeconds",
                table: "Statistics",
                newName: "AvgTicketClosedSeconds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvgTicketClosedSeconds",
                table: "Statistics",
                newName: "AvgTicketResolvedSeconds");
        }
    }
}
