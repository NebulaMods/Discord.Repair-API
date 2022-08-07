using Microsoft.AspNetCore.Mvc;

using DiscordRepair.Database;
using DiscordRepair.Records.Responses;
using DiscordRepair.Utilities;

namespace DiscordRepair.Endpoints.V1.Guild;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/guild/")]
[ApiExplorerSettings(GroupName = "Guild Endpoints")]
public class GetBlacklist : ControllerBase
{
    /// <summary>
    /// Get a list of blacklisted users for the specified server using the guild ID.
    /// </summary>
    /// <param name="guildId"></param>
    /// <remarks>Get a list of blacklisted users for the specified server using the guild ID.</remarks>
    /// <returns></returns>
    [HttpGet("{guildId}/blacklist")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<Database.Models.Blacklist>), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<List<Database.Models.Blacklist>>> HandleAsync(ulong guildId)
    {
        if (guildId is 0)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }
        await using var database = new DatabaseContext();
        var result = await this.VerifyServer(guildId, database, HttpContext.Request.Headers["Authorization"]);
        return result.Item1 is not null
            ? (ActionResult<List<Database.Models.Blacklist>>)result.Item1
            : result.Item2 is null
            ? (ActionResult<List<Database.Models.Blacklist>>)NoContent()
            : result.Item2 is null
            ? NoContent()
            : result.Item2.settings.blacklist.Any() is false
            ? NotFound(new Records.Responses.Generic()
            {
                success = false,
                details = "no one is blacklisted."
            })
            : Ok(result.Item2.settings.blacklist.ToList());
    }
}
