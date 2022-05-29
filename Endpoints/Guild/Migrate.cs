using Discord.WebSocket;
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
public class Migrate : ControllerBase
{
    private readonly Migrations.Pull _migration;
    private readonly DiscordShardedClient _client;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="migration"></param>
    /// <param name="client"></param>
    public Migrate(Migrations.Pull migration, DiscordShardedClient client)
    {
        _migration = migration;
        _client = client;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="migrationRequest"></param>
    /// <returns></returns>
    [HttpPost("{guildId}/migrate")]
    public async Task<ActionResult<Records.Responses.Migration>> MigrateAsync(ulong guildId, Records.Requests.Migrate? migrationRequest)
    {
        if (guildId is 0)
        {
            return BadRequest(new GenericResponse()
            {
                success = false,
                details = ""
            });
        }

        if (await DiscordExtensions.IsGuildBusy(guildId))
        {
            return BadRequest();
        }
        _ = Task.Run(async () => await MigrateUsersAsync(guildId, migrationRequest));
        return Ok(new Records.Responses.Migration()
        {

        });

    }
    private async Task MigrateUsersAsync(ulong guildId, Records.Requests.Migrate? migrationRequest)
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
        try
        {
            //do checks on server entry
            var http = new HttpClient();
            Server? server = await database.servers.FirstOrDefaultAsync(x => x.guildid == guildId);
            await database.statistics.AddAsync(statistics);
            await database.ApplyChangesAsync();
            //check if any users r in linked
            if (await database.members.AnyAsync(x => x.server == guildId) is false)
            {
                return;
            }

            if (migrationRequest is not null)
                if (migrationRequest.userId is not null)
                {
                    Member? member = await database.members.FirstOrDefaultAsync(x => x.userid == migrationRequest.userId && x.server == guildId);
                    if (member is null)
                    {
                        return;
                    }
                    if (await database.blacklist.FirstOrDefaultAsync(x => x.userid == member.userid && x.server == guildId) is not null)
                    {
                        return;
                    }
                    ResponseTypes addUserRequest = await _migration.AddUserFunction(member, server, database, http);
                    return;
                }

            //new thread
            //& return
            await _migration.JoinUsersToGuild(database, server, http, statistics.memberStats, _client, guildId);
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
        }
    }
}
