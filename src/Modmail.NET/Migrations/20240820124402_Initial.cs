using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildOptions",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BannerUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    CategoryId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsSensitiveLogging = table.Column<bool>(type: "bit", nullable: false),
                    TicketTimeoutHours = table.Column<long>(type: "bigint", nullable: false),
                    GreetingMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClosingMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TakeFeedbackAfterClosing = table.Column<bool>(type: "bit", nullable: false),
                    ShowConfirmationWhenClosingTickets = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildOptions", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "TicketTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Emoji = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    EmbedMessageTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmbedMessageContent = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuildTeams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionLevel = table.Column<int>(type: "int", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    PingOnNewTicket = table.Column<bool>(type: "bit", nullable: false),
                    PingOnNewMessage = table.Column<bool>(type: "bit", nullable: false),
                    GuildOptionGuildId = table.Column<decimal>(type: "decimal(20,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuildTeams_GuildOptions_GuildOptionGuildId",
                        column: x => x.GuildOptionGuildId,
                        principalTable: "GuildOptions",
                        principalColumn: "GuildId");
                });

            migrationBuilder.CreateTable(
                name: "GuildTeamMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Key = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    GuildTeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                name: "DiscordUserInfos",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BannerUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Locale = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketBlacklistId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordUserInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketBlacklists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscordUserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    GuildOptionGuildId = table.Column<decimal>(type: "decimal(20,0)", nullable: true)
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
                        name: "FK_TicketBlacklists_GuildOptions_GuildOptionGuildId",
                        column: x => x.GuildOptionGuildId,
                        principalTable: "GuildOptions",
                        principalColumn: "GuildId");
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastMessageDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpenerUserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    CloserUserId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    PrivateMessageChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    ModMessageChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    InitialMessageId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    BotTicketCreatedMessageInDmId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CloseReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsForcedClosed = table.Column<bool>(type: "bit", nullable: false),
                    FeedbackStar = table.Column<int>(type: "int", nullable: true),
                    FeedbackMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Anonymous = table.Column<bool>(type: "bit", nullable: false),
                    TicketTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TicketTypeId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GuildOptionGuildId = table.Column<decimal>(type: "decimal(20,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_DiscordUserInfos_CloserUserId",
                        column: x => x.CloserUserId,
                        principalTable: "DiscordUserInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_DiscordUserInfos_OpenerUserId",
                        column: x => x.OpenerUserId,
                        principalTable: "DiscordUserInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_GuildOptions_GuildOptionGuildId",
                        column: x => x.GuildOptionGuildId,
                        principalTable: "GuildOptions",
                        principalColumn: "GuildId");
                    table.ForeignKey(
                        name: "FK_Tickets_TicketTypes_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "TicketTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketTypes_TicketTypeId1",
                        column: x => x.TicketTypeId1,
                        principalTable: "TicketTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TicketMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SenderUserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MessageDiscordId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketMessages_DiscordUserInfos_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "DiscordUserInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscordUserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketNotes", x => x.Id);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProxyUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    Width = table.Column<int>(type: "int", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<int>(type: "int", nullable: false),
                    MediaType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TicketMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                name: "IX_DiscordUserInfos_TicketBlacklistId",
                table: "DiscordUserInfos",
                column: "TicketBlacklistId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildTeamMembers_GuildTeamId",
                table: "GuildTeamMembers",
                column: "GuildTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildTeams_GuildOptionGuildId",
                table: "GuildTeams",
                column: "GuildOptionGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketBlacklists_DiscordUserId",
                table: "TicketBlacklists",
                column: "DiscordUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketBlacklists_GuildOptionGuildId",
                table: "TicketBlacklists",
                column: "GuildOptionGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketMessageAttachments_TicketMessageId",
                table: "TicketMessageAttachments",
                column: "TicketMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketMessages_SenderUserId",
                table: "TicketMessages",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketMessages_TicketId",
                table: "TicketMessages",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketNotes_TicketId",
                table: "TicketNotes",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CloserUserId",
                table: "Tickets",
                column: "CloserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_GuildOptionGuildId",
                table: "Tickets",
                column: "GuildOptionGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_OpenerUserId",
                table: "Tickets",
                column: "OpenerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketTypeId",
                table: "Tickets",
                column: "TicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketTypeId1",
                table: "Tickets",
                column: "TicketTypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscordUserInfos_TicketBlacklists_TicketBlacklistId",
                table: "DiscordUserInfos",
                column: "TicketBlacklistId",
                principalTable: "TicketBlacklists",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscordUserInfos_TicketBlacklists_TicketBlacklistId",
                table: "DiscordUserInfos");

            migrationBuilder.DropTable(
                name: "GuildTeamMembers");

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
                name: "TicketTypes");

            migrationBuilder.DropTable(
                name: "TicketBlacklists");

            migrationBuilder.DropTable(
                name: "DiscordUserInfos");

            migrationBuilder.DropTable(
                name: "GuildOptions");
        }
    }
}
