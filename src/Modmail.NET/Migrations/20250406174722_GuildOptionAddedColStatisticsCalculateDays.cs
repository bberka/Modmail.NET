using Microsoft.EntityFrameworkCore.Migrations;

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
                defaultValue: 0);

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
