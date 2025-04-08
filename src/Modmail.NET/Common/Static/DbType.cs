using Ardalis.SmartEnum;

namespace Modmail.NET.Common.Static;

public sealed class DbType : SmartEnum<DbType>
{
  public static readonly DbType MsSql = new(nameof(MsSql), 1);
  public static readonly DbType Sqlite = new(nameof(Sqlite), 2);
  public static readonly DbType Postgres = new(nameof(Postgres), 3);
  public static readonly DbType MySql = new(nameof(MySql), 4);
  private DbType(string name, int value) : base(name, value) { }
}