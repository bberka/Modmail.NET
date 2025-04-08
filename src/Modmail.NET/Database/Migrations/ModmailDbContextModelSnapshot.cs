﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modmail.NET.Database;

#nullable disable

namespace Modmail.NET.Migrations
{
    [DbContext(typeof(ModmailDbContext))]
    partial class ModmailDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Modmail.NET.Database.Entities.DiscordUserInfo", b =>
                {
                    b.Property<decimal>("Id")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("AvatarUrl")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("BannerUrl")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("Email")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Locale")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdateDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("Id");

                    b.ToTable("DiscordUserInfos", t =>
                        {
                            t.HasCheckConstraint("CK_DiscordUserInfos_Username_MinLength", "LEN([Username]) >= 1");
                        });
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.GuildOption", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<bool>("AlwaysAnonymous")
                        .HasColumnType("bit");

                    b.Property<string>("BannerUrl")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<decimal>("CategoryId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("IconUrl")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<decimal>("LogChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("ManageBlacklistMinAccessLevel")
                        .HasColumnType("int");

                    b.Property<int>("ManageHangfireMinAccessLevel")
                        .HasColumnType("int");

                    b.Property<int>("ManageTeamsMinAccessLevel")
                        .HasColumnType("int");

                    b.Property<int>("ManageTicketMinAccessLevel")
                        .HasColumnType("int");

                    b.Property<int>("ManageTicketTypeMinAccessLevel")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<bool>("PublicTranscripts")
                        .HasColumnType("bit");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<bool>("SendTranscriptLinkToUser")
                        .HasColumnType("bit");

                    b.Property<int>("StatisticsCalculateDays")
                        .HasColumnType("int");

                    b.Property<bool>("TakeFeedbackAfterClosing")
                        .HasColumnType("bit");

                    b.Property<int>("TicketDataDeleteWaitDays")
                        .HasColumnType("int");

                    b.Property<long>("TicketTimeoutHours")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UpdateDateUtc")
                        .HasColumnType("datetime2");

                    b.HasKey("GuildId");

                    b.ToTable("GuildOptions", t =>
                        {
                            t.HasCheckConstraint("CK_GuildOptions_Name_MinLength", "LEN([Name]) >= 1");

                            t.HasCheckConstraint("CK_GuildOptions_StatisticsCalculateDays_Range", "[StatisticsCalculateDays] BETWEEN 30 AND 365");

                            t.HasCheckConstraint("CK_GuildOptions_TicketDataDeleteWaitDays_Range", "[TicketDataDeleteWaitDays] BETWEEN -1 AND 365");
                        });
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.GuildTeam", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("AllowAccessToWebPanel")
                        .HasColumnType("bit");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("PermissionLevel")
                        .HasColumnType("int");

                    b.Property<bool>("PingOnNewMessage")
                        .HasColumnType("bit");

                    b.Property<bool>("PingOnNewTicket")
                        .HasColumnType("bit");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdateDateUtc")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("GuildTeams", t =>
                        {
                            t.HasCheckConstraint("CK_GuildTeams_Name_MinLength", "LEN([Name]) >= 1");
                        });
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.GuildTeamMember", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GuildTeamId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Key")
                        .HasColumnType("decimal(20,0)");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GuildTeamId");

                    b.ToTable("GuildTeamMembers");
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.Statistic", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("AvgResponseTimeSeconds")
                        .HasPrecision(2)
                        .HasColumnType("float(2)");

                    b.Property<double>("AvgTicketClosedSeconds")
                        .HasPrecision(2)
                        .HasColumnType("float(2)");

                    b.Property<double>("AvgTicketsClosedPerDay")
                        .HasPrecision(2)
                        .HasColumnType("float(2)");

                    b.Property<double>("AvgTicketsOpenedPerDay")
                        .HasPrecision(2)
                        .HasColumnType("float(2)");

                    b.Property<double>("FastestClosedTicketSeconds")
                        .HasPrecision(2)
                        .HasColumnType("float(2)");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<double>("SlowestClosedTicketSeconds")
                        .HasPrecision(2)
                        .HasColumnType("float(2)");

                    b.HasKey("Id");

                    b.ToTable("Statistics");
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.Ticket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Anonymous")
                        .HasColumnType("bit");

