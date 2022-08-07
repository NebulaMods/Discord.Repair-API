
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DiscordRepair.Database;
using DiscordRepair.Database.Models;
using DiscordRepair.Records.Responses;
using DiscordRepair.Utilities;

namespace DiscordRepair.Endpoints.V1.DiscordUser;

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
    /// <param name="guildId"></param>
    /// <remarks>Unlink a discord user from a guild/server.</remarks>
    /// <returns></returns>
    [HttpPost("{userId}/unlink/{guildId}")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Generic>> HandleAsync(ulong userId, ulong guildId)
    {
        if (userId == 0 || guildId == 0)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }
        await using var database = new DatabaseContext();
        var result = await this.VerifyServer(guildId, userId, database);
        if (result.Item1 is not null)
            return result.Item1;
        if (result.Item2 is null)
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        Database.Models.Server serverEntry = await database.servers.FirstAsync(x => x.guildId == guildId);

        Member? userEntry = await database.members.FirstOrDefaultAsync(x => x.discordId == userId && x.server.guildId == guildId);
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
            details = $"Successfully unlinked {userId} from {guildId}."
        });
    }
}
