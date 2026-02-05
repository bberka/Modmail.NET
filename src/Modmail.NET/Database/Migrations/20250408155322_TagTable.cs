using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Database.Migrations
{
    /// <inheritdoc />
    public partial class TagTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Shortcut = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
