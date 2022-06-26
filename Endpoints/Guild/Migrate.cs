using Discord.WebSocket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoreCord.Database;
using RestoreCord.Database.Models;
using RestoreCord.Records.Discord;
using RestoreCord.Records.Responses;
using RestoreCord.Utilities;

namespace RestoreCord.Endpoints.Guild;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/guild/")]
[ApiExplorerSettings(GroupName = "Guild Endpoints")]
[AllowAnonymous]
public class Migrate : ControllerBase
{
    private readonly Migrations.Pull _migration;
    private readonly Migrations.Configuration _migrationConfiguration;
    private readonly DiscordShardedClient _client;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="migration"></param>
    /// <param name="client"></param>
    public Migrate(Migrations.Pull migration, DiscordShardedClient client, Migrations.Configuration migrationConfiguration)
    {
        _migration = migration;
        _client = client;
        _migrationConfiguration = migrationConfiguration;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpPost("{guildId}/migrate")]
    public async Task<ActionResult<Migration>> MigrateAsync(ulong guildId, ulong? userId = null)
    {
        await using var database = new DatabaseContext();
        Server? server = await Miscallenous.VerifyServer(this, guildId, database);
        if (server is null)
            return NoContent();

        if (await DiscordExtensions.IsGuildBusy(guildId))
        {
            return BadRequest(new Migration()
            {
                success = false,
                details = "guild is busy."
            });
        }
        //check if any users r in linked
        List<Member>? members = await database.members.Where(x => x.guildId == guildId).ToListAsync();
        if  (members is null)
        {
            return BadRequest(new Migration()
            {
                success = false,
                details = "no members."
            });
        }
        var statistics = new Database.Models.LogModels.Statistics()
        {
            memberStats = new Database.Models.Statistics.MemberMigration
            {
                totalCount = members.Count,
                estimatedCompletionTime = DateTime.Now.AddSeconds(members.Count * 1.2),
                blacklistedCount = server.settings.blacklist.Count
            },
            active = true,
            guildId = guildId,
            server = server,
            MigratedBy = server.owner
        };
        await database.statistics.AddAsync(statistics);
        await database.ApplyChangesAsync();
        var migrateUsers = Task.Run(async () => await MigrateUsersAsync(guildId, userId));
        Migrations.Configuration? config = _migrationConfiguration;
        config._runningMigrations.TryAdd(statistics, migrateUsers);
        return Ok(new Migration()
        {
            success = true,
            memberStats = statistics.memberStats
        });
    }
    private async Task MigrateUsersAsync(ulong guildId, ulong? userId = null)
    {
        await using var database = new DatabaseContext();
        Database.Models.LogModels.Statistics? statistics = await database.statistics.FirstOrDefaultAsync(x => x.active && x.guildId == guildId);
        if (statistics is null)
        {
            return;
        }
        Server server = await database.servers.FirstAsync(x => x.guildId == guildId);
        Migrations.Pull? mig = _migration;
        DiscordShardedClient? discordClient = _client;
        HttpClient? http = await mig.CreateHttpClientAsync(server.settings.mainBot is not null ? server.settings.mainBot.token : Properties.Resources.Token, server.settings.mainBot is not null ? server.name : "RestoreCord");

        try
        {
            //do checks on server entry
            if (userId is not null)
            {
                Member? member = await database.members.FirstOrDefaultAsync(x => x.discordId == userId && x.guildId == guildId);
                if (member is null)
                {
                    return;
                }
                if (server.settings.blacklist.FirstOrDefault(x => x.discordId == member.discordId) is not null)
                {
                    return;
                }
                ResponseTypes addUserRequest = await _migration.AddUserFunction(member, server, database, http);
                return;
            }
            await _migration.JoinUsersToGuild(database, server, http, statistics.memberStats, guildId);
            return;
        }
        catch (Exception e)
        {
            await e.LogErrorAsync(logToConsole: true);
            return;
        }
        finally
        {
            statistics.active = false;
            await database.ApplyChangesAsync(statistics);
            http.Dispose();
            Migrations.Configuration? config = _migrationConfiguration;
            config._runningMigrations.TryRemove(statistics, out _);
        }
    }
}
