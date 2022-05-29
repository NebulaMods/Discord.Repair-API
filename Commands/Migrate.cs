using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using RestoreCord.Database;
using RestoreCord.Database.Models;
using RestoreCord.Records.Discord;
using RestoreCord.Utilities;
using RestoreCord.Utilities.DiscordAttributes;

namespace RestoreCord.Commands;

[RequireAdministrator]
public class Migrate : InteractionModuleBase<ShardedInteractionContext>
{

    [SlashCommand("migrate", "Migrate all discord members from the database into this guild.")]
    public async Task MigrateMembersAsync(string? userId = null)
    {
        await using var database = new DatabaseContext();
        var statistics = new Database.Models.LogModels.Statistics()
        {
            memberStats = new Database.Models.Statistics.MemberMigration
            {
                startTime = DateTime.Now,

            },
            active = true
        };
        await database.statistics.AddAsync(statistics);
        try
        {
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
            if (await DiscordExtensions.IsGuildBusy(Context.Guild.Id))
            {
                await Context.ReplyWithEmbedAsync("Error Occurred", "This guild has been banned from using the bot.", invisible: true, deleteTimer: 60);
                return;
            }
        }
        catch (Exception e)
        {
            await e.LogErrorAsync();
        }
        finally
        {
            statistics.active = false;
            await database.ApplyChangesAsync(statistics);
        }
        //try
        //{
        //    await Context.Interaction.DeferAsync();
        //    using var apiClient = new HttpClient();

        //    //single user
        //    ulong singleUser2 = 0;
        //    if (string.IsNullOrWhiteSpace(userId) is false)
        //    {
        //        if (ulong.TryParse(userId, out singleUser2) is false)
        //        {
        //            await Context.ReplyWithEmbedAsync("Error Occurred", "Failed to parse the specified user ID, please try again.", invisible: true, deleteTimer: 60);
        //            return;
        //        }
        //    }
        //    HttpContent content = new StringContent(JsonConvert.SerializeObject(new MigrationRequest()
        //    {
        //        userId = singleUser2 == 0 ? null : singleUser2
        //    }));
        //    content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
        //    HttpResponseMessage? response = null;
        //    if (singleUser2 == 0)
        //    {
        //        var task1 = apiClient.PostAsync($"http://localhost:666/guild/migrate/{Context.Guild.Id}", content);
        //        await Task.Delay(TimeSpan.FromSeconds(1));
        //        var quickStats = await apiClient.GetStringAsync($"http://localhost:666/guild/stats/{Context.Guild.Id}");
        //        if (quickStats is null)
        //        {

        //        }
        //        else
        //        {
        //            var currentStats = JsonConvert.DeserializeObject<Statistics>(quickStats);
        //            DateTimeOffset estimatedCompletionTime = DateTime.Now.AddSeconds(currentStats.memberStats.totalCount * 1.2);
        //            await Context.ReplyWithEmbedAsync("Migration Status", $"Attempting to possibly migrate {currentStats.memberStats.totalCount} members to this guild.\nEstimated completion: <t:{estimatedCompletionTime.ToUnixTimeSeconds()}:R>, please be patient.");
        //        }
        //        response = await task1;
        //    }
        //    if (response is null)
        //        return;
        //    var jsonInfo = JsonConvert.DeserializeObject<MigrationResponse>(await response.Content.ReadAsStringAsync());
        //    if (jsonInfo is null)
        //    {
        //        await Context.ReplyWithEmbedAsync("Error Occurred", "Generic error message for now", invisible: true, deleteTimer: 60);
        //        return;
        //    }
        //    switch (jsonInfo.response)
        //    {
        //        case ResponseEnums.MIGRATE_USER_SUCCCESS:
        //            await Context.ReplyWithEmbedAsync("Migration Status", $"Successfully migrated <@{singleUser2}> to this guild.");
        //            return;
        //        case ResponseEnums.MIGRATE_USER_FAILURE:
        //            await Context.ReplyWithEmbedAsync("Error Occurred", "Failed to migrate the specified user, please try again.", invisible: true, deleteTimer: 60);
        //            return;
        //        case ResponseEnums.GUILD_BUSY:
        //            await Context.ReplyWithEmbedAsync("Error Occurred", "This guild is currently in the process of migration/backup/restore, please wait until the process is complete before starting another.", invisible: true, deleteTimer: 60);
        //            return;
        //        case ResponseEnums.FAILURE:
        //            await Context.ReplyWithEmbedAsync("Error Occurred", "An Error Occurred while trying to migrate users to this guild, please try again. If this error occurs again please report this to an administrator, thank you.", deleteTimer: 60);
        //            return;
        //        case ResponseEnums.MISSING_PERMISSIONS:
        //            await Context.ReplyWithEmbedAsync("Error Occurred", "Please check to make sure the bot has manage roles, create invites, & ban permissions. Also make sure, if specified, that the verify role is below the bot's role.", deleteTimer: 60);
        //            return;
        //        case ResponseEnums.INVALID_SERVER:
        //            await Context.ReplyWithEmbedAsync("Error Occurred", "This guild is either banned or does not exist in or database, please try again.", invisible: true, deleteTimer: 60);
        //            return;
        //        case ResponseEnums.NO_MEMBERS:
        //            await Context.ReplyWithEmbedAsync("Error Occurred", "This guild does not contain any members in our database, please try again.", invisible: true, deleteTimer: 60);
        //            return;
        //    }
        //    var statistics = jsonInfo.memberStats;
        //    #region embeds for stats
        //    List<EmbedFieldBuilder> embeds = new();
        //    embeds.Add(new EmbedFieldBuilder()
        //    {
        //        IsInline = true,
        //        Name = "Attempted",
        //        Value = statistics.totalCount - statistics.bannedCount - statistics.alreadyHereCount
        //    });
        //    if (statistics.successCount > 0)
        //    {
        //        embeds.Add(new EmbedFieldBuilder()
        //        {
        //            IsInline = true,
        //            Name = "Successful",
        //            Value = statistics.successCount
        //        });
        //    }

        //    if (statistics.failedCount > 0)
        //    {
        //        embeds.Add(new EmbedFieldBuilder()
        //        {
        //            IsInline = true,
        //            Name = "Failed",
        //            Value = statistics.failedCount
        //        });
        //    }

        //    if (statistics.bannedCount > 0)
        //    {
        //        embeds.Add(new EmbedFieldBuilder()
        //        {
        //            IsInline = true,
        //            Name = "Banned/Blacklisted",
        //            Value = statistics.bannedCount
        //        });
        //    }

        //    if (statistics.tooManyGuildsCount > 0)
        //    {
        //        embeds.Add(new EmbedFieldBuilder()
        //        {
        //            IsInline = true,
        //            Name = "Too Many Guilds",
        //            Value = statistics.tooManyGuildsCount
        //        });
        //    }

        //    if (statistics.invalidTokenCount > 0)
        //    {
        //        embeds.Add(new EmbedFieldBuilder()
        //        {
        //            IsInline = true,
        //            Name = "Invalid/Unauthed",
        //            Value = statistics.invalidTokenCount
        //        });
        //    }

        //    if (statistics.alreadyHereCount > 0)
        //    {
        //        embeds.Add(new EmbedFieldBuilder()
        //        {
        //            IsInline = true,
        //            Name = "Already Here",
        //            Value = statistics.alreadyHereCount
        //        });
        //    }

        //    embeds.Add(new EmbedFieldBuilder()
        //    {
        //        IsInline = true,
        //        Name = "Database",
        //        Value = statistics.totalCount
        //    });
        //    embeds.Add(new EmbedFieldBuilder()
        //    {
        //        IsInline = true,
        //        Name = "Time Elapsed",
        //        Value = statistics.totalTime.TotalSeconds > 60 ? $"{statistics.totalTime.Minutes} Minutes & {statistics.totalTime.Seconds} Seconds" : $"{Math.Round(statistics.totalTime.TotalSeconds, 2)} Seconds"
        //    });
        //    #endregion

        //    if (statistics.successCount > 0)
        //    {
        //        Discord.Rest.RestInteractionMessage? msg = await Context.Interaction.GetOriginalResponseAsync();
        //        if (msg is not null)
        //            await msg.DeleteAsync();
        //        if (statistics.successCount == (statistics.totalCount - statistics.bannedCount - statistics.alreadyHereCount))
        //        {
        //            await Context.Channel.SendEmbedAsync("Migration Status", $"Succesfully migrated all members from the database to this guild.", "RestoreCord made with <3", "https://i.imgur.com/Nfy4OoG.png", embeds: embeds);
        //            return;
        //        }
        //        await Context.Channel.SendEmbedAsync("Migration Status", $"Succesfully migrated some members from the database to this guild.\n" +
        //        "This could mean multiple things such as: members unlinking, being banned from the guild, or other restrictions discord put in place.", "RestoreCord made with <3", "https://i.imgur.com/Nfy4OoG.png", embeds: embeds);
        //        return;
        //    }
        //    await Context.ReplyWithEmbedAsync("Migration Status", $"Failed to migrate members from the database to this guild.\n" +
        //    "This could mean multiple things such as: members unlinking, being banned from the guild, or other restrictions discord put in place.", embeds: embeds);
        //}
        //catch(Exception e)
        //{
        //    await Context.ReplyWithEmbedAsync("Error Occurred", "An Error Occurred while trying to migrate users to this guild, please try again. If this error occurs again please report this to an administrator, thank you.", deleteTimer: 60);
        //    await e.LogErrorAsync(logToConsole: true);
        //}
        //if (await DiscordExtensions.IsGuildBusy(_migration, Context))
        //    return;
        //try
        //{
        //    var migrationStats = new Migration.MemberStatisitics
        //    {
        //        startTime = DateTime.Now,
        //    };
        //    _migration.ActiveMemberMigrations.TryAdd(Context.Guild.Id, migrationStats);
        //    await using var database = new DatabaseContext();

        //    //do checks on server entry
        //    Server? server = await database.servers.FirstOrDefaultAsync(x => x.guildid == Context.Guild.Id);
        //    if (await DiscordExtensions.IsVerifyServerOkay(server, Context) is false)
        //        return;
        //    //check if any users r in linked
        //    if (await database.members.AnyAsync(x => x.server == Context.Guild.Id) is false)
        //    {
        //        await Context.ReplyWithEmbedAsync("Error Occurred", "This guild does not contain any members in our database, please try again.", invisible: true, deleteTimer: 60);
        //        return;
        //    }
        //    using var http = new HttpClient();
        //    _migration.HttpClientSetup(http);
        //    if (string.IsNullOrWhiteSpace(userId) is false)
        //    {
        //        if (ulong.TryParse(userId, out ulong singleUser))
        //        {
        //            Member? member = await database.members.FirstOrDefaultAsync(x => x.userid == singleUser && x.server == Context.Guild.Id);
        //            if (member is null)
        //            {
        //                await Context.ReplyWithEmbedAsync("Error Occurred", "Failed to find the user specificed, please try again.", invisible: true, deleteTimer: 60);
        //                return;
        //            }
        //            if (await database.blacklist.FirstOrDefaultAsync(x => x.userid == member.userid && x.server == Context.Guild.Id) is not null)
        //            {
        //                await Context.ReplyWithEmbedAsync("Error Occurred", "Failed to migrate the specified user as they are blacklisted from this guild.", invisible: true, deleteTimer: 60);
        //                return;
        //            }
        //            ResponseTypes addUserRequest = await AddUserFunction(member, server, database, http, _migration);
        //            switch (addUserRequest)
        //            {
        //                case ResponseTypes.Success:
        //                    await Context.ReplyWithEmbedAsync("Migration Status", $"Successfully migrated <@{member.userid}> to this guild.");
        //                    return;
        //                default:
        //                    await Context.ReplyWithEmbedAsync("Error Occurred", "Failed to migrate the specified user, please try again.", invisible: true, deleteTimer: 60);
        //                    return;
        //            }
        //        }
        //        await Context.ReplyWithEmbedAsync("Error Occurred", "Failed to parse the user specificed, please try again.", invisible: true, deleteTimer: 60);
        //        return;
        //    }
        //    await JoinUsersToGuild(database, server, http, Context, migrationStats, _migration);
        //}
        //catch (Exception e)
        //{
        //    await Context.ReplyWithEmbedAsync("Error Occurred", "An Error Occurred while trying to migrate users to this guild, please try again. If this error occurs again please report this to an administrator, thank you.", deleteTimer: 60);
        //    await e.LogErrorAsync(logToConsole: true);
        //}
        //finally
        //{
        //    Context.Guild.PurgeUserCache();
        //    _migration.ActiveMemberMigrations.TryRemove(Context.Guild.Id, out _);
        //}
    }
}
