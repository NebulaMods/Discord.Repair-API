using System.Diagnostics;
using System.Reflection;
using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using RestoreCord.Utilities;

namespace RestoreCord.Commands;

public class InformationCommand : InteractionModuleBase<ShardedInteractionContext>
{

    [SlashCommand("information", "Display information about the bot.")]
    public async Task DisplayInformationAsync()
    {
        DateTimeOffset uptime = Process.GetCurrentProcess().StartTime;
        long totalMembers = 0;
        foreach(Discord.WebSocket.SocketGuild? guild in Context.Client.Guilds)
            totalMembers += guild.MemberCount;
        await using var database = new Database.DatabaseContext();

        var fields = new List<EmbedFieldBuilder>()
        {
            new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = "Latency",
                Value = $"{Context.Client.Latency}ms"
            },
            new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = "Version",
                Value = $"{Assembly.GetExecutingAssembly().GetName().Version}"
            },
            new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = "Guild Count",
                Value = $"{Context.Client.Guilds.Count}"
            },
            new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = "Total Member Count",
                Value = totalMembers
            },
            new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = "Active Migrations",
                Value = $"{await database.statistics.Where(x => x.active).CountAsync()}"
            },
            new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = "Uptime",
                Value = $"<t:{uptime.ToUnixTimeSeconds()}:R>"
            },
        };
        Database.Models.LogModels.Statistics? migrationEntry = await database.statistics.FirstOrDefaultAsync(x => x.active && x.guildId == Context.Guild.Id && x.guildStats != null);
        if (migrationEntry is not null)
        {
            fields.Add(new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = "Migration Progress",
                Value = $"Success: {migrationEntry.memberStats.successCount}\n" +
                $"Attempted: {migrationEntry.memberStats.failedCount + migrationEntry.memberStats.alreadyHereCount + migrationEntry.memberStats.bannedCount + migrationEntry.memberStats.invalidTokenCount + migrationEntry.memberStats.tooManyGuildsCount}\n" +
                $"Remaining: {migrationEntry.memberStats.totalCount - (migrationEntry.memberStats.failedCount + migrationEntry.memberStats.alreadyHereCount + migrationEntry.memberStats.bannedCount + migrationEntry.memberStats.invalidTokenCount + migrationEntry.memberStats.tooManyGuildsCount + migrationEntry.memberStats.successCount)}\n" +
                $"Total: {migrationEntry.memberStats.totalCount}"
            });
        }

        Embed? embed = new EmbedBuilder()
        {
            Title = "RestoreCord Information",
            Color = Miscallenous.RandomDiscordColour(),
            Author = new EmbedAuthorBuilder
            {
                Url = "https://restorecord.com",
                Name = "RestoreCord",
                IconUrl = "https://i.imgur.com/Nfy4OoG.png"
            },
            Footer = new EmbedFooterBuilder
            {
                Text = "Bot by Nebula#0911 <3",
                IconUrl = "https://nebulamods.ca/content/media/images/logo-nebulamods.png"
            },
            Fields = fields,
        }.WithCurrentTimestamp().Build();
        await Context.Interaction.RespondAsync(embed: embed);
    }
}
