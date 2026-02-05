using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Server.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Server.Handlers;

public class GetOptionHandler : IRequestHandler<GetOptionQuery, Option>
{
    private readonly ModmailDbContext _dbContext;
    private readonly IOptions<BotConfig> _options;

    public GetOptionHandler(ModmailDbContext dbContext, IOptions<BotConfig> options)
    {
        _dbContext = dbContext;
        _options = options;
    }

    public async ValueTask<Option> Handle(GetOptionQuery request, CancellationToken cancellationToken)
    {
        var option = await _dbContext.Options.FirstOrDefaultAsync(x => x.ServerId == _options.Value.MainServerId, cancellationToken);
        if (option is null) throw new ModmailBotException(Lang.ServerNotSetup);
        return option;
    }
}