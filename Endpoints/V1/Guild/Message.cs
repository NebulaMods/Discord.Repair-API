using System.Drawing;

using Discord;
using Discord.Rest;

using Microsoft.AspNetCore.Mvc;

using DiscordRepair.Database;
using DiscordRepair.Records.Responses;
using DiscordRepair.Utilities;

namespace DiscordRepair.Endpoints.V1.Guild;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/guild/")]
[ApiExplorerSettings(GroupName = "Guild Endpoints")]
public class Message : ControllerBase
{
    /// <summary>
    /// Send a message to a discord channel using the defaault bot for the server/guild.
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="channelId"></param>
    /// <param name="message"></param>
    /// <remarks>Send a message to a discord channel using the defaault bot for the server/guild.</remarks>
    /// <returns></returns>
    [HttpPost("{guildId}/{channelId}/message")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Generic>> HandleAsync(ulong guildId, ulong channelId, Records.Requests.Guild.Message message)
    {
        if (message is null || guildId is 0 || channelId is 0)
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        await using var client = new DiscordRestClient();
        await using var database = new DatabaseContext();
        var result = await this.VerifyServer(guildId, database, HttpContext.Request.Headers["Authorization"]);
        if (result.Item1 is not null)
            return result.Item1;
        if (result.Item2 is null)
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        await client.LoginAsync(TokenType.Bot, result.Item2.settings.mainBot.token);
        //login
        var guild = await client.GetGuildAsync(guildId);
        if (guild == null) 
            return BadRequest(new Generic()
            {
                success = false,
                details = "guild is invalid."
            });
        var channel = await guild.GetTextChannelAsync(channelId);
        var discordColour = new Discord.Color();
        System.Drawing.Color color = new();
        switch (message.verifyMessage.embedColour)
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
                if (string.IsNullOrEmpty(message.verifyMessage.embedColour))
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
                            Url = $"https://discord.com/oauth2/authorize?client_id={result.Item2.settings.mainBot.clientId}&scope=identify+guilds.join&response_type=code&prompt=none&prompt=none&redirect_uri={Properties.Resources.UrlRedirect}&state={guildId}",
                        }.Build(),
                    }
                }
            }
        }.Build();
        Embed? embed = new EmbedBuilder()
        {
            Title = "Verification",
            Color = discordColour,
            Author = new EmbedAuthorBuilder
            {
                Url = "https://discord.repair",
                Name = guild.Name,
                IconUrl = guild.IconUrl
            },
            Footer = new EmbedFooterBuilder
            {
                Text = "Discord Repair",
                IconUrl = "https://discord.repair/content/logo.png"
            },
            ImageUrl = message.verifyMessage.imageUrl,
            Description = string.IsNullOrWhiteSpace(message.verifyMessage.embedDescription) is false ? message.verifyMessage.embedDescription : "Click the \"Verify\" button and press Authorize to view the rest of the server",
        }.Build();
        await channel.SendMessageAsync(embed: embed, components: msg);
        return Ok(new Generic()
        {
            success = true,
            details = "successfully sent message to discord channel."
        });
    }
}
