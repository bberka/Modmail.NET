﻿using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class AnotherServerAlreadySetupException : BotExceptionBase
{
  public AnotherServerAlreadySetupException() : base(LangProvider.This.GetTranslation(LangKeys.AnotherServerAlreadySetup)) { }
}