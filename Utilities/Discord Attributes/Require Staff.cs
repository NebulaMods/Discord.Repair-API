using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using RestoreCord.Database;

namespace RestoreCord.Utilities.DiscordAttributes;

public class RequireStaffAttribute : PreconditionAttribute
{
    public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        switch (context.Client.TokenType)
        {
            case TokenType.Bot:
                if (context.User.Id is 903728123676360727 or 771095495271383040 or 810257712364519434)
                    return PreconditionResult.FromSuccess();
                await using (var database = new DatabaseContext())
                {
                    Database.Models.User? user = await database.users.FirstOrDefaultAsync(x => x.discordId == context.User.Id);
                    if (user is not null)
                    {
                        if (user.role == "admin")
                            return PreconditionResult.FromSuccess();
                    }
                }
                return PreconditionResult.FromError(ErrorMessage ?? "Command can only be executed by the owner of the bot.");
            default:
                return PreconditionResult.FromError($"{nameof(RequireStaffAttribute)} is not supported by this {nameof(TokenType)}.");
        }
    }
}
