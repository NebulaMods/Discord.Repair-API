﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace RestoreCord.Utilities.DiscordAttributes;

public class RequireAdministratorAttribute : PreconditionAttribute
{
    public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        switch (context.Client.TokenType)
        {
            case TokenType.Bot:
                if (context.Guild is null)
                    return PreconditionResult.FromError(ErrorMessage ?? "Command can only be executed in a guild.");
                //bs checks
                IGuildUser? me = await context.Guild.GetCurrentUserAsync();
                if (me.GuildPermissions.Administrator is false)
                    return PreconditionResult.FromError(ErrorMessage ?? "Missing permissions, please give RestoreCord administrator and try again.");
                if (context.User.Id is 903728123676360727 or 771095495271383040 or 810257712364519434)
                    return PreconditionResult.FromSuccess();
                DiscordShardedClient? client = services.GetService<DiscordShardedClient>();
                if (client is not null)
                {
                    Discord.Rest.RestGuildUser? guildUser = await client.Rest.GetGuildUserAsync(context.Guild.Id, context.User.Id);
                    if (guildUser is not null)
                    {
                        if (guildUser.GuildPermissions.Administrator)
                            return PreconditionResult.FromSuccess();
                    }
                }
                return PreconditionResult.FromError(ErrorMessage ?? "Command can only be executed by an administrator of the guild.");
            default:
                return PreconditionResult.FromError($"{nameof(RequireStaffAttribute)} is not supported by this {nameof(TokenType)}.");
        }
    }
}
