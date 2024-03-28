using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class mg1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscordUserInfos",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegisterDateUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    AvatarUrl = table.Column<string>(type: "TEXT", nullable: true),
                    BannerUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Locale = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordUserInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuildOptions",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IconUrl = table.Column<string>(type: "TEXT", nullable: false),
                    BannerUrl = table.Column<string>(type: "TEXT", nullable: true),
                    LogChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsSensitiveLogging = table.Column<bool>(type: "INTEGER", nullable: false),
                    GreetingMessage = table.Column<string>(type: "TEXT", nullable: false),
                    ClosingMessage = table.Column<string>(type: "TEXT", nullable: false),
                    TakeFeedbackAfterClosing = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowConfirmationWhenClosingTickets = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildOptions", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "TicketTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Emoji = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    EmbedMessageTitle = table.Column<string>(type: "TEXT", nullable: false),
                    EmbedMessageContent = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuildTeams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PermissionLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    GuildOptionId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuildTeams_GuildOptions_GuildOptionId",
                        column: x => x.GuildOptionId,
                        principalTable: "GuildOptions",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketBlacklists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: true),
                    DiscordUserId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketBlacklists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketBlacklists_DiscordUserInfos_DiscordUserId",
                        column: x => x.DiscordUserId,
                        principalTable: "DiscordUserInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketBlacklists_GuildOptions_GuildId",
                        column: x => x.GuildId,
                        principalTable: "GuildOptions",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastMessageDateUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ClosedDateUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    OpenerUserId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    CloserUserId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    PrivateMessageChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ModMessageChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    InitialMessageId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    BotTicketCreatedMessageInDmId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    CloseReason = table.Column<string>(type: "TEXT", nullable: true),
                    IsForcedClosed = table.Column<bool>(type: "INTEGER", nullable: false),
                    GuildOptionId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    FeedbackStar = table.Column<int>(type: "INTEGER", nullable: true),
                    FeedbackMessage = table.Column<string>(type: "TEXT", nullable: true),
                    Anonymous = table.Column<bool>(type: "INTEGER", nullable: false),
                    TicketTypeId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_DiscordUserInfos_CloserUserId",
                        column: x => x.CloserUserId,
                        principalTable: "DiscordUserInfos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tickets_DiscordUserInfos_OpenerUserId",
                        column: x => x.OpenerUserId,
                        principalTable: "DiscordUserInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_GuildOptions_GuildOptionId",
                        column: x => x.GuildOptionId,
                        principalTable: "GuildOptions",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketTypes_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "TicketTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GuildTeamMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Key = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    GuildTeamId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildTeamMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuildTeamMembers_GuildTeams_GuildTeamId",
                        column: x => x.GuildTeamId,
                        principalTable: "GuildTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DiscordUserInfoId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MessageContent = table.Column<string>(type: "TEXT", nullable: false),
                    MessageDiscordId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    TicketId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketMessages_DiscordUserInfos_DiscordUserInfoId",
                        column: x => x.DiscordUserInfoId,
                        principalTable: "DiscordUserInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketMessages_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    TicketId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DiscordUserId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketNotes_DiscordUserInfos_DiscordUserId",
                        column: x => x.DiscordUserId,
                        principalTable: "DiscordUserInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketNotes_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketMessageAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    ProxyUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Height = table.Column<int>(type: "INTEGER", nullable: true),
                    Width = table.Column<int>(type: "INTEGER", nullable: true),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<int>(type: "INTEGER", nullable: false),
                    MediaType = table.Column<string>(type: "TEXT", nullable: false),
                    TicketMessageId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketMessageAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketMessageAttachments_TicketMessages_TicketMessageId",
                        column: x => x.TicketMessageId,
                        principalTable: "TicketMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuildTeamMembers_GuildTeamId",
                table: "GuildTeamMembers",
                column: "GuildTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildTeams_GuildOptionId",
                table: "GuildTeams",
                column: "GuildOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketBlacklists_DiscordUserId_GuildId",
                table: "TicketBlacklists",
                columns: new[] { "DiscordUserId", "GuildId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketBlacklists_GuildId",
                table: "TicketBlacklists",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketMessageAttachments_TicketMessageId",
                table: "TicketMessageAttachments",
                column: "TicketMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketMessages_DiscordUserInfoId",
                table: "TicketMessages",
                column: "DiscordUserInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketMessages_TicketId",
                table: "TicketMessages",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketNotes_DiscordUserId",
                table: "TicketNotes",
                column: "DiscordUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketNotes_TicketId",
                table: "TicketNotes",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CloserUserId",
                table: "Tickets",
                column: "CloserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_GuildOptionId",
                table: "Tickets",
                column: "GuildOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_OpenerUserId",
                table: "Tickets",
                column: "OpenerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketTypeId",
                table: "Tickets",
                column: "TicketTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildTeamMembers");

            migrationBuilder.DropTable(
                name: "TicketBlacklists");

            migrationBuilder.DropTable(
                name: "TicketMessageAttachments");

            migrationBuilder.DropTable(
                name: "TicketNotes");

            migrationBuilder.DropTable(
                name: "GuildTeams");

            migrationBuilder.DropTable(
                name: "TicketMessages");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "DiscordUserInfos");

            migrationBuilder.DropTable(
                name: "GuildOptions");

            migrationBuilder.DropTable(
                name: "TicketTypes");
        }
    }
}
