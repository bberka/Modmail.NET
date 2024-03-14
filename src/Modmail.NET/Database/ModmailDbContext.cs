using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Static;

namespace Modmail.NET.Database;

public class ModmailDbContext : DbContext
{
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    switch (EnvContainer.This.DbType) {
      case DbType.Sqlite:
        optionsBuilder.UseSqlite(EnvContainer.This.DbConnectionString);
        break;
      case DbType.Postgres:
        optionsBuilder.UseNpgsql(EnvContainer.This.DbConnectionString);
        break;
      case DbType.MsSql:
        optionsBuilder.UseSqlServer(EnvContainer.This.DbConnectionString);
        break;
      case DbType.MySql:
        optionsBuilder.UseMySql(EnvContainer.This.DbConnectionString, ServerVersion.AutoDetect(EnvContainer.This.DbConnectionString));
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  public DbSet<ModmailLog> ModmailLogs { get; set; }
  public DbSet<ModmailOption> ModmailOptions { get; set; }
}