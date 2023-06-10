using Discord;

using DiscordRepair.Api.Database;
using DiscordRepair.Api.Database.Models;
using DiscordRepair.Api.Records.Responses;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Utilities;

internal static class DiscordExtensions
{
    /// <summary>
    /// Sends a response message with an embedded content in response to a Discord interaction.
    /// </summary>
    /// <param name="context">The interaction context.</param>
    /// <param name="title">The title of the embedded content.</param>
    /// <param name="description">The description of the embedded content.</param>
    /// <param name="url">The URL of the embedded content.</param>
    /// <param name="imageUrl">The URL of the thumbnail of the embedded content.</param>
    /// <param name="embeds">The list of fields for the embedded content.</param>
    /// <param name="deleteTimer">The time after which the response message should be deleted.</param>
    /// <param name="invisible">Whether the response message should be invisible.</param>
    internal static async Task ReplyWithEmbedAsync(this IInteractionContext context, string title, string description, string url = "", string imageUrl = "", List<EmbedFieldBuilder>? embeds = null, int? deleteTimer = null, bool invisible = false)
    {
        try
        {
            // Create the embed builder object and set its properties.
            var embedBuilder = new EmbedBuilder()
                .WithTitle(title)
                .WithColor(Miscallenous.RandomDiscordColour())
                .WithAuthor(new EmbedAuthorBuilder
                {
                    Url = "https://discord.repair",
                    Name = "Discord.Repair",
                    IconUrl = "https://discord.repair/favicon.ico"
                })
                .WithFooter(new EmbedFooterBuilder
                {
                    Text = $"Issued by: {context.User.Username} | {context.User.Id}",
                    IconUrl = context.User.GetAvatarUrl()
                })
                .WithDescription(description)
                .WithUrl(url)
                .WithThumbnailUrl(imageUrl)
                .WithCurrentTimestamp();

            // If there are fields to be added, add them.
            if (embeds is not null)
            {
                embedBuilder.WithFields(embeds);
            }

            // Build the embed object.
            var embed = embedBuilder.Build();

            // If the context is not a RestInteractionContext, throw an exception.
            if (context is not Discord.Rest.RestInteractionContext restContext)
            {
                throw new ArgumentNullException(nameof(restContext), "Failed to convert context to a sharded context.");
            }

            // If the interaction has already been responded to, modify the original response with the new embed.
            if (restContext.Interaction.HasResponded)
            {
                await context.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed);
            }
            // Otherwise, respond to the interaction with the new embed.
            else
            {
                await context.Interaction.RespondAsync(embed: embed, ephemeral: invisible);
            }

            // If a delete timer has been set and the response message is not invisible, start a task to delete the response message after the specified time.
            if (deleteTimer is not null && invisible is false)
            {
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds((int)deleteTimer));
                    IUserMessage? msg = await context.Interaction.GetOriginalResponseAsync();
                    await msg.DeleteAsync();
                }, TaskCreationOptions.LongRunning);
            }
        }
        catch (Exception ex)
        {
            // Log the exception.
        }
    }

    /// <summary>
    /// Sends a message with an embedded content to the given channel.
    /// </summary>
    /// <param name="channel">The channel where the message will be sent.</param>
    /// <param name="title">The title of the embedded message.</param>
    /// <param name="description">The description of the embedded message.</param>
    /// <param name="footer">The text of the footer of the embedded message.</param>
    /// <param name="footerIcon">The icon url of the footer of the embedded message.</param>
    /// <param name="embeds">Optional list of EmbedFieldBuilders to add to the embedded message.</param>
    /// <param name="deleteTimer">Optional time, in seconds, to wait before deleting the message. If not provided, the message will not be deleted.</param>
    /// <returns>A task that represents the asynchronous operation of sending the message.</returns>
    internal static async Task SendEmbedAsync(this IChannel channel, string title, string description, string footer, string footerIcon = "https://i.imgur.com/Nfy4OoG.png", List<EmbedFieldBuilder>? embeds = null, int? deleteTimer = null)
    {
        // Create an EmbedBuilder object and set its properties based on the parameters.
        var embedBuilder = new EmbedBuilder()
        .WithTitle(title)
        .WithColor(Miscallenous.RandomDiscordColour())
        .WithAuthor(new EmbedAuthorBuilder
        {
            Url = "https://discord.repair",
            Name = "Discord.Repair",
            IconUrl = "https://i.imgur.com/Nfy4OoG.png"
        })
        .WithFooter(new EmbedFooterBuilder
        {
            Text = footer,
            IconUrl = footerIcon
        })
        .WithDescription(description)
        .WithCurrentTimestamp();

        // If embeds were provided, add them to the EmbedBuilder.
        if (embeds is not null)
        {
            embedBuilder.WithFields(embeds);
        }

        // Build the final embed.
        var embed = embedBuilder.Build();

        // Send the message to the appropriate channel based on its type.
        IUserMessage? msg = channel switch
        {
            ITextChannel textChannel => await textChannel.SendMessageAsync(embed: embed),
            IDMChannel dmChannel => await dmChannel.SendMessageAsync(embed: embed),
            _ => null,
        };

        // If a delete timer was provided and the message was sent successfully, schedule the message to be deleted after the specified time has passed.
        if (deleteTimer is not null && msg is not null)
        {
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds((int)deleteTimer));
                await msg.DeleteAsync();
            }, TaskCreationOptions.LongRunning);
        }
    }

    /// <summary>
    /// Verifies that the provided guild ID, user ID, and token are valid.
    /// </summary>
    /// <param name="base">The controller base instance.</param>
    /// <param name="guildId">The ID of the guild to verify.</param>
    /// <param name="userId">The ID of the user to verify.</param>
    /// <param name="token">The token to verify.</param>
    /// <returns>
    /// An <see cref="ActionResult"/> if any of the parameters are invalid; otherwise, <see langword="null"/>.
    /// </returns>
    internal static ActionResult? VerifyServer(this ControllerBase @base, ulong guildId, ulong? userId, string token)
    {
        return guildId == 0
            ? @base.BadRequest(new Generic { success = false, details = "Invalid guild ID. Please try again." })
            : string.IsNullOrWhiteSpace(token)
            ? @base.BadRequest(new Generic { success = false, details = "Invalid token." })
            : userId is null or 0
            ? @base.BadRequest(new Generic { success = false, details = "Invalid user ID. Please try again." })
            : (ActionResult?)null;
    }

    /// <summary>
    /// Verifies that the provided guild ID and token are valid.
    /// </summary>
    /// <param name="base">The controller base instance.</param>
    /// <param name="guildId">The ID of the guild to verify.</param>
    /// <param name="token">The token to verify.</param>
    /// <returns>
    /// An <see cref="ActionResult"/> if any of the parameters are invalid; otherwise, <see langword="null"/>.
    /// </returns>
    internal static ActionResult? VerifyServer(this ControllerBase @base, ulong guildId, string token)
    {
        return guildId == 0
            ? @base.BadRequest(new Generic { success = false, details = "Invalid guild ID. Please try again." })
            : string.IsNullOrWhiteSpace(token) ? @base.BadRequest(new Generic { success = false, details = "Invalid token." }) : (ActionResult?)null;
    }

    /// <summary>
    /// Verify the existence and status of a server in the database.
    /// </summary>
    /// <param name="base">The controller base to extend.</param>
    /// <param name="database">The database context.</param>
    /// <param name="guildId">The ID of the server to verify.</param>
    /// <param name="token">The API token associated with the server.</param>
    /// <returns>A tuple containing the HTTP result and the server object.</returns>
    internal static async ValueTask<(ActionResult? httpResult, Server? server)> VerifyServer(this ControllerBase @base, DatabaseContext database, ulong guildId, string token)
    {
        // Query the database to find the server with the specified guild ID and API token.
        Server? server = await database.servers.FirstOrDefaultAsync(x => x.guildId == guildId && x.owner.apiToken == token);

        // If the server cannot be found in the database, return a 400 Bad Request HTTP result and null server object.
        if (server is null)
        {
            return ((ActionResult? httpResult, Server? server))(@base.BadRequest(new Generic() { success = false, details = "Server does not exist. Please try again." }), null);
        }

        // If the server is banned, return a 400 Bad Request HTTP result and null server object.
        if (server.banned)
        {
            return (@base.BadRequest(new Generic() { success = false, details = "Server is banned." }), null);
        }

        // Otherwise, return null HTTP result and the server object.
        return (null, server);
    }

    /// <summary>
    /// Verify the validity of the parameters for creating a new server.
    /// </summary>
    /// <param name="base">The controller base to extend.</param>
    /// <param name="serverName">The name of the server to create.</param>
    /// <param name="userId">The ID of the user creating the server.</param>
    /// <param name="token">The API token associated with the user.</param>
    /// <returns>An HTTP result if there is an error, otherwise null.</returns>
    internal static ActionResult? VerifyServer(this ControllerBase @base, string? serverName, ulong userId, string token)
    {
        // If the server name or user ID is invalid, return a 400 Bad Request HTTP result.
        if (string.IsNullOrWhiteSpace(serverName) || userId is 0)
        {
            return @base.BadRequest(new Generic() { success = false, details = "Invalid parameters. Please try again." });
        }

        // If the server name is too long, return a 400 Bad Request HTTP result.
        if (serverName.Length > 64)
        {
            return @base.BadRequest(new Generic() { success = false, details = "Invalid parameter. Please try again." });
        }

        // If the API token is invalid, return a 400 Bad Request HTTP result.
        if (string.IsNullOrWhiteSpace(token))
        {
            return @base.BadRequest(new Generic() { success = false, details = "Invalid token." });
        }

        // Otherwise, return null HTTP result.
        return null;
    }

    /// <summary>
    /// Verifies server information based on the provided server name and API token.
    /// </summary>
    /// <param name="base">The base controller to return a result from.</param>
    /// <param name="serverName">The name of the server to verify.</param>
    /// <param name="token">The API token to verify.</param>
    /// <returns>An ActionResult object if the server name or token is invalid, otherwise null.</returns>
    internal static ActionResult? VerifyServer(this ControllerBase @base, string? serverName, string token)
    {
        return string.IsNullOrWhiteSpace(serverName)
            ? @base.BadRequest(new Generic() { success = false, details = "invalid parameters, please try again." })
            : serverName.Length > 64
            ? @base.BadRequest(new Generic()
            {
                success = false,
                details = "invalid parameter, please try again."
            })
            : string.IsNullOrWhiteSpace(token) ? @base.BadRequest(new Generic() { success = false, details = "invalid token." }) : (ActionResult?)null;
    }

    /// <summary>
    /// Verifies server information based on the provided server name and API token from a given database context.
    /// </summary>
    /// <param name="base">The base controller to return a result from.</param>
    /// <param name="database">The database context to search for the server.</param>
    /// <param name="serverName">The name of the server to verify.</param>
    /// <param name="token">The API token to verify.</param>
    /// <returns>A tuple of an ActionResult object and a Server object, where the ActionResult is not null if the server name or token is invalid, and the Server is not null if the server exists and is not banned.</returns>
    internal static async ValueTask<(ActionResult? httpResult, Server? server)> VerifyServer(this ControllerBase @base, DatabaseContext database, string serverName, string token)
    {
        Server? server = await database.servers.FirstOrDefaultAsync(x => (x.name == serverName || x.key.ToString() == serverName) && x.owner.apiToken == token);
        return server is null
            ? ((ActionResult? httpResult, Server? server))(@base.BadRequest(new Generic() { success = false, details = "server does not exist, please try again." }), null)
            : server.banned
            ? (@base.BadRequest(new Generic() { success = false, details = "server is banned." }), null)
            : (null, server);
    }

}
//internal static async ValueTask<bool> IsGuildBusy(ulong guildId)
//{
//    try
//    {
//        await using var database = new DatabaseContext();
//        return await database.statistics.OrderByDescending(x => x.active).FirstOrDefaultAsync(x => x.guildId == guildId && x.active) is not null;
//    }
//    catch (Exception ex)
//    {
//        await ex.LogErrorAsync();
//        return true;
//    }
//}

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
//    Discord.Rest.RestRole? isRoleAboveMe = user.RoleIds.FirstOrDefault(x => x. > serverRole.Position);
//    if (isRoleAboveMe is null)
//    {
//        await context.ReplyWithEmbedAsync("Error Occurred", $"Bot role is not above {serverRole.Mention}", invisible: true, deleteTimer: 60);
//        return false;
//    }
//    return true;
//}

//internal static bool IsVerifyServerOkay(this Server server) => server is not null && server.banned is false;

//internal static async ValueTask<bool> IsTopRoleServerOkay(Server server, Discord.Rest.RestInteractionContext context)
//{
//    if (server is null)
//    {
//        await context.ReplyWithEmbedAsync("Error Occurred", "This guild does not exist in our database, please try again.", invisible: true, deleteTimer: 60);
//        return false;
//    }
//    if (server.banned)
//    {
//        await context.ReplyWithEmbedAsync("Error Occurred", "This guild has been banned from using the bot.", invisible: true, deleteTimer: 60);
//        return false;
//    }
//    return true;
//}
