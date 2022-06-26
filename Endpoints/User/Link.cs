using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoreCord.Database;
using RestoreCord.Database.Models;

namespace RestoreCord.Endpoints.User;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/user/")]
[ApiExplorerSettings(GroupName = "Account Endpoints")]
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
    public async Task<ActionResult<Member>> LinkAsync(ulong userId, ulong guildId, Records.Requests.LinkUser user)
    {
        await using var database = new DatabaseContext();
        Server? server = await Utilities.Miscallenous.VerifyServer(this, guildId, userId, database);
        if (server is null)
            return NoContent();

        Member? userEntry = await database.members.FirstOrDefaultAsync(x => x.discordId == userId && x.botUsed.clientId == user.botClientId);
        Server? serverEntry = await database.servers.FirstOrDefaultAsync(x => x.guildId == guildId);
        if (serverEntry is null)
        {
            return BadRequest(new Records.Responses.GenericResponse()
            {
                success = false,
                details = "server is not valid"
            });
        }
        Database.Models.User? owner = await database.users.FirstAsync(x => x.username == serverEntry.owner.username);
        if (userEntry is not null)
        {
            await database.members.BatchUpdate(database)
                .Set(userEntry.accessToken, user.accessToken)
                .Set(userEntry.refreshToken, user.refreshToken)
                .Set(userEntry.avatar, user.avatar)
                .Set(userEntry.ip, user.ip)
                .Set(userEntry.username, user.username)
                .Where(x => x.discordId == userEntry.discordId && x.botUsed.clientId == user.botClientId)
                .ExecuteAsync();
            return Ok("successfully updated user");
        }
        var newMember = new Member()
        {
            discordId = userId,
            accessToken = user.accessToken,
            avatar = user.avatar,
            creationDate = user.creationDate,
            ip = user.ip,
            refreshToken = user.refreshToken,
            botUsed = owner.bots.FirstOrDefault(x => x.clientId == user.botClientId),
            username = user.username,
            guildId = guildId
        };
        await database.members.AddAsync(newMember);
        await database.ApplyChangesAsync();
        return Created("successfully created member.", newMember);
    }
}
