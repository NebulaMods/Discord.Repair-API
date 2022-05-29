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
        Server? server = await database.servers.FirstOrDefaultAsync(x => x.guildid == arg2.Id);
        if (server is null)
        {
            return;
        }
        if (string.IsNullOrWhiteSpace(server.banned) is false)
        {
            return;
        }
        if (server.autoBlacklist is false)
            return;
        var owner = await database.users.FirstOrDefaultAsync(x => x.username == server.owner);
        if (owner is not null)
        {
            if (owner.role != "business" || owner.admin is false)
                return;
        }
        Blacklist? blacklistUser = await database.blacklist.FirstOrDefaultAsync(x => x.userid == arg1.Id && x.server == arg2.Id);
        if (blacklistUser is not null)
        {
            return;
        }
        Member? userEntry = await database.members.FirstOrDefaultAsync(x => x.userid == arg1.Id && x.server == arg2.Id);
        await database.blacklist.AddAsync(new Blacklist()
        {
            ip = userEntry?.ip,
            server = arg2.Id,
            userid = arg1.Id,
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
                Server? server = await database.servers.FirstOrDefaultAsync(x => x.guildid == guildUser.Guild.Id);
                if (server is null)
                    return;
                if (string.IsNullOrWhiteSpace(server.banned) is false)
                    return;
                if (server.roleid is null)
                    return;
                Discord.Rest.RestGuild? guildSocket = await _client.Rest.GetGuildAsync(guildUser.Guild.Id);
                Discord.Rest.RestRole? guildRole = guildSocket.Roles.FirstOrDefault(x => x.Id == (ulong)server.roleid);
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
                if (server.autoKickUnVerified is false)
                    return;
                if (server.autoKickUnVerifiedTime == 0)
                    return;
                SocketRole? anythingHigher = guildUser.Guild.CurrentUser.Roles.FirstOrDefault(x => x.Position > guildRole.Position);
                if (anythingHigher is null)
                {
                    await guildUser.Guild.TextChannels.First().SendEmbedAsync("Error Occurred", $"Failed to give {guildUser.Mention} the {guildRole.Mention} role, because my role is below it.", "RestoreCord made with <3", "https://i.imgur.com/Nfy4OoG.png");
                    return;
                }
                Discord.Rest.RestGuildUser? updatedUser = await _client.Rest.GetGuildUserAsync(guildUser.Guild.Id, guildUser.Id);
                if (updatedUser.RoleIds.Contains((ulong)server.roleid))
                    return;
                verifyTime = server.autoKickUnVerifiedTime;
                roleId = (ulong)server.roleid;
                dmOnKick = server.dmOnAutoKick;
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
                    var dm = await updatedUser.CreateDMChannelAsync();
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
