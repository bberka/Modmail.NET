namespace Modmail.NET.Exceptions;

public class TicketTypeAlreadyExistsException : BotExceptionBase
{
    public TicketTypeAlreadyExistsException(string name) : base(LangData.This.GetTranslation(LangKeys.TICKET_TYPE_ALREADY_EXISTS))
    {
        Name = name;
    }

    public string Name { get; }
}