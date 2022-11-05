using DiscordRepair.Records.Responses;
using DiscordRepair.Utilities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Endpoints.V1.API;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/api/")]
[ApiExplorerSettings(GroupName = "API Endpoints")]
public class Statistics : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("statistics")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 400)]
    [ProducesResponseType(typeof(Generic), 404)]
    public async Task<ActionResult> HandleAsync()
    {
        var username = HttpContext.WhoAmI();
        await using var database = new Database.DatabaseContext();
        var userEntry = await database.users.FirstOrDefaultAsync(x => x.username == username);
        if (userEntry is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "user doesn't exist, please try again"
            });
        }
        if (userEntry.accountType is not Database.Models.AccountType.Staff)
        {
            return Unauthorized(new Generic()
            {
                success = false,
                details = "user doesn't have access to this resource."
            });
        }
        
        return Ok(new StatsResponse()
        {
            serverCount = await database.servers.CountAsync(),
            linkedMemberCount = await database.members.CountAsync(),
            userCount = await database.users.CountAsync(),
            backupCount = await database.users.SelectMany(x => x.backups).CountAsync(),
        });
    }
}
