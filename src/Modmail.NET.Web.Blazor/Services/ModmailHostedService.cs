namespace Modmail.NET.Web.Blazor.Services;

public sealed class ModmailHostedService : IHostedService
{
  public async Task StartAsync(CancellationToken cancellationToken) {
    await ModmailBot.This.StartAsync();
  }

  public async Task StopAsync(CancellationToken cancellationToken) {
    await ModmailBot.This.StopAsync();
  }
}