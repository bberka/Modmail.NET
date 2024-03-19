namespace Modmail.NET.Console;

internal class Program
{
  private static async Task Main(string[] args) {
    await ModmailBot.This.StartAsync();
  }
}