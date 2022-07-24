using Discord;
using Discord.WebSocket;

using Microsoft.EntityFrameworkCore;

using RestoreCord.Database;
using RestoreCord.Database.Models;
using RestoreCord.Utilities;

namespace RestoreCord.Events;

public class User
{
    private readonly DiscordShardedClient _client;
    public User(DiscordShardedClient client)
    {
        _client = client;
        _client.UserJoined += UserJoinedGuild;
        _client.UserBanned += UserBannedFromGuild;
    }

    private async Task UserBannedFromGuild(SocketUser arg1, SocketGuild arg2)
    {
        await using var database = new DatabaseContext();
        Server? server = await database.servers.FirstOrDefaultAsync(x => x.guildId == arg2.Id);
        if (server is null)
        {
            return;
        }
        if (server.banned)
        {
            return;
        }
        if (server.settings.autoBlacklist is false)
            return;
        Database.Models.User? owner = await database.users.FirstOrDefaultAsync(x => x.username == server.owner.username);
        if (owner is not null)
        {
            if (owner.role != "business" || owner.role == "admin")
                return;
        }
        Blacklist? blacklistUser = server.settings.blacklist.FirstOrDefault(x => x.discordId == arg1.Id);
        if (blacklistUser is not null)
        {
            return;
        }
        Member? userEntry = await database.members.FirstOrDefaultAsync(x => x.discordId == arg1.Id && x.server.guildId == arg2.Id);
        server.settings.blacklist.Add(new Blacklist()
        {
            ip = userEntry?.ip,
            discordId = arg1.Id,
        });
        await database.ApplyChangesAsync();
    }

    private async Task UserJoinedGuild(SocketGuildUser guildUser)
    {
        await AutoKick(guildUser);
    }

    private async Task AutoKick(SocketGuildUser guildUser)
    {
        try
        {
            if (guildUser.IsBot)
                return;
            if (guildUser.GuildPermissions.Administrator)
                return;
            int verifyTime = 0;
            ulong roleId = 0;
            bool dmOnKick = false;
            //check if bot || user is admin
            await using (var database = new DatabaseContext())
            {
                Server? server = await database.servers.FirstOrDefaultAsync(x => x.guildId == guildUser.Guild.Id);
                if (server is null)
                    return;
                if (server.banned)
                    return;
                if (server.roleId is null)
                    return;
                Discord.Rest.RestGuild? guildSocket = await _client.Rest.GetGuildAsync(guildUser.Guild.Id);
                Discord.Rest.RestRole? guildRole = guildSocket.Roles.FirstOrDefault(x => x.Id == (ulong)server.roleId);
                if (guildRole is null)
                    return;
                if (guildUser.Id is 903728123676360727 or 771095495271383040 or 810257712364519434 or 852591545844105247)
                {
                    await guildUser.AddRoleAsync(guildRole);
                    IDMChannel? dms = await guildUser.CreateDMChannelAsync();
                    if (dms is not null)
                        await dms.SendEmbedAsync("Verify Role Bypass", $"Successfully bypassed verify and given {guildRole.Name}", "RestoreCord made with <3");
                    return;
                }
                if (server.settings.autoKickUnVerified is false)
                    return;
                if (server.settings.autoKickUnVerifiedTime == 0)
                    return;
                SocketRole? anythingHigher = guildUser.Guild.CurrentUser.Roles.FirstOrDefault(x => x.Position > guildRole.Position);
                if (anythingHigher is null)
                {
                    await guildUser.Guild.TextChannels.First().SendEmbedAsync("Error Occurred", $"Failed to give {guildUser.Mention} the {guildRole.Mention} role, because my role is below it.", "RestoreCord made with <3", "https://i.imgur.com/Nfy4OoG.png");
                    return;
                }
                Discord.Rest.RestGuildUser? updatedUser = await _client.Rest.GetGuildUserAsync(guildUser.Guild.Id, guildUser.Id);
                if (updatedUser.RoleIds.Contains((ulong)server.roleId))
                    return;
                verifyTime =server.settings.autoKickUnVerifiedTime;
                roleId = (ulong)server.roleId;
                dmOnKick = server.settings.dmOnAutoKick;
            }
            _ = Task.Run(async () =>
            {

                await Task.Delay(TimeSpan.FromMinutes(verifyTime > 60 ? 60 : verifyTime));
                Discord.Rest.RestGuildUser? updatedUser = await _client.Rest.GetGuildUserAsync(guildUser.Guild.Id, guildUser.Id);
                if (updatedUser.RoleIds.Contains(roleId))
                    return;
                await updatedUser.KickAsync($"Did not verify within {(verifyTime > 60 ? 60 : verifyTime)} minutes");
                //dm user
                if (dmOnKick)
                {
                    Discord.Rest.RestDMChannel? dm = await updatedUser.CreateDMChannelAsync();
                    MessageComponent? components = new ComponentBuilder()
                    {
                        ActionRows = new List<ActionRowBuilder>()
                    {
                        new ActionRowBuilder()
                        {
                            Components = new List<IMessageComponent>
                            {
                                new ButtonBuilder()
                                {
                                    Url = $"https://discord.com/oauth2/authorize?client_id=791106018175614988&scope=identify+guilds.join&response_type=code&prompt=none&prompt=none&redirect_uri=https://restorecord.com/auth/&state={guildUser.Guild.Id}",
                                    Label = "Join Back!",
                                    Style = ButtonStyle.Link
                                }.Build(),
                            }
                        }
                    }
                    }.Build();
                    Embed? embed = new EmbedBuilder()
                    {
                        Title = "You've Been Kicked!",
                        Color = Miscallenous.RandomDiscordColour(),
                        Author = new EmbedAuthorBuilder
                        {
                            Name = guildUser.Guild.Name,
                            IconUrl = guildUser.Guild.IconUrl
                        },
                        Footer = new EmbedFooterBuilder
                        {
                            Text = "RestoreCord",
                            IconUrl = "https://i.imgur.com/Nfy4OoG.png"
                        },
                        Description = $"You've been kicked from {guildUser.Guild.Name} for exceeding the verification time of {(verifyTime > 60 ? 60 : verifyTime)} minutes.",
                    }.WithCurrentTimestamp().Build();
                    await dm.SendMessageAsync(embed: embed, components: components);
                }
            });
        }
        catch (Exception e) { await e.LogErrorAsync(); }
    }

}
