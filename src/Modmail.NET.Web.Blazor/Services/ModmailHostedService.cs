namespace Modmail.NET.Web.Blazor.Services;

public sealed class ModmailHostedService : IHostedService
{
  private readonly ModmailBot _modmailBot;

  public ModmailHostedService(ModmailBot modmailBot) {
    _modmailBot = modmailBot;
  }
  public async Task StartAsync(CancellationToken cancellationToken) {
    await ModmailBot.StartAsync();
  }

  public async Task StopAsync(CancellationToken cancellationToken) {
    await ModmailBot.StopAsync();
  }
}