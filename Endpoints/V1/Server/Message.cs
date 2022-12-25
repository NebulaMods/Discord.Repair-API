using System.Drawing;

using Discord;
using Discord.Rest;

using DiscordRepair.Api.Database;
using DiscordRepair.Api.Database.Models;
using DiscordRepair.Api.Records.Discord;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Endpoints.V1.Server;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/server/")]
[ApiExplorerSettings(GroupName = "Server Endpoints")]
public class Message : ControllerBase
{
    /// <summary>
    /// Send a message to a discord channel using the defaault bot for the server.
    /// </summary>
    /// <param name="server"></param>
    /// <param name="channelId"></param>
    /// <param name="message"></param>
    /// <remarks>Send a message to a discord channel using the defaault bot for the server.</remarks>
    /// <returns></returns>
    [HttpPost("{server}/{channelId}/message")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Generic>> HandleAsync(string server, ulong channelId, Records.Requests.Server.Message message)
    {
        if (message is null || string.IsNullOrWhiteSpace(server) || channelId is 0)
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        var verifyResult = this.VerifyServer(server, HttpContext.WhatIsMyToken());
        if (verifyResult is not null)
            return verifyResult;
        await using var database = new DatabaseContext();
        var (httpResult, serverEntry) = await this.VerifyServer(database, server, HttpContext.WhatIsMyToken());
        if (httpResult is not null)
            return httpResult;
        if (serverEntry is null)
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        await using var client = new DiscordRestClient();
        await client.LoginAsync(TokenType.Bot, serverEntry.settings.mainBot.token);
        //login
        var guild = await client.GetGuildAsync(serverEntry.guildId);
        if (guild == null)
            return BadRequest(new Generic()
            {
                success = false,
                details = "guild is invalid."
            });
        var channel = await guild.GetTextChannelAsync(channelId);
        var discordColour = new Discord.Color();
        System.Drawing.Color color = new();
        switch (message.verifyMessage.embedColour?.ToLower())
        {
            case "random":
                discordColour = Miscallenous.RandomDiscordColour();
                break;
            case "black":
                color = ColorTranslator.FromHtml("#000000");
                discordColour = new Discord.Color(color.R, color.G, color.B);
                break;
            case "blue":
                color = ColorTranslator.FromHtml("#0000FF");
                discordColour = new Discord.Color(color.R, color.G, color.B);
                break;
            case "brown":
                color = ColorTranslator.FromHtml("#A52A2A");
                discordColour = new Discord.Color(color.R, color.G, color.B);
                break;
            case "cyan":
                color = ColorTranslator.FromHtml("#00FFFF");
                discordColour = new Discord.Color(color.R, color.G, color.B);
                break;
            case "gold":
                color = ColorTranslator.FromHtml("#FFD700");
                discordColour = new Discord.Color(color.R, color.G, color.B);
                break;
            case "green":
                color = ColorTranslator.FromHtml("#008000");
                discordColour = new Discord.Color(color.R, color.G, color.B);
                break;
            case "pink":
                color = ColorTranslator.FromHtml("#FFC0CB");
                discordColour = new Discord.Color(color.R, color.G, color.B);
                break;
            case "yellow":
                color = ColorTranslator.FromHtml("#FFFF00");
                discordColour = new Discord.Color(color.R, color.G, color.B);
                break;
            case "navy":
                color = ColorTranslator.FromHtml("#000080");
                discordColour = new Discord.Color(color.R, color.G, color.B);
                break;
            case "orange":
                color = ColorTranslator.FromHtml("#FFA500");
                discordColour = new Discord.Color(color.R, color.G, color.B);
                break;
            case "red":
                color = ColorTranslator.FromHtml("#FF0000");
                discordColour = new Discord.Color(color.R, color.G, color.B);
                break;
            case "purple":
                color = ColorTranslator.FromHtml("#800080");
                discordColour = new Discord.Color(color.R, color.G, color.B);
                break;
            default:
                if (string.IsNullOrEmpty(message.verifyMessage.embedColour) || message.verifyMessage.embedColour is "string")
                {
                    discordColour = Miscallenous.RandomDiscordColour();
                    break;
                }
                color = ColorTranslator.FromHtml($"{(message.verifyMessage.embedColour.Contains('#') ? message.verifyMessage.embedColour : $"#{message.verifyMessage.embedColour}")}");
                discordColour = new Discord.Color(color.R, color.G, color.B);
                break;
        }

        MessageComponent? msg = new ComponentBuilder()
        {
            ActionRows = new List<ActionRowBuilder>()
            {
                new ActionRowBuilder()
                {
                    Components = new List<IMessageComponent>
                    {
                        new ButtonBuilder()
                        {
                            Style = ButtonStyle.Link,
                            Label = "Verify",
#if DEBUG
                            Url = $"https://discord.com/oauth2/authorize?client_id={serverEntry.settings.mainBot.clientId}&scope=identify+guilds.join&response_type=code&prompt=none&prompt=none&redirect_uri={Properties.Resources.TestUrlRedirect}&state={serverEntry.key}",
#else
                            Url = $"https://discord.com/oauth2/authorize?client_id={serverEntry.settings.mainBot.clientId}&scope=identify+guilds.join&response_type=code&prompt=none&prompt=none&redirect_uri={Properties.Resources.UrlRedirect}&state={serverEntry.key}",
#endif
                        }.Build(),
                    }
                }
            }
        }.Build();
        Embed? embed = new EmbedBuilder()
        {
            Title = message.verifyMessage.title,
            Color = discordColour,
            Author = new EmbedAuthorBuilder
            {
                Url = "https://discord.repair",
                Name = guild.Name,
                IconUrl = guild.IconUrl
            },
            Footer = new EmbedFooterBuilder
            {
                Text = message.verifyMessage.footerText,
                IconUrl = message.verifyMessage.footerIconUrl
            },
            ImageUrl = message.verifyMessage.imageUrl,
            Description = message.verifyMessage.embedDescription,
        }.Build();
        await channel.SendMessageAsync(embed: embed, components: msg);
        return Ok(new Generic()
        {
            success = true,
            details = "successfully sent message to discord channel."
        });
    }
}
