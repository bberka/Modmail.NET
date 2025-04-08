using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class TicketTypeAlreadyExistsException : ModmailBotException
{
  public TicketTypeAlreadyExistsException(string name) : base(LangProvider.This.GetTranslation(LangKeys.TicketTypeAlreadyExists)) {
    Name = name;
  }

  public string Name { get; }
}