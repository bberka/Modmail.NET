﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modmail.NET.Database;

#nullable disable

namespace Modmail.NET.Migrations
{
    [DbContext(typeof(ModmailDbContext))]
    [Migration("20240318133122_guild_option_anon")]
    partial class guild_option_anon
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.28");

            modelBuilder.Entity("Modmail.NET.Entities.GuildOption", b =>
                {
                    b.Property<ulong>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("CategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClosingMessage")
                        .HasColumnType("TEXT");

                    b.Property<string>("GreetingMessage")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsSensitiveLogging")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("LogChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("ShowConfirmationWhenClosingTickets")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("TakeFeedbackAfterClosing")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("UpdateDate")
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

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateDate")
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

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GuildTeamId");

                    b.ToTable("GuildTeamMembers");
                });

            modelBuilder.Entity("Modmail.NET.Entities.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<ulong>("GuildOptionId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("MessageContent")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("UseEmbed")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GuildOptionId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Modmail.NET.Entities.Ticket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Anonymous")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("ClosedDate")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("DiscordUserId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("GuildOptionId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("InitialMessageId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsForcedClosed")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastMessageDate")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("ModMessageChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Priority")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("PrivateMessageChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GuildOptionId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketFeedback", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("TEXT");

                    b.Property<byte>("Stars")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("TicketId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("TicketId")
                        .IsUnique();

                    b.ToTable("TicketFeedbacks");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<ulong>("AuthorId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("MessageContent")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TicketId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

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

                    b.Property<DateTime>("RegisterDate")
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

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TicketId")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

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

            modelBuilder.Entity("Modmail.NET.Entities.Tag", b =>
                {
                    b.HasOne("Modmail.NET.Entities.GuildOption", "GuildOption")
                        .WithMany("Tags")
                        .HasForeignKey("GuildOptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GuildOption");
                });

            modelBuilder.Entity("Modmail.NET.Entities.Ticket", b =>
                {
                    b.HasOne("Modmail.NET.Entities.GuildOption", "GuildOption")
                        .WithMany("Tickets")
                        .HasForeignKey("GuildOptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GuildOption");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketFeedback", b =>
                {
                    b.HasOne("Modmail.NET.Entities.Ticket", "Ticket")
                        .WithOne("TicketFeedback")
                        .HasForeignKey("Modmail.NET.Entities.TicketFeedback", "TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketMessage", b =>
                {
                    b.HasOne("Modmail.NET.Entities.Ticket", null)
                        .WithMany("TicketMessages")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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
                    b.HasOne("Modmail.NET.Entities.Ticket", "Ticket")
                        .WithMany()
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("Modmail.NET.Entities.GuildOption", b =>
                {
                    b.Navigation("GuildTeams");

                    b.Navigation("Tags");

                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("Modmail.NET.Entities.GuildTeam", b =>
                {
                    b.Navigation("GuildTeamMembers");
                });

            modelBuilder.Entity("Modmail.NET.Entities.Ticket", b =>
                {
                    b.Navigation("TicketFeedback")
                        .IsRequired();

                    b.Navigation("TicketMessages");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketMessage", b =>
                {
                    b.Navigation("TicketMessageAttachments");
                });
#pragma warning restore 612, 618
        }
    }
}