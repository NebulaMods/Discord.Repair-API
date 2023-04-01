using DiscordRepair.Api.Database;
using DiscordRepair.Api.Database.Models;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Records.Responses.User;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Endpoints.V1.User;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/user/")]
[ApiExplorerSettings(GroupName = "Account Endpoints")]
public class Get : ControllerBase
{
    /// <summary>
    /// Get a specific user using their username or email.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <remarks>Get a specific user using their username or email.</remarks>
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
        var userLookedUp = await database.users.FirstAsync(x => x.username == user || x.email == user);
        return Ok(new GetUserResponse()
        {
            accountType = userLookedUp.accountType,
            createdAt = userLookedUp.createdAt,
            apiToken = userLookedUp.apiToken,
            banned = userLookedUp.banned,
            discordId = userLookedUp.discordId,
            email = userLookedUp.email,
            expiry = userLookedUp.expiry,
            lastIP = userLookedUp.lastIP,
            pfp = userLookedUp.pfp,
            username = userLookedUp.username,
        });
    }

    /// <summary>
    /// Get a list of all users.
    /// </summary>
    /// <returns></returns>
    /// <remarks>Get a list of users.</remarks>
    [HttpGet]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<GetAllUsersResponse>), 200)]
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
        var users = await database.users.ToListAsync();
        List<GetAllUsersResponse> allUsers = new();
        foreach (var user in users)
        {
            allUsers.Add(new GetAllUsersResponse
            {
                accountType = user.accountType,
                creationDate = user.createdAt,
                discordId = user.discordId,
                email = user.email,
                lastIp = user.lastIP,
                expiry = user.expiry,
                username = user.username,
            });
        }
        return Ok(allUsers);
    }

}
