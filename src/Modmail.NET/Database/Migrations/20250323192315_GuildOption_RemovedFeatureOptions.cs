using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    /// <inheritdoc />
    public partial class GuildOption_RemovedFeatureOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnableDiscordChannelLogging",
                table: "GuildOptions");

            migrationBuilder.DropColumn(
                name: "IsSensitiveLogging",
                table: "GuildOptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEnableDiscordChannelLogging",
                table: "GuildOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSensitiveLogging",
                table: "GuildOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
