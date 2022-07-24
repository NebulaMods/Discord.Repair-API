using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using RestoreCord.Database;
using RestoreCord.Database.Models;
using RestoreCord.Records.Responses;
using RestoreCord.Utilities;

namespace RestoreCord.Endpoints.V1.DiscordUser;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/discord-user/")]
[ApiExplorerSettings(GroupName = "Discord Account Endpoints")]
public class Link : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="guildId"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    [HttpPut("{userId}/link/{guildId}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<string>> HandleAsync(ulong userId, ulong guildId, Records.Requests.Guild.User.Link user)
    {
        await using var database = new DatabaseContext();
        var result = await this.VerifyServer(guildId, userId, database);
        if (result.Item1 is not null)
            return result.Item1;
        if (result.Item2 is null)
            return NoContent();

        Member? userEntry = await database.members.FirstOrDefaultAsync(x => x.discordId == userId && x.botUsed.clientId == user.botClientId);
        Database.Models.Server serverEntry = await database.servers.FirstAsync(x => x.guildId == guildId);
        if (serverEntry is null)
        {
            return BadRequest(new Records.Responses.Generic()
            {
                success = false,
                details = "server is not valid"
            });
        }
        Database.Models.User? owner = await database.users.FirstAsync(x => x.username == serverEntry.owner.username);
        if (userEntry is not null)
        {
            await database.BatchUpdate<Member>()
                .Set(x => x.accessToken, x => user.accessToken)
                .Set(x => x.refreshToken, x => user.refreshToken)
                .Set(x => x.avatar, x => user.avatar)
                .Set(x => x.ip, x => user.ip)
                .Set(x => x.username, x => user.username)
                .Where(x => x.discordId == userEntry.discordId && x.botUsed.clientId == user.botClientId)
                .ExecuteAsync();
            await database.ApplyChangesAsync();
            return Ok(new Generic()
            {
                details = "successfully updated user."
            });
        }
        var userBot = owner.bots.FirstOrDefault(x => x.clientId == user.botClientId);
        var newMember = new Member()
        {
            discordId = userId,
            accessToken = user.accessToken,
            avatar = user.avatar,
            creationDate = user.creationDate,
            ip = user.ip,
            refreshToken = user.refreshToken,
            botUsed = userBot,
            username = user.username,
            server = serverEntry,
        };
        await database.members.AddAsync(newMember);
        await database.ApplyChangesAsync();
        return Created("successfully created member.", newMember.accessToken);
    }
}
