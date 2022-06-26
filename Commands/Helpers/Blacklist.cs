using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using RestoreCord.Database;
using RestoreCord.Database.Models;
using RestoreCord.Utilities;
using RestoreCord.Utilities.DiscordAttributes;

namespace RestoreCord.Commands;

[RequireAdministrator]
public class Blacklist : InteractionModuleBase<ShardedInteractionContext>
{

    [SlashCommand("blacklist-user", "Ban and blacklist a specific user from verifying.")]
    public async Task BlacklistUserAsync(IUser user, string? reason = null)
    {
        await using var database = new DatabaseContext();
        Server? server = await database.servers.FirstOrDefaultAsync(x => x.guildId == Context.Guild.Id);
        if (server is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occurred", "This guild does not exist in our database, please try again.", invisible: true, deleteTimer: 60);
            return;
        }
        if (server.banned)
        {
            await Context.ReplyWithEmbedAsync("Error Occurred", "This guild has been banned from using the bot.", invisible: true, deleteTimer: 60);
            return;
        }
        Database.Models.Blacklist? blacklistUser = server.settings.blacklist.FirstOrDefault(x => x.discordId == user.Id);
        if (blacklistUser is not null)
        {
            await Context.ReplyWithEmbedAsync("Error Occurred", "This user is already blacklisted.", invisible: true, deleteTimer: 60);
            return;
        }
        Member? userEntry = await database.members.FirstOrDefaultAsync(x => x.discordId == user.Id && x.guildId == Context.Guild.Id);
        server.settings.blacklist.Add(new Database.Models.Blacklist()
        {
            ip = userEntry?.ip,
            discordId = user.Id,
            reason = reason
        });
        await database.ApplyChangesAsync();
        await Context.Guild.GetUser(user.Id).BanAsync(7, reason);
        await Context.ReplyWithEmbedAsync("Blacklist", $"Successfully blacklisted & banned {user.Mention}", deleteTimer: 60);
    }
}
