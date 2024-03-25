using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class mg5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAutoUpdateGuildInformation",
                table: "GuildOptions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAutoUpdateGuildInformation",
                table: "GuildOptions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
