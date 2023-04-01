using DiscordRepair.Api.Database;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Endpoints.V1.Server;

/// <summary>
/// 
/// </summary>
//[ApiController]
//[Route("/v1/server/")]
//[ApiExplorerSettings(GroupName = "Server Endpoints")]
public class Statistics : ControllerBase
{

    /// <summary>
    /// Returns the most recent or currently active server history.
    /// </summary>
    /// <param name="server"></param>
    /// <remarks>Returns the most recent or currently active server history.</remarks>
    /// <returns></returns>
    [HttpGet("{server}/stats")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Records.Responses.Server.Statistics), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Records.Responses.Server.Statistics>> HandleAsync(string server)
    {
        var verifyResult = this.VerifyServer(server, HttpContext.WhatIsMyToken());
        if (verifyResult is not null)
        {
            return verifyResult;
        }

        await using var database = new DatabaseContext();
        var (httpResult, serverEntry) = await this.VerifyServer(database, server, HttpContext.WhatIsMyToken());
        if (httpResult is not null)
        {
            return httpResult;
        }

        if (serverEntry is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }

        Database.Models.LogModels.Statistics? latestEntry = await database.statistics.OrderBy(x => x.key).LastOrDefaultAsync(x => x.server == serverEntry);
        return latestEntry is null
            ? NotFound(new Generic()
            {
                success = false,
                details = "no migration history."
            })
            : Ok(new Records.Responses.Server.Statistics()
            {
                active = latestEntry.active,
                guildId = latestEntry.guildId,
                guildStats = latestEntry.guildStats,
                memberStats = latestEntry.memberStats,
                MigratedBy = latestEntry.MigratedBy?.username,
                server = serverEntry.name,
                startDate = latestEntry.startDate,
                endDate = latestEntry.endDate
            });
    }
}
