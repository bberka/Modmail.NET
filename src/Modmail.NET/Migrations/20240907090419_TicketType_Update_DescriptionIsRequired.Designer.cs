﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modmail.NET.Database;

#nullable disable

namespace Modmail.NET.Migrations
{
    [DbContext(typeof(ModmailDbContext))]
    [Migration("20240907090419_TicketType_Update_DescriptionIsRequired")]
    partial class TicketType_Update_DescriptionIsRequired
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.28")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Modmail.NET.Entities.DiscordUserInfo", b =>
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

                    b.ToTable("DiscordUserInfos");
                });

            modelBuilder.Entity("Modmail.NET.Entities.GuildOption", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("BannerUrl")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<decimal>("CategoryId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("IconUrl")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSensitiveLogging")
                        .HasColumnType("bit");

                    b.Property<decimal>("LogChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<bool>("ShowConfirmationWhenClosingTickets")
                        .HasColumnType("bit");

                    b.Property<bool>("TakeFeedbackAfterClosing")
                        .HasColumnType("bit");

                    b.Property<long>("TicketTimeoutHours")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UpdateDateUtc")
                        .HasColumnType("datetime2");

                    b.HasKey("GuildId");

                    b.ToTable("GuildOptions");
                });

            modelBuilder.Entity("Modmail.NET.Entities.GuildTeam", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

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

                    b.ToTable("GuildTeams");
                });

            modelBuilder.Entity("Modmail.NET.Entities.GuildTeamMember", b =>
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

            modelBuilder.Entity("Modmail.NET.Entities.Ticket", b =>
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

            modelBuilder.Entity("Modmail.NET.Entities.TicketBlacklist", b =>
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

            modelBuilder.Entity("Modmail.NET.Entities.TicketMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MessageContent")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<decimal>("MessageDiscordId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("SenderUserId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<Guid>("TicketId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("SenderUserId");

                    b.HasIndex("TicketId");

                    b.ToTable("TicketMessages");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketMessageAttachment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte[]>("Content")
                        .HasColumnType("varbinary(max)");

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

                    b.ToTable("TicketMessageAttachments");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketNote", b =>
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

                    b.ToTable("TicketNotes");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketType", b =>
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

                    b.ToTable("TicketTypes");
                });

            modelBuilder.Entity("Modmail.NET.Entities.GuildTeamMember", b =>
                {
                    b.HasOne("Modmail.NET.Entities.GuildTeam", "GuildTeam")
                        .WithMany("GuildTeamMembers")
                        .HasForeignKey("GuildTeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GuildTeam");
                });

            modelBuilder.Entity("Modmail.NET.Entities.Ticket", b =>
                {
                    b.HasOne("Modmail.NET.Entities.DiscordUserInfo", "AssignedUser")
                        .WithMany("AssignedTickets")
                        .HasForeignKey("AssignedUserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Modmail.NET.Entities.DiscordUserInfo", "CloserUser")
                        .WithMany("ClosedTickets")
                        .HasForeignKey("CloserUserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Modmail.NET.Entities.DiscordUserInfo", "OpenerUser")
                        .WithMany("OpenedTickets")
                        .HasForeignKey("OpenerUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Modmail.NET.Entities.TicketType", "TicketType")
                        .WithMany()
                        .HasForeignKey("TicketTypeId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("AssignedUser");

                    b.Navigation("CloserUser");

                    b.Navigation("OpenerUser");

                    b.Navigation("TicketType");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketBlacklist", b =>
                {
                    b.HasOne("Modmail.NET.Entities.DiscordUserInfo", "DiscordUser")
                        .WithMany()
                        .HasForeignKey("DiscordUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("DiscordUser");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketMessage", b =>
                {
                    b.HasOne("Modmail.NET.Entities.DiscordUserInfo", null)
                        .WithMany()
                        .HasForeignKey("SenderUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Modmail.NET.Entities.Ticket", null)
                        .WithMany("Messages")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketMessageAttachment", b =>
                {
                    b.HasOne("Modmail.NET.Entities.TicketMessage", null)
                        .WithMany("Attachments")
                        .HasForeignKey("TicketMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketNote", b =>
                {
                    b.HasOne("Modmail.NET.Entities.Ticket", null)
                        .WithMany("TicketNotes")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Modmail.NET.Entities.DiscordUserInfo", b =>
                {
                    b.Navigation("AssignedTickets");

                    b.Navigation("ClosedTickets");

                    b.Navigation("OpenedTickets");
                });

            modelBuilder.Entity("Modmail.NET.Entities.GuildTeam", b =>
                {
                    b.Navigation("GuildTeamMembers");
                });

            modelBuilder.Entity("Modmail.NET.Entities.Ticket", b =>
                {
                    b.Navigation("Messages");

                    b.Navigation("TicketNotes");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketMessage", b =>
                {
                    b.Navigation("Attachments");
                });
#pragma warning restore 612, 618
        }
    }
}