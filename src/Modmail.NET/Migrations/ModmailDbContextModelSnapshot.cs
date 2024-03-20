﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
            modelBuilder.HasAnnotation("ProductVersion", "6.0.28");

            modelBuilder.Entity("Modmail.NET.Entities.DiscordUserInfo", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AvatarUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("BannerUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("Locale")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateDateUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("DiscordUserInfos");
                });

            modelBuilder.Entity("Modmail.NET.Entities.GuildOption", b =>
                {
                    b.Property<ulong>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("CategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClosingMessage")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("GreetingMessage")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsSensitiveLogging")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("LogChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("TEXT");

                    b.Property<bool>("ShowConfirmationWhenClosingTickets")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("TakeFeedbackAfterClosing")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("UpdateDateUtc")
                        .HasColumnType("TEXT");

                    b.HasKey("GuildId");

                    b.ToTable("GuildOptions");
                });

            modelBuilder.Entity("Modmail.NET.Entities.GuildTeam", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<ulong>("GuildOptionId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("PermissionLevel")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateDateUtc")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GuildOptionId");

                    b.ToTable("GuildTeams");
                });

            modelBuilder.Entity("Modmail.NET.Entities.GuildTeamMember", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("GuildTeamId")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("Key")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("UpdateDateUtc")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GuildTeamId");

                    b.ToTable("GuildTeamMembers");
                });

            modelBuilder.Entity("Modmail.NET.Entities.Ticket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Anonymous")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CloseReason")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ClosedDateUtc")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("DiscordUserInfoId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FeedbackMessage")
                        .HasColumnType("TEXT");

                    b.Property<int?>("FeedbackStar")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("GuildOptionId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("InitialMessageId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsForcedClosed")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastMessageDateUtc")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("ModMessageChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Priority")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("PrivateMessageChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DiscordUserInfoId");

                    b.HasIndex("GuildOptionId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketBlacklist", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<ulong>("DiscordUserInfoId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("GuildOptionId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Reason")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GuildOptionId");

                    b.HasIndex("DiscordUserInfoId", "GuildOptionId")
                        .IsUnique();

                    b.ToTable("TicketBlacklists");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<ulong>("DiscordUserInfoId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MessageContent")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TicketId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DiscordUserInfoId");

                    b.HasIndex("TicketId");

                    b.ToTable("TicketMessages");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketMessageAttachment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Content")
                        .HasColumnType("BLOB");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("FileSize")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Height")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MediaType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ProxyUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TicketMessageId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("Width")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TicketMessageId");

                    b.ToTable("TicketMessageAttachments");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketNote", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<ulong>("DiscordUserInfoId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("RegisterDateUtc")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TicketId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DiscordUserInfoId");

                    b.HasIndex("TicketId");

                    b.ToTable("TicketNotes");
                });

            modelBuilder.Entity("Modmail.NET.Entities.GuildTeam", b =>
                {
                    b.HasOne("Modmail.NET.Entities.GuildOption", "GuildOption")
                        .WithMany("GuildTeams")
                        .HasForeignKey("GuildOptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GuildOption");
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
                    b.HasOne("Modmail.NET.Entities.DiscordUserInfo", "DiscordUserInfo")
                        .WithMany()
                        .HasForeignKey("DiscordUserInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Modmail.NET.Entities.GuildOption", "GuildOption")
                        .WithMany("Tickets")
                        .HasForeignKey("GuildOptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DiscordUserInfo");

                    b.Navigation("GuildOption");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketBlacklist", b =>
                {
                    b.HasOne("Modmail.NET.Entities.DiscordUserInfo", "DiscordUserInfo")
                        .WithMany("TicketBlacklists")
                        .HasForeignKey("DiscordUserInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Modmail.NET.Entities.GuildOption", "GuildOption")
                        .WithMany()
                        .HasForeignKey("GuildOptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DiscordUserInfo");

                    b.Navigation("GuildOption");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketMessage", b =>
                {
                    b.HasOne("Modmail.NET.Entities.DiscordUserInfo", "DiscordUserInfo")
                        .WithMany()
                        .HasForeignKey("DiscordUserInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Modmail.NET.Entities.Ticket", null)
                        .WithMany("TicketMessages")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DiscordUserInfo");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketMessageAttachment", b =>
                {
                    b.HasOne("Modmail.NET.Entities.TicketMessage", "TicketMessage")
                        .WithMany("TicketMessageAttachments")
                        .HasForeignKey("TicketMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TicketMessage");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketNote", b =>
                {
                    b.HasOne("Modmail.NET.Entities.DiscordUserInfo", "DiscordUserInfo")
                        .WithMany()
                        .HasForeignKey("DiscordUserInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Modmail.NET.Entities.Ticket", "Ticket")
                        .WithMany("TicketNotes")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DiscordUserInfo");

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("Modmail.NET.Entities.DiscordUserInfo", b =>
                {
                    b.Navigation("TicketBlacklists");
                });

            modelBuilder.Entity("Modmail.NET.Entities.GuildOption", b =>
                {
                    b.Navigation("GuildTeams");

                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("Modmail.NET.Entities.GuildTeam", b =>
                {
                    b.Navigation("GuildTeamMembers");
                });

            modelBuilder.Entity("Modmail.NET.Entities.Ticket", b =>
                {
                    b.Navigation("TicketMessages");

                    b.Navigation("TicketNotes");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketMessage", b =>
                {
                    b.Navigation("TicketMessageAttachments");
                });
#pragma warning restore 612, 618
        }
    }
}
