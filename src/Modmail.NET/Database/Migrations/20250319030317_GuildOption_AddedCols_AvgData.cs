using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    /// <inheritdoc />
    public partial class GuildOption_AddedCols_AvgData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AvgTicketsClosePerDay",
                table: "GuildOptions",
                type: "float(2)",
                precision: 2,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AvgTicketsOpenPerDay",
                table: "GuildOptions",
                type: "float(2)",
                precision: 2,
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvgTicketsClosePerDay",
                table: "GuildOptions");

            migrationBuilder.DropColumn(
                name: "AvgTicketsOpenPerDay",
                table: "GuildOptions");
        }
    }
}
