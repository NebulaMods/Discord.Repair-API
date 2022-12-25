using DiscordRepair.Api.Database;
using DiscordRepair.Api.Records.Responses;
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
public class Delete : ControllerBase
{
    /// <summary>
    /// Delete a user using their username or email.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <remarks>Delete a user using their username or email.</remarks>
    [HttpDelete("{user}")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    [ProducesResponseType(typeof(Generic), 404)]
    public async Task<ActionResult> HandlesAsync(string user)
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
        if (userEntry.accountType is not Database.Models.AccountType.Staff)
        {
            return Unauthorized(new Generic()
            {
                details = "user doesn't have access to this resource.",
                success = false
            });
        }
        var userToRemove = await database.users.FirstOrDefaultAsync(x => x.username == user || x.email == user);
        if (userToRemove is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "user doesn't esxist, please try again."
            });
        }
        database.RemoveRange(userToRemove.globalBlacklist);
        database.RemoveRange(userToRemove.bots);
        foreach (var backup in userToRemove.backups)
        {
            foreach (var catChannel in backup.catgeoryChannels)
            {
                database.RemoveRange(catChannel.permissions);
            }
            foreach (var txtChannel in backup.textChannels)
            {
                database.RemoveRange(txtChannel.permissions);
            }
            foreach (var vipChannel in backup.voiceChannels)
            {
                database.RemoveRange(vipChannel.permissions);
            }
            foreach (var guildUser in backup.users)
            {
                database.RemoveRange(guildUser.assignedRoles);
            }
            database.RemoveRange(backup.stickers);
            database.RemoveRange(backup.users);
            database.RemoveRange(backup.catgeoryChannels);
            database.RemoveRange(backup.emojis);
            database.RemoveRange(backup.roles);
            database.RemoveRange(backup.voiceChannels);
            database.RemoveRange(backup.textChannels);
        }
        database.RemoveRange(userToRemove.backups);
        database.users.Remove(userToRemove);
        await database.ApplyChangesAsync();
        return Ok(new Generic()
        {
            details = $"successfully deleted {userToRemove.username}",
            success = true
        });
    }
}
