using DiscordRepair.Api.Database;
using DiscordRepair.Api.Records.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Endpoints.V1.DiscordUser;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/discord-user/")]
[ApiExplorerSettings(GroupName = "Discord Account Endpoints")]
[AllowAnonymous]
public class LinkedGuilds : ControllerBase
{
    /// <summary>
    /// Display the guilds associated to a user ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <remarks>Display the guilds associated to a user ID.</remarks>
    /// <returns></returns>
    [HttpGet("{userId}/guilds")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ulong[]), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<List<ulong>>> HandleAsync(ulong userId)
    {
        if (userId is 0)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid parameters, please try again."
            });
        }
        await using var database = new DatabaseContext();
        var userEntries = await database.members.Where(x => x.discordId == userId).Select(x => x.server.guildId).ToArrayAsync();
        return userEntries.Length is 0
            ? NotFound(new Generic()
            {
                success = false,
                details = "not linked to any guilds."
            })
            : Ok(userEntries);
    }
}
