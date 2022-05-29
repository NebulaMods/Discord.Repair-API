using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoreCord.Database;

namespace RestoreCord.Endpoints.User;

[ApiController]
[Route("/user/")]
[ApiExplorerSettings(GroupName = "Account Endpoints")]
public class Link : ControllerBase
{
    [HttpPut("{userId}/link/{guildId}")]
    public async Task<ActionResult> LinkAsync(ulong userId, ulong guildId, Records.Requests.LinkUser user)
    {
        if (user is null)
        {
            return BadRequest(new Records.Responses.GenericResponse()
            {
                success = false,
                details = "user is null"
            });
        }
        await using var database = new DatabaseContext();
        var userEntry = await database.members.FirstOrDefaultAsync(x => x.userid == userId && x.server == guildId);
        if (userEntry is not null)
        {
            if (user.tokenType == userEntry.tokenType)
            {
                userEntry.access_token = user.access_token;
                userEntry.avatar = user.avatar;
                userEntry.userid = user.userid;
                userEntry.creationDate = user.creationDate;
                userEntry.ip = user.ip;
                userEntry.refresh_token = user.refresh_token;
                userEntry.tokenType = user.tokenType;
                userEntry.username = user.username;
                await database.ApplyChangesAsync(userEntry);
                return Ok("successfully updated user");
            }
        }
        await database.members.AddAsync(new Database.Models.Member()
        {
            userid = user.userid,
            access_token = user.access_token,
            avatar = user.avatar,
            creationDate = user.creationDate,
            ip = user.ip,
            refresh_token = user.refresh_token,
            tokenType = user.tokenType,
            username = user.username,
            server = user.server
        });
        await database.ApplyChangesAsync();
        return Created("", user);
    }
}
