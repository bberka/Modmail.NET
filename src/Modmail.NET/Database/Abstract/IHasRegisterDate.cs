namespace Modmail.NET.Database.Abstract;

public interface IHasRegisterDate
{
  DateTime RegisterDateUtc { get; set; }
}