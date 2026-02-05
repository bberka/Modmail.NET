namespace Modmail.NET.Database.Abstract;

public interface IHasUpdateDate
{
  DateTime? UpdateDateUtc { get; set; }
}