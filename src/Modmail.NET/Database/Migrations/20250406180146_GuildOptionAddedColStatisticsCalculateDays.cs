using Microsoft.EntityFrameworkCore.Migrations;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.Metric.Static;

#nullable disable

namespace Modmail.NET.Migrations
{
    /// <inheritdoc />
    public partial class GuildOptionAddedColStatisticsCalculateDays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatisticsCalculateDays",
                table: "GuildOptions",
                type: "int",
                nullable: false,
                defaultValue: MetricConstants.DefaultStatisticsCalculateDays);

            migrationBuilder.AddCheckConstraint(
                name: "CK_GuildOptions_StatisticsCalculateDays_Range",
                table: "GuildOptions",
                sql: "[StatisticsCalculateDays] BETWEEN 30 AND 365");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_GuildOptions_StatisticsCalculateDays_Range",
                table: "GuildOptions");

            migrationBuilder.DropColumn(
                name: "StatisticsCalculateDays",
                table: "GuildOptions");
        }
    }
}
