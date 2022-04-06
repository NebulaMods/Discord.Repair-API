﻿using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using RestoreCord.Database;
using RestoreCord.Database.Models;

namespace RestoreCord.Utilities;

internal static class DiscordExtensions
{
    internal static async Task ReplyWithEmbedAsync(this IInteractionContext context, string title, string description, string url = "", string imageUrl = "", List<EmbedFieldBuilder>? embeds = null, int? deleteTimer = null, bool invisible = false)
    {
        if (context is not ShardedInteractionContext shardedContext)
        {
            throw new ArgumentNullException(nameof(shardedContext), "Failed to convert context to a sharded context.");
        }

        Embed? embed = new EmbedBuilder()
        {
            Title = title,
            Color = Miscallenous.RandomDiscordColour(),
            Author = new EmbedAuthorBuilder
            {
                Url = "https://restorecord.com",
                Name = "RestoreCord",
                IconUrl = "https://i.imgur.com/Nfy4OoG.png"
            },
            Footer = new EmbedFooterBuilder
            {
                Text = $"Issued by: {context.User.Username} | {context.User.Id}",
                IconUrl = context.User.GetAvatarUrl()
            },
            Description = description,
            Url = url,
            ThumbnailUrl = imageUrl,
        }.WithCurrentTimestamp().Build();
        if (embeds is not null)
        {
            embed = embed.ToEmbedBuilder().WithFields(embeds).Build();
        }

        if (shardedContext.Interaction.HasResponded)
        {
            await context.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed);
        }
        else
        {
            await context.Interaction.RespondAsync(embed: embed, ephemeral: invisible);
        }

        try
        {
            if (deleteTimer is not null && invisible is false)
            {
                _ = Task.Run(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds((int)deleteTimer));
                    IUserMessage? msg = context.Interaction.GetOriginalResponseAsync().Result;
                    msg?.DeleteAsync();
                });
            }
        }
        catch { }
    }
    internal static async Task SendEmbedAsync(this IChannel channel, string title, string description, string footer, string footerIcon = "https://i.imgur.com/Nfy4OoG.png", List<EmbedFieldBuilder>? embeds = null, int? deleteTimer = null)
    {
        Embed? embed = new EmbedBuilder()
        {
            Title = title,
            Color = Miscallenous.RandomDiscordColour(),
            Author = new EmbedAuthorBuilder
            {
                Url = "https://restorecord.com",
                Name = "RestoreCord",
                IconUrl = "https://i.imgur.com/Nfy4OoG.png"
            },
            Footer = new EmbedFooterBuilder
            {
                Text = footer,
                IconUrl = footerIcon
            },
            Description = description,
        }.WithCurrentTimestamp().Build();
        if (embeds is not null)
        {
            embed = embed.ToEmbedBuilder().WithFields(embeds).Build();
        }

        IUserMessage msg = (channel as ITextChannel) is not null ? await (channel as ITextChannel).SendMessageAsync(embed: embed) : await (channel as IDMChannel).SendMessageAsync(embed: embed);
        try
        {
            if (deleteTimer is not null && msg is not null)
            {
                _ = Task.Run(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds((int)deleteTimer));
                    msg.DeleteAsync();
                });
            }
        }
        catch { }
    }

    internal static async ValueTask<bool> CheckBusinessMembership(DatabaseContext database, ShardedInteractionContext context, bool sendEmbed = true)
    {
        if (context.User.Id != 771095495271383040)
        {
            User? user = await database.users.FirstOrDefaultAsync(x => x.userId == context.User.Id);
            if (user is null)
            {
                if (sendEmbed)
                    await context.ReplyWithEmbedAsync("Error Occurred", "Sorry, this feature is only available for business users. (You may have to link your account on the dashboard)", invisible: true, deleteTimer: 60);
                return false;
            }
            if (user.admin is false)
            {
                switch (user.role)
                {
                    case "business":
                        return true;
                    default:
                        if (sendEmbed)
                            await context.ReplyWithEmbedAsync("Error Occurred", "Sorry, this feature is only available for business users. (You may have to link your account on the dashboard)", invisible: true, deleteTimer: 60);
                        return false;
                }
            }
        }
        return true;
    }
    internal static async ValueTask<bool> CheckPremiumMembership(DatabaseContext database, ShardedInteractionContext context, bool sendEmbed = true)
    {
        if (context.User.Id != 771095495271383040)
        {
            User? user = await database.users.FirstOrDefaultAsync(x => x.userId == context.User.Id);
            if (user is null)
            {
                if (sendEmbed)
                    await context.ReplyWithEmbedAsync("Error Occurred", "Sorry, this feature is only available for premium users. (You may have to link your account on the dashboard)", invisible: true, deleteTimer: 60);
                return false;
            }
            if (user.admin is false)
            {
                switch (user.role)
                {
                    case "business":
                    case "premium":
                        return true;
                    default:
                        if (sendEmbed)
                            await context.ReplyWithEmbedAsync("Error Occurred", "Sorry, this feature is only available for premium users. (You may have to link your account on the dashboard)", invisible: true, deleteTimer: 60);
                        return false;
                }
            }
        }
        return true;
    }
    
    internal static async ValueTask<bool> IsGuildBusy(this OldMigration migration, ulong guildId)
    {
        try
        {
            return migration.ActiveGuildMigrations.ContainsKey(guildId)
                ? true
                : migration.ActiveMemberMigrations.ContainsKey(guildId);
        }
        catch (Exception ex)
        {
            await ex.LogErrorAsync();
            return true;
        }
    }

    internal static async ValueTask<bool> IsAboveVerifyRole(this Server server, ShardedInteractionContext context)
    {
        if (server.roleid is null)
            return true;
        Discord.WebSocket.SocketRole? serverRole = context.Guild.GetRole((ulong)server.roleid);
        if (serverRole is null)
        {
            await context.ReplyWithEmbedAsync("Error Occurred", $"Verify role could not be found, please check server settings on the dashboard.", invisible: true, deleteTimer: 60);
            return false;
        }
        Discord.WebSocket.SocketRole? isRoleAboveMe = context.Guild.CurrentUser.Roles.FirstOrDefault(x => x.Position > serverRole.Position);
        if (isRoleAboveMe is null)
        {
            await context.ReplyWithEmbedAsync("Error Occurred", $"Bot role is not above {serverRole.Mention}", invisible: true, deleteTimer: 60);
            return false;
        }
        return true;
    }

    internal static bool IsVerifyServerOkay(this Server server) => server is null ? false : string.IsNullOrWhiteSpace(server.banned) is true;

    internal static async ValueTask<bool> IsTopRoleServerOkay(Server server, ShardedInteractionContext context)
    {
        if (server is null)
        {
            await context.ReplyWithEmbedAsync("Error Occurred", "This guild does not exist in our database, please try again.", invisible: true, deleteTimer: 60);
            return false;
        }
        if (string.IsNullOrWhiteSpace(server.banned) is false)
        {
            await context.ReplyWithEmbedAsync("Error Occurred", "This guild has been banned from using the bot.", invisible: true, deleteTimer: 60);
            return false;
        }
        return true;
    }
}