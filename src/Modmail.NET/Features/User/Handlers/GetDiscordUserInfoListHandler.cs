using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.User.Queries;

namespace Modmail.NET.Features.User.Handlers;

public class GetDiscordUserInfoListHandler : IRequestHandler<GetDiscordUserInfoListQuery, List<UserInformation>>
{
    private readonly ModmailDbContext _dbContext;

    public GetDiscordUserInfoListHandler(ModmailDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<List<UserInformation>> Handle(GetDiscordUserInfoListQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.UserInformation.ToListAsync(cancellationToken);
    }
}