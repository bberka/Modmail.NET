﻿namespace Modmail.NET.Common.Static;

public static class DbLength
{
  public const int Name = 128;
  public const int Url = 4000;
  public const int Email = 128;
  public const int Locale = 10;
  public const int BotMessage = 1000; //Greetings, closing messages
  public const int Message = int.MaxValue;
  public const int Reason = 300;
  public const int FeedbackMessage = 1000;
  public const int FileName = 500;
  public const int MediaType = 100;
  public const int Note = 1000;
  public const int KeyString = 128;
  public const int Emoji = 100;
  public const int Description = 1000;
  public const int TagName = 32;
  public const int TagTitle = 64;
}