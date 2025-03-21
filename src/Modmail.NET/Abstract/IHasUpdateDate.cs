namespace Modmail.NET.Abstract;

public interface IHasUpdateDate
{
  DateTime? UpdateDateUtc { get; set; }
}