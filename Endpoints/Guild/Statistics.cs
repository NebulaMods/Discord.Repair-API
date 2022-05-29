using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoreCord.Records.Responses;

namespace RestoreCord.Endpoints.Guild;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/guild/")]
[ApiExplorerSettings(GroupName = "Guild Endpoints")]
public class Statistics : ControllerBase
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <returns></returns>
    [HttpGet("{guildId}/stats")]
    public async Task<ActionResult<Database.Models.LogModels.Statistics>> GetStatisticsAsync(ulong guildId)
    {
        if (guildId is 0)
        {
            return BadRequest(new GenericResponse()
            {
                success = false,
                details = "invalid guild id"
            });
        }
        await using var database = new Database.DatabaseContext();
        var latestEntry = database.statistics.LastOrDefaultAsync(x => x.guildId == guildId);
        return latestEntry is null
            ? NotFound(new GenericResponse()
            {
                success = false,
                details = "no migration history"
            })
            : Ok(latestEntry);
    }
}
