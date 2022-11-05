using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DiscordRepair.Records.Responses;
using DiscordRepair.Utilities;

namespace DiscordRepair.Endpoints.V1.Guild;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/guild/")]
[ApiExplorerSettings(GroupName = "Guild Endpoints")]
public class Statistics : ControllerBase
{

    /// <summary>
    /// Returns the most recent or currently active server history.
    /// </summary>
    /// <param name="guildId"></param>
    /// <remarks>Returns the most recent or currently active server history.</remarks>
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
        var result = await this.VerifyServer(guildId, database, HttpContext.Request.Headers["Authorization"]);
        if (result.Item1 is not null)
            return result.Item1;
        if (result.Item2 is null)
            return NoContent();
        Database.Models.LogModels.Statistics? latestEntry = await database.statistics.OrderBy(x => x.key).LastOrDefaultAsync(x => x.guildId == guildId);
        return latestEntry is null
            ? NotFound(new Generic()
            {
                success = false,
                details = "no migration history."
            })
            : Ok(new Records.Responses.Guild.Statistics()
            {
                active = latestEntry.active,
                guildId = latestEntry.guildId,
                guildStats = latestEntry.guildStats,
                memberStats = latestEntry.memberStats,
                MigratedBy = latestEntry.MigratedBy?.username,
                serverId = latestEntry.server.key,
                startDate = latestEntry.startDate,
                endDate = latestEntry.endDate
            });
    }
}
