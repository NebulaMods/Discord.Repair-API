using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DiscordRepair.Database;
using DiscordRepair.Database.Models;
using DiscordRepair.Records.Responses;
using DiscordRepair.Utilities;

namespace DiscordRepair.Endpoints.V1.Guild.User;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/guild/")]
[ApiExplorerSettings(GroupName = "Guild User Endpoints")]
public class Get : ControllerBase
{
    /// <summary>
    /// Get information about a specific user from a guild/server.
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="userId"></param>
    /// <remarks>Get information about a specific user from a guild/server.</remarks>
    /// <returns></returns>
    [HttpGet("{guildId}/user/{userId}")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Records.Responses.Guild.User.GetGuildUserResponse), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Records.Responses.Guild.User.GetGuildUserResponse>> HandleAsync(ulong guildId, ulong userId)
    {
        if (userId is 0 || guildId is 0)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }
        await using var database = new DatabaseContext();
        var result = await this.VerifyServer(guildId, userId, database, HttpContext.Request.Headers["Authorization"]);
        if (result.Item1 is not null)
            return result.Item1;
        if (result.Item2 is null)
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        Member? serverUser = await database.members.FirstOrDefaultAsync(x => x.discordId == userId && x.server.guildId == guildId);
        return serverUser is null
            ? NotFound(new Generic()
            {
                success = false,
                details = "user does not exist."
            })
            : Ok(new Records.Responses.Guild.User.GetGuildUserResponse()
            {
                accessToken = serverUser.accessToken,
                avatar = serverUser.avatar,
                botClientId = serverUser.botUsed?.clientId,
                creationDate = serverUser.creationDate,
                discordId = serverUser.discordId,
                guildId = guildId,
                ip = serverUser.ip,
                refreshToken = serverUser.refreshToken,
                username = serverUser.username,
            });
    }

    /// <summary>
    /// Get all members associated to the specified guild ID.
    /// </summary>
    /// <param name="guildId"></param>
    /// <remarks>Get all members associated to the specified guild ID.</remarks>
    /// <returns></returns>
    [HttpGet("{guildId}/user")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<ulong>), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<List<ulong>>> HandleAsync(ulong guildId)
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
            ? (ActionResult<List<ulong>>)result.Item1
            : result.Item2 is null
            ? (ActionResult<List<ulong>>)BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            })
            : await database.members.AnyAsync() is false
            ? NotFound(new Generic()
            {
                success = false,
                details = "no members linked."
            })
            : Ok(await database.members.Where(x => x.server.guildId == guildId).Select(x => x.discordId).ToListAsync());
    }
}
