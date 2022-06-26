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
    public async Task<ActionResult> GetStatisticsAsync(ulong guildId)
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
        Database.Models.LogModels.Statistics? latestEntry = await database.statistics.OrderBy(x => x.key).LastOrDefaultAsync(x => x.guildId == guildId);
        if (latestEntry is null)
            return NotFound(new GenericResponse()
            {
                success = false,
                details = "no migration history"
            });
        return Ok(new StatisticsResponse()
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

    public record StatisticsResponse
    {
        public int serverId { get; set; }
        public ulong guildId { get; set; }
        public string MigratedBy { get; set; }
        public bool active { get; set; }
        public DateTime startDate { get; set; }
        public DateTime? endDate { get; set; }
        public virtual Database.Models.Statistics.MemberMigration? memberStats { get; set; }
        public virtual Database.Models.Statistics.GuildMigration? guildStats { get; set; }
    }
}
