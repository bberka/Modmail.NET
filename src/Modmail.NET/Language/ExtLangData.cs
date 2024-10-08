﻿namespace Modmail.NET.Language;

public static class ExtLangData
{
  public static string GetTranslation(this LangKeys key) {
    return LangData.This.GetTranslation(key);
  }

  // public static string GetTranslation(this LangKeys key, string lang) => LangData.This.GetTranslation(lang, key);
  public static string GetTranslation(this LangKeys key, params object[] args) {
    return LangData.This.GetTranslation(key, args);
  }
}