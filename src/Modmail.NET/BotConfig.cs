using System.ComponentModel.DataAnnotations;

namespace Modmail.NET;

public class BotConfig
{
    private readonly Uri? _domain;
    public EnvironmentType Environment { get; init; }
    public required string BotToken { get; init; }
    public required string BotClientId { get; init; }
    public required string BotClientSecret { get; init; }
    public required string BotPrefix { get; init; }
    public ulong MainServerId { get; init; }
    public required ulong[] SuperUsers { get; init; }
    public required string DbConnectionString { get; init; }
    public required string DefaultLanguage { get; init; }
    public bool SensitiveEfCoreDataLog { get; init; }

    [Url]
    public required string Domain
    {
        get => _domain?.AbsoluteUri ?? throw new InvalidOperationException("Domain is not set");
        init => _domain = Uri.TryCreate(value, UriKind.Absolute, out var uri) ? uri : throw new ArgumentException("Invalid URL", nameof(value));
    }

    public Uri DomainUri => _domain ?? throw new InvalidOperationException("Domain is not set");
}