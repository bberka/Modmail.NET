using Modmail.NET.Common;

namespace Modmail.NET.Console;

class Program
{
  static async Task Main(string[] args) {
    await ModmailBot.This.StartAsync();
  }
}