using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using RestoreCord.Records.Responses;
using RestoreCord.Utilities;

namespace RestoreCord.Endpoints.V1.Guild;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/guild/")]
[ApiExplorerSettings(GroupName = "Guild Endpoints")]
public class Statistics : ControllerBase
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <returns></returns>
    [HttpGet("{guildId}/stats")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Records.Responses.Guild.Statistics), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Records.Responses.Guild.Statistics>> HandleAsync(ulong guildId)
    {
        await using var database = new Database.DatabaseContext();
        var result = await this.VerifyServer(guildId, database);
        if (result.Item1 is not null)
            return result.Item1;
        if (result.Item2 is null)
            return NoContent();
        Database.Models.LogModels.Statistics? latestEntry = await database.statistics.OrderBy(x => x.key).LastOrDefaultAsync(x => x.guildId == guildId);
        return latestEntry is null
            ? NotFound(new Generic()
            {
                success = false,
                details = "no migration history"
            })
            : Ok(new Records.Responses.Guild.Statistics()
            {
                active = latestEntry.active,
                guildId = latestEntry.guildId,
                guildStats = latestEntry.guildStats,
                memberStats = latestEntry.memberStats,
                MigratedBy = latestEntry.MigratedBy.username,
                serverId = latestEntry.server.key,
                startDate = latestEntry.startDate,
                endDate = latestEntry.endDate
            });
    }
}
