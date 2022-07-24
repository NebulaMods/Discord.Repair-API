using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using RestoreCord.Database;
using RestoreCord.Database.Models;
using RestoreCord.Records.Responses;
using RestoreCord.Utilities;

namespace RestoreCord.Endpoints.V1.Guild.User;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/guild/")]
[ApiExplorerSettings(GroupName = "Guild User Endpoints")]
public class Get : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("{guildId}/user/{userId}")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Records.Responses.Guild.User.Get), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Records.Responses.Guild.User.Get>> HandleAsync(ulong guildId, ulong userId)
    {
        await using var database = new DatabaseContext();
        var result = await this.VerifyServer(guildId, userId, database);
        if (result.Item1 is not null)
            return result.Item1;
        if (result.Item2 is null)
            return NoContent();
        Database.Models.Server serverEntry = await database.servers.FirstAsync(x => x.guildId == guildId);
        Member? serverUser = await database.members.FirstOrDefaultAsync(x => x.discordId == userId && x.server.guildId == guildId);
        return serverUser is null
            ? NotFound(new Generic()
            {
                success = false,
                details = "user does not exist."
            })
            : Ok(new Records.Responses.Guild.User.Get()
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
}
