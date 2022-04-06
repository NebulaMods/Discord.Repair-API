using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using RestoreCord.Records.Responses;
using RestoreCord.Utilities;

namespace RestoreCord.Endpoints;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/guild/")]
[ApiExplorerSettings(GroupName = "Guild Endpoints")]
public class Statistics : ControllerBase
{
    private readonly OldMigration _migration;
    private readonly DiscordShardedClient _client;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="migration"></param>
    /// <param name="client"></param>
    public Statistics(OldMigration migration, DiscordShardedClient client)
    {
        _migration = migration;
        _client = client;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <returns></returns>
    [HttpGet("{guildId}/stats")]
    public async Task<ActionResult<GuildStatistics>> GetStatisticsAsync(ulong guildId)
    {
        if (guildId is 0)
        {
            return BadRequest(new GenericResponse()
            {
                success = false,
                details = "invalid guild id"
            });
        }

        if (await _migration.IsGuildBusy(guildId) is false)
        {
            return NotFound(new GenericResponse()
            {
                success = false,
                details = "guild has nothing going on right now"
            });
        }

        return Ok(new GuildStatistics()
        {
            guildStats = _migration.ActiveGuildMigrations.FirstOrDefault(x => x.Key == guildId).Value,
            memberStats = _migration.ActiveMemberMigrations.FirstOrDefault(x => x.Key == guildId).Value,
        });
    }
}
