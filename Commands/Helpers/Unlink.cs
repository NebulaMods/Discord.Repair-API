using Discord.Interactions;

using Microsoft.EntityFrameworkCore;

using RestoreCord.Database;
using RestoreCord.Database.Models;
using RestoreCord.Utilities;

namespace RestoreCord.Commands;
public class Unlink : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("unlink", "Unlink your account from this guild or the guild specified. Visit the website for more details.")]
    public async Task UnlinkGuildAsync(string? guildId = null)
    {
        await using var database = new DatabaseContext();
        ulong guildID = 0;
        if (string.IsNullOrWhiteSpace(guildId) is false)
        {
            if (ulong.TryParse(guildId, out guildID) is false)
            {
                await Context.ReplyWithEmbedAsync("Error Occurred", "The guild id enter is not valid, please try again.", invisible: true, deleteTimer: 60);
                return;
            }
        }
        if (string.IsNullOrWhiteSpace(guildId) && Context.Guild is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occurred", "The guild id enter is not valid, please try again.", invisible: true, deleteTimer: 60);
            return;
        }

        Member? userEntry = await database.members.FirstOrDefaultAsync(x => x.discordId == Context.User.Id && x.server.guildId == (guildID != 0 ? guildID : Context.Guild.Id));
        if (userEntry is null)
        {
            await Context.ReplyWithEmbedAsync("Unlink Status", "Account is not linked to the specified guild.", deleteTimer: 60);
            return;
        }
        database.Remove(userEntry);
        await database.ApplyChangesAsync();
        await Context.ReplyWithEmbedAsync("Unlink Status", $"Successfully unlinked from {(guildId is null ? Context.Guild.Id : guildId)}.", deleteTimer: 60);
        try
        {
            Discord.Rest.RestGuildUser? guildUser = await Context.Client.Rest.GetGuildUserAsync(guildID != 0 ? guildID : Context.Guild.Id, Context.User.Id);
            if (guildUser is not null)
            {
                Server? serverEntry = await database.servers.FirstOrDefaultAsync(x => x.guildId == (guildID != 0 ? guildID : Context.Guild.Id));
                if (serverEntry is null)
                    return;
                if (serverEntry.roleId is not null)
                await guildUser.RemoveRoleAsync((ulong)serverEntry.roleId);
            }
        }
        catch { }
    }
}
