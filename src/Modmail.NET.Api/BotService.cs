namespace Modmail.NET.Api;

public sealed class BotService : BackgroundService
{
  protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
    await ModmailBot.This.StartAsync();
  }
}