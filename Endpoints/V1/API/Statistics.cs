using DiscordRepair.Api.Records.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Endpoints.V1.API;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/api/")]
[ApiExplorerSettings(GroupName = "API Endpoints")]
[AllowAnonymous]
public class Statistics : ControllerBase
{
    /// <summary>
    /// Get statistics about the service
    /// </summary>
    /// <returns></returns>
    [HttpGet("statistics")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(APIStatsResponse), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    public async Task<ActionResult> HandleAsync()
    {
        await using var database = new Database.DatabaseContext();
        return Ok(new APIStatsResponse()
        {
            serverCount = await database.servers.CountAsync(),
            linkedMemberCount = await database.members.CountAsync(),
            userCount = await database.users.CountAsync(),
            backupCount = await database.users.SelectMany(x => x.backups).CountAsync(),
        });
    }
}
