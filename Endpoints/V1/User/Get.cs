using DiscordRepair.Database;
using DiscordRepair.Database.Models;
using DiscordRepair.Records.Responses;
using DiscordRepair.Records.Responses.User;
using DiscordRepair.Utilities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Endpoints.V1.User;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/user/")]
[ApiExplorerSettings(GroupName = "Account Endpoints")]
public class Get : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    [HttpGet("{user}")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GetUserResponse), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    [ProducesResponseType(typeof(Generic), 404)]
    public async Task<ActionResult> HandleAsync(string user)
    {
        var username = HttpContext.WhoAmI();
        await using var database = new DatabaseContext();
        var userEntry = await database.users.FirstOrDefaultAsync(x => x.username == username);
        if (userEntry is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "user doesn't exist, please try again."
            });
        }
        if (string.IsNullOrWhiteSpace(user) || user is "@me" or "me")
        {
            user = username;
        }
        else
        {
            if (userEntry.accountType is not AccountType.Staff)
            {
                return Unauthorized(new Generic()
                {
                    success = false,
                    details = "user doesn't have access to this resource."
                });
            }
        }
        var userLookedUp = await database.users.FirstOrDefaultAsync(x => x.username == user || x.email == user);
        return Ok(new GetUserResponse()
        {
            accountType = userEntry.accountType,
            createdAt = userEntry.createdAt,
            apiToken = userEntry.apiToken,
            banned = userEntry.banned,
            discordId = userEntry.discordId,
            email = userEntry.email,
            expiry = userEntry.expiry,
            lastIP = userEntry.lastIP,
            pfp = userEntry.pfp,
            username = username,
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<string>), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    [ProducesResponseType(typeof(Generic), 404)]
    public async Task<ActionResult> HandleGetAllAsync()
    {
        var username = HttpContext.WhoAmI();
        await using var database = new DatabaseContext();
        var userEntry = await database.users.FirstOrDefaultAsync(x => x.username == username);
        if (userEntry is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "user doesn't exist, please try again."
            });
        }
        if (userEntry.accountType is not AccountType.Staff)
        {
            return Unauthorized(new Generic()
            {
                success = false,
                details = "user doesn't have access to this resource."
            });
        }
        var users = await database.users.Select(x => x.username).ToListAsync();
        return Ok(users);
    }
}
