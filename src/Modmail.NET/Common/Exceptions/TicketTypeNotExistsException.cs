using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class TicketTypeNotExistsException : ModmailBotException
{
  public TicketTypeNotExistsException(string name = null) : base(LangProvider.This.GetTranslation(LangKeys.TicketTypeNotExists)) {
    Name = name;
  }

  public string Name { get; }
}