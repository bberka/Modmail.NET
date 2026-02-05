using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    /// <inheritdoc />
    public partial class StatisticTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvgResponseTimeMinutes",
                table: "GuildOptions");

            migrationBuilder.DropColumn(
                name: "AvgTicketsClosePerDay",
                table: "GuildOptions");

            migrationBuilder.DropColumn(
                name: "AvgTicketsOpenPerDay",
                table: "GuildOptions");

            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AvgResponseTimeMinutes = table.Column<double>(type: "float(2)", precision: 2, nullable: false),
                    AvgTicketsClosedPerDay = table.Column<double>(type: "float(2)", precision: 2, nullable: false),
                    AvgTicketsOpenedPerDay = table.Column<double>(type: "float(2)", precision: 2, nullable: false),
                    AvgTicketResolvedMinutes = table.Column<double>(type: "float(2)", precision: 2, nullable: false),
                    FastestClosedTicketMinutes = table.Column<double>(type: "float(2)", precision: 2, nullable: false),
                    SlowestClosedTicketMinutes = table.Column<double>(type: "float(2)", precision: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Statistics");

            migrationBuilder.AddColumn<double>(
                name: "AvgResponseTimeMinutes",
                table: "GuildOptions",
                type: "float(2)",
                precision: 2,
                nullable: false,
                defaultValue: 0.0);

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
    }
}
