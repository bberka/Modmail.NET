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
    [Migration("20240315192413_newtable_tags")]
    partial class newtable_tags
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.28");

            modelBuilder.Entity("Modmail.NET.Entities.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<ulong>("GuildId")
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

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Modmail.NET.Entities.Ticket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ClosedDate")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("DiscordUserId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("GuildId")
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

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<ulong>("AuthorId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("ChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MessageContent")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<ulong>("MessageId")
                        .HasColumnType("INTEGER");

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

            modelBuilder.Entity("Modmail.NET.Entities.TicketOption", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<ulong>("CategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsSensitiveLogging")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("LogChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TicketOptions");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketMessage", b =>
                {
                    b.HasOne("Modmail.NET.Entities.Ticket", null)
                        .WithMany("ModmailMessageLogs")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketMessageAttachment", b =>
                {
                    b.HasOne("Modmail.NET.Entities.TicketMessage", "TicketMessage")
                        .WithMany("TicketAttachments")
                        .HasForeignKey("TicketMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TicketMessage");
                });

            modelBuilder.Entity("Modmail.NET.Entities.Ticket", b =>
                {
                    b.Navigation("ModmailMessageLogs");
                });

            modelBuilder.Entity("Modmail.NET.Entities.TicketMessage", b =>
                {
                    b.Navigation("TicketAttachments");
                });
#pragma warning restore 612, 618
        }
    }
}
