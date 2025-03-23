namespace Modmail.NET.Web.Blazor.Services;

public class ModmailHostedService : IHostedService
{
  private readonly ModmailBot _modmailBot;

  public ModmailHostedService(ModmailBot modmailBot) {
    _modmailBot = modmailBot;
  }

  public async Task StartAsync(CancellationToken cancellationToken) {
    await _modmailBot.StartAsync();
  }

  public async Task StopAsync(CancellationToken cancellationToken) {
    await _modmailBot.StopAsync();
  }
}