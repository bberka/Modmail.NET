using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    /// <inheritdoc />
    public partial class GuildOptionConstraintFixMinValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_GuildOptions_StatisticsCalculateDays_Range",
                table: "GuildOptions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_GuildOptions_TicketDataDeleteWaitDays_Range",
                table: "GuildOptions");

            migrationBuilder.AddCheckConstraint(
                name: "CK_GuildOptions_StatisticsCalculateDays_Range",
                table: "GuildOptions",
                sql: "[StatisticsCalculateDays] BETWEEN -1 AND 365");

            migrationBuilder.AddCheckConstraint(
                name: "CK_GuildOptions_TicketDataDeleteWaitDays_Range",
                table: "GuildOptions",
                sql: "[TicketDataDeleteWaitDays] BETWEEN -1 AND 365");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_GuildOptions_StatisticsCalculateDays_Range",
                table: "GuildOptions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_GuildOptions_TicketDataDeleteWaitDays_Range",
                table: "GuildOptions");

            migrationBuilder.AddCheckConstraint(
                name: "CK_GuildOptions_StatisticsCalculateDays_Range",
                table: "GuildOptions",
                sql: "[StatisticsCalculateDays] BETWEEN 30 AND 365");

            migrationBuilder.AddCheckConstraint(
                name: "CK_GuildOptions_TicketDataDeleteWaitDays_Range",
                table: "GuildOptions",
                sql: "[TicketDataDeleteWaitDays] BETWEEN 1 AND 365");
        }
    }
}
