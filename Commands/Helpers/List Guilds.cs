using Discord.Interactions;

using Microsoft.EntityFrameworkCore;

using RestoreCord.Database;
using RestoreCord.Database.Models;
using RestoreCord.Utilities;

namespace RestoreCord.Commands;
public class ListGuilds : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("linked-guilds", "Display a list of guilds your account is associated with.")]
    public async Task DisplayLinkedGuildsAsync()
    {
        await using var database = new DatabaseContext();
        List<Member>? guilds = await database.members.Where(x => x.discordId == Context.User.Id).ToListAsync();
        if (guilds.Any() is false)
        {
            await Context.ReplyWithEmbedAsync("Linked Guilds", "You do not have any guilds linked.", invisible: true, deleteTimer: 60);
            return;
        }
        string guildString = string.Empty;
        foreach(Member? guild in guilds)
        {
            Server? guildName = await database.servers.FirstOrDefaultAsync(x => x.guildId == guild.server.guildId);
            if (guildName is null)
            {
                guildString += $"{guild.server.guildId} | [Unknown Name](https://discord.com/channels/{guild.server.guildId})\n";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(guildName.name) is false)
                {
                    guildString += $"{guild.server.guildId} | [{guildName.name}](https://discord.com/channels/{guild.server.guildId})\n";
                }
                else
                {
                    guildString += $"{guild.server.guildId} | [Unknown Name](https://discord.com/channels/{guild.server.guildId})\n";
                }
            }
        }
        await Context.ReplyWithEmbedAsync("Linked Guilds", guildString, deleteTimer: 120);
    }
}
