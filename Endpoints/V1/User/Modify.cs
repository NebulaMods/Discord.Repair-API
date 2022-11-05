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
public class Modify : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="modifyRequest"></param>
    /// <returns></returns>
    [HttpPatch("{user}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult> HandlesAsync(string user, ModifyUserRequest modifyRequest)
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
        if (string.IsNullOrWhiteSpace(user) || user is "@me" or "me" || user == username)
        {
            user = username;
        }
        else
        {
            if (userEntry.accountType is not Database.Models.AccountType.Staff)
            {
                return Unauthorized(new Generic()
                {
                    success = false,
                    details = "user doesn't have access to this resource."
                });
            }
        }
        var userToUpdate = await database.users.FirstOrDefaultAsync(x => x.username == user || x.email == user);
        if (userToUpdate is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "user doesn't exist, please try again."
            });
        }
        bool changeToken = false;
        if (string.IsNullOrWhiteSpace(modifyRequest.pfp) is false)
            userEntry.pfp = modifyRequest.pfp;
        if (string.IsNullOrWhiteSpace(modifyRequest.email) is false)
        {
            if (await database.users.FirstOrDefaultAsync(x => x.email == modifyRequest.email) is null)
            {
                userEntry.email = modifyRequest.email;
                changeToken = true;
            }
        }
        if (string.IsNullOrWhiteSpace(modifyRequest.password) is false)
        {
            //check if string is base64?
            userEntry.password = await Miscallenous.HashPassword(modifyRequest.password);
            changeToken = true;
        }
        if (string.IsNullOrWhiteSpace(modifyRequest.username) is false)
        {
            if (await database.users.FirstOrDefaultAsync(x => x.username == modifyRequest.username) is null)
                userEntry.username = modifyRequest.username;
        }
        userEntry.discordId = modifyRequest.discordId;
        if (userEntry.accountType is AccountType.Staff)
        {
            userEntry.expiry = modifyRequest.expiry;
            if (modifyRequest.banned is not null)
                userEntry.banned = (bool)modifyRequest.banned;
            if (modifyRequest.accountType is not null)
                userEntry.accountType = (AccountType)modifyRequest.accountType;
            userEntry.lastIP = modifyRequest.lastIP;
        }
        userEntry.apiToken = Miscallenous.GenerateApiToken();
        await database.ApplyChangesAsync(userEntry);
        return Ok(new Generic()
        {
            success = true,
            details = $"successfully updated {user}"
        });
    }
}