                    b.Property<decimal?>("AssignedUserId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("BotTicketCreatedMessageInDmId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("CloseReason")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<DateTime?>("ClosedDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("CloserUserId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("FeedbackMessage")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int?>("FeedbackStar")
                        .HasColumnType("int");

                    b.Property<decimal>("InitialMessageId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<bool>("IsForcedClosed")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastMessageDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("ModMessageChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("OpenerUserId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<decimal>("PrivateMessageChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("TicketTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AssignedUserId");

                    b.HasIndex("CloserUserId");

                    b.HasIndex("OpenerUserId");

                    b.HasIndex("TicketTypeId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.TicketBlacklist", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("DiscordUserId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("Reason")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DiscordUserId")
                        .IsUnique();

                    b.ToTable("TicketBlacklists");
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.TicketMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("BotMessageId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("ChangeStatus")
                        .HasColumnType("int");

                    b.Property<string>("MessageContent")
                        .HasMaxLength(2147483647)
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("MessageDiscordId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("SenderUserId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<bool>("SentByMod")
                        .HasColumnType("bit");

                    b.Property<Guid>("TicketId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("SenderUserId");

                    b.HasIndex("TicketId");

                    b.ToTable("TicketMessages");
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.TicketMessageAttachment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("FileSize")
                        .HasColumnType("int");

                    b.Property<int?>("Height")
                        .HasColumnType("int");

                    b.Property<string>("MediaType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ProxyUrl")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<Guid>("TicketMessageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<int?>("Width")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TicketMessageId");

                    b.ToTable("TicketMessageAttachments", t =>
                        {
                            t.HasCheckConstraint("CK_TicketMessageAttachments_FileName_MinLength", "LEN([FileName]) >= 1");

                            t.HasCheckConstraint("CK_TicketMessageAttachments_MediaType_MinLength", "LEN([MediaType]) >= 1");

                            t.HasCheckConstraint("CK_TicketMessageAttachments_ProxyUrl_MinLength", "LEN([ProxyUrl]) >= 1");

                            t.HasCheckConstraint("CK_TicketMessageAttachments_Url_MinLength", "LEN([Url]) >= 1");
                        });
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.TicketMessageHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MessageContentAfter")
                        .HasMaxLength(2147483647)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MessageContentBefore")
                        .HasMaxLength(2147483647)
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("TicketMessageId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TicketMessageId");

                    b.ToTable("TicketMessageHistory");
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.TicketNote", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<decimal>("DiscordUserId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("TicketId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TicketId");

                    b.ToTable("TicketNotes", t =>
                        {
                            t.HasCheckConstraint("CK_TicketNotes_Content_MinLength", "LEN([Content]) >= 1");
                        });
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.TicketType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("EmbedMessageContent")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("EmbedMessageTitle")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Emoji")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdateDateUtc")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("TicketTypes", t =>
                        {
                            t.HasCheckConstraint("CK_TicketTypes_Description_MinLength", "LEN([Description]) >= 1");

                            t.HasCheckConstraint("CK_TicketTypes_Key_MinLength", "LEN([Key]) >= 1");

                            t.HasCheckConstraint("CK_TicketTypes_Name_MinLength", "LEN([Name]) >= 1");
                        });
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.GuildTeamMember", b =>
                {
                    b.HasOne("Modmail.NET.Database.Entities.GuildTeam", "GuildTeam")
                        .WithMany("GuildTeamMembers")
                        .HasForeignKey("GuildTeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GuildTeam");
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.Ticket", b =>
                {
                    b.HasOne("Modmail.NET.Database.Entities.DiscordUserInfo", "AssignedUser")
                        .WithMany("AssignedTickets")
                        .HasForeignKey("AssignedUserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Modmail.NET.Database.Entities.DiscordUserInfo", "CloserUser")
                        .WithMany("ClosedTickets")
                        .HasForeignKey("CloserUserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Modmail.NET.Database.Entities.DiscordUserInfo", "OpenerUser")
                        .WithMany("OpenedTickets")
                        .HasForeignKey("OpenerUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Modmail.NET.Database.Entities.TicketType", "TicketType")
                        .WithMany()
                        .HasForeignKey("TicketTypeId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("AssignedUser");

                    b.Navigation("CloserUser");

                    b.Navigation("OpenerUser");

                    b.Navigation("TicketType");
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.TicketBlacklist", b =>
                {
                    b.HasOne("Modmail.NET.Database.Entities.DiscordUserInfo", "DiscordUser")
                        .WithMany()
                        .HasForeignKey("DiscordUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("DiscordUser");
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.TicketMessage", b =>
                {
                    b.HasOne("Modmail.NET.Database.Entities.DiscordUserInfo", null)
                        .WithMany()
                        .HasForeignKey("SenderUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Modmail.NET.Database.Entities.Ticket", null)
                        .WithMany("Messages")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.TicketMessageAttachment", b =>
                {
                    b.HasOne("Modmail.NET.Database.Entities.TicketMessage", null)
                        .WithMany("Attachments")
                        .HasForeignKey("TicketMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.TicketMessageHistory", b =>
                {
                    b.HasOne("Modmail.NET.Database.Entities.TicketMessage", "TicketMessage")
                        .WithMany("History")
                        .HasForeignKey("TicketMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TicketMessage");
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.TicketNote", b =>
                {
                    b.HasOne("Modmail.NET.Database.Entities.Ticket", null)
                        .WithMany("TicketNotes")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.DiscordUserInfo", b =>
                {
                    b.Navigation("AssignedTickets");

                    b.Navigation("ClosedTickets");

                    b.Navigation("OpenedTickets");
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.GuildTeam", b =>
                {
                    b.Navigation("GuildTeamMembers");
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.Ticket", b =>
                {
                    b.Navigation("Messages");

                    b.Navigation("TicketNotes");
                });

            modelBuilder.Entity("Modmail.NET.Database.Entities.TicketMessage", b =>
                {
                    b.Navigation("Attachments");

                    b.Navigation("History");
                });
#pragma warning restore 612, 618
        }
    }
}
