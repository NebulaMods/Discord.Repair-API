using Discord.WebSocket;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using RestoreCord.Database;
using RestoreCord.Database.Models;
using RestoreCord.MigrationMaster.Models;
using RestoreCord.Records.Responses;
using RestoreCord.Utilities;

namespace RestoreCord.Endpoints.V1.Guild;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/guild/")]
[ApiExplorerSettings(GroupName = "Guild Endpoints")]
public class Migrate : ControllerBase
{
    private readonly MigrationMaster.Pull _migration;
    private readonly MigrationMaster.Configuration _migrationConfiguration;
    private readonly DiscordShardedClient _client;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="migration"></param>
    /// <param name="client"></param>
    public Migrate(MigrationMaster.Pull migration, DiscordShardedClient client, MigrationMaster.Configuration migrationConfiguration)
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
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Records.Responses.Guild.Migration), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Records.Responses.Guild.Migration>> HandleAsync(ulong guildId, ulong? userId = null)
    {
        await using var database = new DatabaseContext();
        var result = await this.VerifyServer(guildId, database);
        if (result.Item1 is not null)
            return result.Item1;
        if (result.Item2 is null)
            return NoContent();
        Database.Models.Server serverEntry = await database.servers.FirstAsync(x => x.guildId == guildId);

        if (await DiscordExtensions.IsGuildBusy(guildId))
        {
            return BadRequest(new Records.Responses.Guild.Migration()
            {
                success = false,
                details = "guild is busy."
            });
        }
        //check if any users r in linked
        List<Member>? members = await database.members.Where(x => x.server == serverEntry).ToListAsync();
        if  (members is null)
        {
            return BadRequest(new Records.Responses.Guild.Migration()
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
                blacklistedCount = serverEntry.settings.blacklist.Count
            },
            active = true,
            guildId = guildId,
            server = serverEntry,
            MigratedBy = serverEntry.owner
        };
        await database.statistics.AddAsync(statistics);
        await database.ApplyChangesAsync();
        var migrateUsers = Task.Run(async () => await MigrateUsersAsync(guildId, userId));
        MigrationMaster.Configuration? config = _migrationConfiguration;
        config._runningMigrations.TryAdd(statistics, migrateUsers);
        return Ok(new Records.Responses.Guild.Migration()
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
        Database.Models.Server server = await database.servers.FirstAsync(x => x.guildId == guildId);
        MigrationMaster.Pull? mig = _migration;
        DiscordShardedClient? discordClient = _client;
        HttpClient? http = await mig.CreateHttpClientAsync(server.settings.mainBot is not null ? server.settings.mainBot.token : Properties.Resources.Token, server.settings.mainBot is not null ? server.name : "RestoreCord");

        try
        {
            //do checks on server entry
            if (userId is not null)
            {
                Member? member = await database.members.FirstOrDefaultAsync(x => x.discordId == userId && x.server == server);
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
            MigrationMaster.Configuration? config = _migrationConfiguration;
            config._runningMigrations.TryRemove(statistics, out _);
        }
    }
}
