using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modmail.NET.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Options",
                columns: table => new
                {
                    ServerId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    BannerUrl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    LogChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    CategoryId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    TicketTimeoutHours = table.Column<long>(type: "bigint", nullable: false),
                    TakeFeedbackAfterClosing = table.Column<bool>(type: "bit", nullable: false),
                    AlwaysAnonymous = table.Column<bool>(type: "bit", nullable: false),
                    PublicTranscripts = table.Column<bool>(type: "bit", nullable: false),
                    SendTranscriptLinkToUser = table.Column<bool>(type: "bit", nullable: false),
                    StatisticsCalculateDays = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Options", x => x.ServerId);
                    table.CheckConstraint("CK_Options_Name_MinLength", "LEN([Name]) >= 1");
                    table.CheckConstraint("CK_Options_StatisticsCalculateDays_Range", "[StatisticsCalculateDays] BETWEEN 30 AND 365");
                });

            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AvgResponseTimeSeconds = table.Column<double>(type: "float(2)", precision: 2, nullable: false),
                    AvgTicketsClosedPerDay = table.Column<double>(type: "float(2)", precision: 2, nullable: false),
                    AvgTicketsOpenedPerDay = table.Column<double>(type: "float(2)", precision: 2, nullable: false),
                    AvgTicketClosedSeconds = table.Column<double>(type: "float(2)", precision: 2, nullable: false),
                    FastestClosedTicketSeconds = table.Column<double>(type: "float(2)", precision: 2, nullable: false),
                    SlowestClosedTicketSeconds = table.Column<double>(type: "float(2)", precision: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.CheckConstraint("CK_Tags_Content_MinLength", "LEN([Content]) >= 1");
                    table.CheckConstraint("CK_Tags_Name_MinLength", "LEN([Name]) >= 1");
                    table.CheckConstraint("CK_Tags_Title_MinLength", "LEN([Title]) >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    PingOnNewTicket = table.Column<bool>(type: "bit", nullable: false),
                    PingOnNewMessage = table.Column<bool>(type: "bit", nullable: false),
                    SuperUserTeam = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.CheckConstraint("CK_Teams_Name_MinLength", "LEN([Name]) >= 1");
                });

            migrationBuilder.CreateTable(
                name: "TicketTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Emoji = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    EmbedMessageTitle = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EmbedMessageContent = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTypes", x => x.Id);
                    table.CheckConstraint("CK_TicketTypes_Description_MinLength", "LEN([Description]) >= 1");
                    table.CheckConstraint("CK_TicketTypes_Key_MinLength", "LEN([Key]) >= 1");
                    table.CheckConstraint("CK_TicketTypes_Name_MinLength", "LEN([Name]) >= 1");
                });

            migrationBuilder.CreateTable(
                name: "UserInformation",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    BannerUrl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Locale = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInformation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthPolicy = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamPermissions_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamUsers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Blacklists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    UserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    AuthorUserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blacklists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blacklists_UserInformation_AuthorUserId",
                        column: x => x.AuthorUserId,
                        principalTable: "UserInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Blacklists_UserInformation_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    AssignedUserId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    PrivateMessageChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    ModMessageChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    InitialMessageId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    BotTicketCreatedMessageInDmId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CloseReason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsForcedClosed = table.Column<bool>(type: "bit", nullable: false),
                    FeedbackStar = table.Column<int>(type: "int", nullable: true),
                    FeedbackMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Anonymous = table.Column<bool>(type: "bit", nullable: false),
                    TicketTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketTypes_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "TicketTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_UserInformation_AssignedUserId",
                        column: x => x.AssignedUserId,
                        principalTable: "UserInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_UserInformation_CloserUserId",
                        column: x => x.CloserUserId,
                        principalTable: "UserInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_UserInformation_OpenerUserId",
                        column: x => x.OpenerUserId,
                        principalTable: "UserInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BotMessageId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    SenderUserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    MessageDiscordId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SentByMod = table.Column<bool>(type: "bit", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChangeStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_UserInformation_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "UserInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketNotes", x => x.Id);
                    table.CheckConstraint("CK_TicketNotes_Content_MinLength", "LEN([Content]) >= 1");
                    table.ForeignKey(
                        name: "FK_TicketNotes_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketNotes_UserInformation_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TicketMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ProxyUrl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Height = table.Column<int>(type: "int", nullable: true),
                    Width = table.Column<int>(type: "int", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<int>(type: "int", nullable: false),
                    MediaType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageAttachments", x => x.Id);
                    table.CheckConstraint("CK_MessageAttachments_FileName_MinLength", "LEN([FileName]) >= 1");
                    table.CheckConstraint("CK_MessageAttachments_MediaType_MinLength", "LEN([MediaType]) >= 1");
                    table.CheckConstraint("CK_MessageAttachments_ProxyUrl_MinLength", "LEN([ProxyUrl]) >= 1");
                    table.CheckConstraint("CK_MessageAttachments_Url_MinLength", "LEN([Url]) >= 1");
                    table.ForeignKey(
                        name: "FK_MessageAttachments_Messages_TicketMessageId",
                        column: x => x.TicketMessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MessageContentBefore = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    MessageContentAfter = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    TicketMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageHistory_Messages_TicketMessageId",
                        column: x => x.TicketMessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blacklists_AuthorUserId",
                table: "Blacklists",
                column: "AuthorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Blacklists_UserId",
                table: "Blacklists",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageAttachments_TicketMessageId",
                table: "MessageAttachments",
                column: "TicketMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageHistory_TicketMessageId",
                table: "MessageHistory",
                column: "TicketMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderUserId",
                table: "Messages",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TagId",
                table: "Messages",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TicketId",
                table: "Messages",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamPermissions_TeamId_AuthPolicy",
                table: "TeamPermissions",
                columns: new[] { "TeamId", "AuthPolicy" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Name",
                table: "Teams",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamUsers_TeamId",
                table: "TeamUsers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamUsers_UserId",
                table: "TeamUsers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketNotes_TicketId",
                table: "TicketNotes",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketNotes_UserId",
                table: "TicketNotes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AssignedUserId",
                table: "Tickets",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CloserUserId",
                table: "Tickets",
                column: "CloserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_OpenerUserId",
                table: "Tickets",
                column: "OpenerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketTypeId",
                table: "Tickets",
                column: "TicketTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blacklists");

            migrationBuilder.DropTable(
                name: "MessageAttachments");

            migrationBuilder.DropTable(
                name: "MessageHistory");

            migrationBuilder.DropTable(
                name: "Options");

            migrationBuilder.DropTable(
                name: "Statistics");

            migrationBuilder.DropTable(
                name: "TeamPermissions");

            migrationBuilder.DropTable(
                name: "TeamUsers");

            migrationBuilder.DropTable(
                name: "TicketNotes");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "TicketTypes");

            migrationBuilder.DropTable(
                name: "UserInformation");
        }
    }
}
