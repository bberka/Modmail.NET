using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Static;

namespace Modmail.NET.Database;

public partial class ModmailDbContext : DbContext
{
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    switch (MMConfig.This.DbType) {
      case DbType.Sqlite:
        optionsBuilder.UseSqlite(MMConfig.This.DbConnectionString);
        break;
      case DbType.Postgres:
        optionsBuilder.UseNpgsql(MMConfig.This.DbConnectionString);
        break;
      case DbType.MsSql:
        optionsBuilder.UseSqlServer(MMConfig.This.DbConnectionString);
        break;
      case DbType.MySql:
        optionsBuilder.UseMySql(MMConfig.This.DbConnectionString, ServerVersion.AutoDetect(MMConfig.This.DbConnectionString));
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
  }


  public DbSet<Ticket> Tickets { get; set; }
  public DbSet<TicketMessageAttachment> TicketMessageAttachments { get; set; }
  public DbSet<TicketMessage> TicketMessages { get; set; }
  public DbSet<TicketOption> TicketOptions { get; set; }
}