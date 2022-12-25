
using DiscordRepair.Api.Database;
using DiscordRepair.Api.Database.Models;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Utilities;

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
public class UnlinkAccount : ControllerBase
{

    /// <summary>
    /// Unlink a discord user from a guild/server.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="serverKey"></param>
    /// <remarks>Unlink a discord user from a guild/server.</remarks>
    /// <returns></returns>
    [HttpPost("{userId}/unlink/{serverKey}")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Generic>> HandleAsync(ulong userId, string serverKey)
    {
        if (userId == 0 || string.IsNullOrWhiteSpace(serverKey))
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }
        await using var database = new DatabaseContext();
        var server = await database.servers.FirstOrDefaultAsync(x => x.key.ToString() == serverKey);
        if (server is null)
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        Member? userEntry = await database.members.FirstOrDefaultAsync(x => x.discordId == userId && x.server == server);
        if (userEntry is null)
        {
            return NotFound(new Generic()
            {
                success = false,
                details = "user does not exist."
            });
        }
        database.Remove(userEntry);
        await database.ApplyChangesAsync();
        return Ok(new Generic()
        {
            success = true,
            details = $"Successfully unlinked {userId} from {serverKey}."
        });
    }
}
