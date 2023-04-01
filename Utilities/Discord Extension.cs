using Discord;

using DiscordRepair.Api.Database;
using DiscordRepair.Api.Database.Models;
using DiscordRepair.Api.Records.Responses;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Utilities;

internal static class DiscordExtensions
{
    internal static async Task ReplyWithEmbedAsync(this IInteractionContext context, string title, string description, string url = "", string imageUrl = "", List<EmbedFieldBuilder>? embeds = null, int? deleteTimer = null, bool invisible = false)
    {
        try
        {
            if (context is not Discord.Rest.RestInteractionContext restContext)
            {
                throw new ArgumentNullException(nameof(restContext), "Failed to convert context to a sharded context.");
            }

            Embed? embed = new EmbedBuilder()
            {
                Title = title,
                Color = Miscallenous.RandomDiscordColour(),
                Author = new EmbedAuthorBuilder
                {
                    Url = "https://discord.repair",
                    Name = "Discord.Repair",
                    IconUrl = "https://discord.repair/favicon.ico"
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

            if (restContext.Interaction.HasResponded)
            {
                await context.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed);
            }
            else
            {
                await context.Interaction.RespondAsync(embed: embed, ephemeral: invisible);
            }

            if (deleteTimer is not null && invisible is false)
            {
                _ = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds((int)deleteTimer));
                    IUserMessage? msg = await context.Interaction.GetOriginalResponseAsync();
                    await msg.DeleteAsync();
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
                Url = "https://discord.repair",
                Name = "Discord.Repair",
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

        IUserMessage msg = channel as ITextChannel is not null ? await (channel as ITextChannel).SendMessageAsync(embed: embed) : await (channel as IDMChannel).SendMessageAsync(embed: embed);
        try
        {
            if (deleteTimer is not null && msg is not null)
            {
                _ = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds((int)deleteTimer));
                    await msg.DeleteAsync();
                });
            }
        }
        catch { }
    }

    internal static ActionResult? VerifyServer(this ControllerBase @base, ulong guildId, ulong userId, string token)
    {
        return userId is 0 || guildId is 0
            ? @base.BadRequest(new Generic() { success = false, details = "invalid parameters, please try again." })
            : string.IsNullOrWhiteSpace(token) ? @base.BadRequest(new Generic() { success = false, details = "invalid token." }) : (ActionResult?)null;
    }
    internal static ActionResult? VerifyServer(this ControllerBase @base, ulong guildId, string token)
    {
        return guildId is 0
            ? @base.BadRequest(new Generic() { success = false, details = "invalid parameters, please try again." })
            : string.IsNullOrWhiteSpace(token) ? @base.BadRequest(new Generic() { success = false, details = "invalid token." }) : (ActionResult?)null;
    }
    internal static async ValueTask<(ActionResult? httpResult, Server? server)> VerifyServer(this ControllerBase @base, DatabaseContext database, ulong guildId, string token)
    {
        Server? server = await database.servers.FirstOrDefaultAsync(x => x.guildId == guildId && x.owner.apiToken == token);
        return server is null
            ? ((ActionResult? httpResult, Server? server))(@base.BadRequest(new Generic() { success = false, details = "server does not exist, please try again." }), null)
            : server.banned
            ? (@base.BadRequest(new Generic() { success = false, details = "server is banned." }), null)
            : (null, server);
    }

    internal static ActionResult? VerifyServer(this ControllerBase @base, string? serverName, ulong userId, string token)
    {
        return (string.IsNullOrWhiteSpace(serverName) || userId is 0)
            ? @base.BadRequest(new Generic() { success = false, details = "invalid parameters, please try again." })
            : serverName.Length > 64
            ? @base.BadRequest(new Generic()
            {
                success = false,
                details = "inalivd paramater, please try again."
            })
            : string.IsNullOrWhiteSpace(token) ? @base.BadRequest(new Generic() { success = false, details = "invalid token." }) : (ActionResult?)null;
    }
    internal static ActionResult? VerifyServer(this ControllerBase @base, string? serverName, string token)
    {
        return string.IsNullOrWhiteSpace(serverName)
            ? @base.BadRequest(new Generic() { success = false, details = "invalid parameters, please try again." })
            : serverName.Length > 64
            ? @base.BadRequest(new Generic()
            {
                success = false,
                details = "inalivd paramater, please try again."
            })
            : string.IsNullOrWhiteSpace(token) ? @base.BadRequest(new Generic() { success = false, details = "invalid token." }) : (ActionResult?)null;
    }

    internal static async ValueTask<(ActionResult? httpResult, Server? server)> VerifyServer(this ControllerBase @base, DatabaseContext database, string serverName, string token)
    {
        Server? server = await database.servers.FirstOrDefaultAsync(x => (x.name == serverName || x.key.ToString() == serverName) && x.owner.apiToken == token);
        return server is null
            ? ((ActionResult? httpResult, Server? server))(@base.BadRequest(new Generic() { success = false, details = "server does not exist, please try again." }), null)
            : server.banned
            ? (@base.BadRequest(new Generic() { success = false, details = "server is banned." }), null)
            : (null, server);
    }

    internal static async ValueTask<bool> IsGuildBusy(ulong guildId)
    {
        try
        {
            await using var database = new DatabaseContext();
            return await database.statistics.OrderByDescending(x => x.active).FirstOrDefaultAsync(x => x.guildId == guildId && x.active) is not null;
        }
        catch (Exception ex)
        {
            await ex.LogErrorAsync();
            return true;
        }
    }

    //internal static async ValueTask<bool> IsAboveVerifyRole(this Server server, Discord.Rest.RestInteractionContext context)
    //{
    //    if (server.roleId is null)
    //        return true;
    //    Discord.Rest.RestRole? serverRole = context.Guild.GetRole((ulong)server.roleId);
    //    if (serverRole is null)
    //    {
    //        await context.ReplyWithEmbedAsync("Error Occurred", $"Verify role could not be found, please check server settings on the dashboard.", invisible: true, deleteTimer: 60);
    //        return false;
    //    }
    //    var user = await context.Guild.GetCurrentUserAsync();
    //    if (user is null)
    //    {
    //        await context.ReplyWithEmbedAsync("Error Occurred", $"Unable to fetch user data.", invisible: true, deleteTimer: 60);
    //        return false;
    //    }
    //    var guildRoles = context.Guild.Roles;
    //    Discord.Rest.RestRole? isRoleAboveMe = user.RoleIds.FirstOrDefault(x => x.Position > serverRole.Position);
    //    if (isRoleAboveMe is null)
    //    {
    //        await context.ReplyWithEmbedAsync("Error Occurred", $"Bot role is not above {serverRole.Mention}", invisible: true, deleteTimer: 60);
    //        return false;
    //    }
    //    return true;
    //}

    internal static bool IsVerifyServerOkay(this Server server) => server is not null && server.banned is false;

    internal static async ValueTask<bool> IsTopRoleServerOkay(Server server, Discord.Rest.RestInteractionContext context)
    {
        if (server is null)
        {
            await context.ReplyWithEmbedAsync("Error Occurred", "This guild does not exist in our database, please try again.", invisible: true, deleteTimer: 60);
            return false;
        }
        if (server.banned)
        {
            await context.ReplyWithEmbedAsync("Error Occurred", "This guild has been banned from using the bot.", invisible: true, deleteTimer: 60);
            return false;
        }
        return true;
    }
}
