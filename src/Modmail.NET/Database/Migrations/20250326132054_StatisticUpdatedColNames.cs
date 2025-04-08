using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    /// <inheritdoc />
    public partial class StatisticUpdatedColNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SlowestClosedTicketMinutes",
                table: "Statistics",
                newName: "SlowestClosedTicketSeconds");

            migrationBuilder.RenameColumn(
                name: "FastestClosedTicketMinutes",
                table: "Statistics",
                newName: "FastestClosedTicketSeconds");

            migrationBuilder.RenameColumn(
                name: "AvgTicketResolvedMinutes",
                table: "Statistics",
                newName: "AvgTicketResolvedSeconds");

            migrationBuilder.RenameColumn(
                name: "AvgResponseTimeMinutes",
                table: "Statistics",
                newName: "AvgResponseTimeSeconds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SlowestClosedTicketSeconds",
                table: "Statistics",
                newName: "SlowestClosedTicketMinutes");

            migrationBuilder.RenameColumn(
                name: "FastestClosedTicketSeconds",
                table: "Statistics",
                newName: "FastestClosedTicketMinutes");

            migrationBuilder.RenameColumn(
                name: "AvgTicketResolvedSeconds",
                table: "Statistics",
                newName: "AvgTicketResolvedMinutes");

            migrationBuilder.RenameColumn(
                name: "AvgResponseTimeSeconds",
                table: "Statistics",
                newName: "AvgResponseTimeMinutes");
        }
    }
}
