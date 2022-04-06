using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoreCord.Database;
using RestoreCord.Database.Models;
using RestoreCord.Records;
using RestoreCord.Records.Discord;
using RestoreCord.Records.Responses;
using RestoreCord.Utilities;

namespace RestoreCord.Endpoints;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/guild/")]
[ApiExplorerSettings(GroupName = "Guild Endpoints")]
public class Migrate : ControllerBase
{
    private readonly OldMigration _migration;
    private readonly DiscordShardedClient _client;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="migration"></param>
    /// <param name="client"></param>
    public Migrate(OldMigration migration, DiscordShardedClient client)
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
    public async Task<ActionResult<Records.Responses.Migration>> MigrateAsync(ulong guildId, Records.Requests.Migration migrationRequest)
    {
        return Ok();
        if (migrationRequest is null)
        {
            return base.BadRequest(new Records.Responses.Migration()
            {
                response = Enums.FAILURE
            });
        }

        if (await _migration.IsGuildBusy(guildId))
        {
            return base.BadRequest(new Records.Responses.Migration()
            {
                response = Enums.GUILD_BUSY
            });
        }

        try
        {
            var migrationStats = new OldMigration.MemberStatisitics
            {
                startTime = DateTime.Now,
            };
            _migration.ActiveMemberMigrations.TryAdd(guildId, migrationStats);
            await using var database = new DatabaseContext();

            //do checks on server entry
            Server? server = await database.servers.FirstOrDefaultAsync(x => x.guildid == guildId);
            if (server?.IsVerifyServerOkay() is false)
            {
                return base.BadRequest(new Records.Responses.Migration()
                {
                    response = Enums.INVALID_SERVER
                });
            }
            //check if any users r in linked
            if (await database.members.AnyAsync(x => x.server == guildId) is false)
            {
                return base.BadRequest(new Records.Responses.Migration()
                {
                    response = Enums.NO_MEMBERS
                });
            }
            using var http = new HttpClient();
            _migration.HttpClientSetup(http);
            if (migrationRequest.userId is not null)
            {
                Member? member = await database.members.FirstOrDefaultAsync(x => x.userid == migrationRequest.userId && x.server == guildId);
                if (member is null)
                {
                    return base.BadRequest(new Records.Responses.Migration()
                    {
                        response = Enums.MIGRATE_USER_FAILURE
                    });
                }
                if (await database.blacklist.FirstOrDefaultAsync(x => x.userid == member.userid && x.server == guildId) is not null)
                {
                    return base.BadRequest(new Records.Responses.Migration()
                    {
                        response = Enums.MIGRATE_USER_FAILURE
                    });
                }
                ResponseTypes addUserRequest = await OldMigration.AddUserFunction(member, server, database, http, _migration);
                return addUserRequest switch
                {
                    ResponseTypes.Success => base.Ok(new Records.Responses.Migration()
                    {
                        response = Enums.MIGRATE_USER_SUCCESS
                    }),
                    _ => base.BadRequest(new Records.Responses.Migration()
                    {
                        response = Enums.MIGRATE_USER_FAILURE
                    }),
                };
            }

            //new thread
            //& return
            return await OldMigration.JoinUsersToGuild(database, server, http, migrationStats, _migration, _client, guildId)
                ? base.Ok(new global::RestoreCord.Records.Responses.Migration()
            {
                response = Enums.SUCCESS,
                memberStats = migrationStats
            })
                : base.BadRequest(new global::RestoreCord.Records.Responses.Migration()
            {
                response = Enums.MISSING_PERMISSIONS
            });
        }
        catch (Exception e)
        {
            await e.LogErrorAsync(logToConsole: true);
            return base.BadRequest(new Records.Responses.Migration()
            {
                response = Enums.FAILURE
            });
        }
        finally
        {
            _migration.ActiveMemberMigrations.TryRemove(guildId, out _);
        }
    }
}
