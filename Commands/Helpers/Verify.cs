using System.Drawing;
using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using RestoreCord.Database;
using RestoreCord.Database.Models;
using RestoreCord.Utilities;
using RestoreCord.Utilities.DiscordAttributes;

namespace RestoreCord.Commands;

[RequireAdministrator]
public class Verify : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("send-verify", "Send the verify embed to the specified channel.")]
    public async Task SendVerifyEmbedAsync(IChannel channel, string? embedDescription = null, string? imageUrl = null, string? embedColour = "random")
    {
        if (channel is not ITextChannel textChannel)
        {
            await Context.ReplyWithEmbedAsync("Error Occurred", "The specified channel is not a text channel, please try again.", invisible: true, deleteTimer: 60);
            return;
        }

        await using var database = new DatabaseContext();
        Server? server = await database.servers.FirstOrDefaultAsync(x => x.guildid == Context.Guild.Id);
        if (server is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occurred", "This guild does not exist in our database, please try again.", invisible: true, deleteTimer: 60);
            return;
        }
        if (string.IsNullOrWhiteSpace(server.banned) is false)
        {
            await Context.ReplyWithEmbedAsync("Error Occurred", "This guild has been banned from using the bot.", invisible: true, deleteTimer: 60);
            return;
        }
        var discordColour = new Discord.Color();
        System.Drawing.Color color = new();
        switch (embedColour)
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
                if (string.IsNullOrEmpty(embedColour))
                {
                    discordColour = Miscallenous.RandomDiscordColour();
                    break;
                }
                color = ColorTranslator.FromHtml($"{(embedColour.Contains('#') ? embedColour : $"#{embedColour}")}");
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
                            Url = $"https://discord.com/oauth2/authorize?client_id=791106018175614988&scope=identify+guilds.join&response_type=code&prompt=none&prompt=none&redirect_uri=https://restorecord.com/auth/&state={Context.Guild.Id}",
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
                Url = "https://restorecord.com",
                Name = Context.Guild.Name,
                IconUrl = Context.Guild.IconUrl
            },
            Footer = new EmbedFooterBuilder
            {
                Text = "RestoreCord",
                IconUrl = "https://i.imgur.com/Nfy4OoG.png"
            },
            ImageUrl = imageUrl,
            Description = string.IsNullOrWhiteSpace(embedDescription) is false ? embedDescription : "Click the \"Verify\" button and press Authorize to view the rest of the server",
        }.Build();
        Task<IUserMessage>? task1 = textChannel.SendMessageAsync(embed: embed, components: msg);
        Task? task2 = Context.ReplyWithEmbedAsync("Verify Embed", $"Successfully sent verify embed to {textChannel.Mention}", invisible: true, deleteTimer: 60);
        await Task.WhenAll(task1, task2);
    }
}
