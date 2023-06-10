using DiscordRepair.Api.Database;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Endpoints.V1.User;

/// <summary>
/// Represents an API controller for deleting users.
/// </summary>
[ApiController]
[Route("/v1/user/")]
[ApiExplorerSettings(GroupName = "Account Endpoints")]
public class Delete : ControllerBase
{
    /// <summary>
    /// Deletes a user using their username or email.
    /// </summary>
    /// <param name="user">The username or email of the user to delete.</param>
    /// <returns>An action result indicating the success or failure of the operation.</returns>
    /// <remarks>Delete a user using their username or email.</remarks>
    [HttpDelete("{user}")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    [ProducesResponseType(typeof(Generic), 404)]
    public async Task<ActionResult> HandlesAsync(string user)
    {
        // Retrieve the user entry for the requesting user
        var currentUser = await HttpContext.GetCurrentUserAsync();
        if (currentUser == null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "Requesting user doesn't exist, please try again."
            });
        }

        // Check if the requesting user is a staff member
        if (currentUser.accountType != Database.Models.AccountType.Staff)
        {
            return Unauthorized(new Generic()
            {
                details = "Requesting user doesn't have access to this resource.",
                success = false
            });
        }

        // Retrieve the user entry for the user to delete
        var userToDelete = await Utilities.Miscallenous.GetUserByUsernameOrEmailAsync(user);
        if (userToDelete == null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "User to delete doesn't exist, please try again."
            });
        }

        await using var database = new DatabaseContext();

        // Remove all global blacklists associated with the user
        database.RemoveRange(userToDelete.globalBlacklist);

        // Remove all bots associated with the user and their corresponding members
        foreach (var bot in userToDelete.bots)
        {
            var members = await database.members.Where(x => x.botUsed == bot).ToArrayAsync();
            database.members.RemoveRange(members);
        }
        database.RemoveRange(userToDelete.bots);

        // Remove all backups associated with the user and their associated data
        foreach (var backup in userToDelete.backups)
        {
            if (backup.catgeoryChannels is not null)
                database.RemoveRange(backup.catgeoryChannels.SelectMany(x => x.permissions));
            if (backup.textChannels is not null)
                database.RemoveRange(backup.textChannels.SelectMany(x => x.permissions));
            if (backup.voiceChannels is not null)
                database.RemoveRange(backup.voiceChannels.SelectMany(x => x.permissions));
            if (backup.users is not null)
                database.RemoveRange(backup.users.SelectMany(x => x.assignedRoles));
            if (backup.stickers is not null)
                database.RemoveRange(backup.stickers);
            if (backup.users is not null)
                database.RemoveRange(backup.users);
            if (backup.catgeoryChannels is not null)
                database.RemoveRange(backup.catgeoryChannels);
            if (backup.emojis is not null)
                database.RemoveRange(backup.emojis);
            if (backup.roles is not null)
                database.RemoveRange(backup.roles);
            if (backup.voiceChannels is not null)
                database.RemoveRange(backup.voiceChannels);
            if (backup.textChannels is not null)
                database.RemoveRange(backup.textChannels);
            if (backup.messages is not null)
                database.RemoveRange(backup.messages);
            if (backup.guild is not null)
                database.Remove(backup.guild);
        }

        // Remove all migrations and servers associated with the user
        var migrationsToDelete = await database.migrations.Where(x => x.user == userToDelete).ToArrayAsync();
        var serversToDelete = await database.servers.Where(x => x.owner == userToDelete).ToArrayAsync();
        database.migrations.RemoveRange(migrationsToDelete);
        database.servers.RemoveRange(serversToDelete);

        // Remove the user from the database
        database.RemoveRange(userToDelete);

        // Save the changes to the database
        await database.ApplyChangesAsync();

        return Ok(new Generic()
        {
            details = $"Successfully deleted user '{userToDelete.username}'.",
            success = true
        });
    }

}
//public async Task<ActionResult> HandlesAsync(string user)
//{
//    var username = HttpContext.WhoAmI();
//    await using var database = new DatabaseContext();
//    var userEntry = await database.users.FirstOrDefaultAsync(x => x.username.Equals(username, StringComparison.OrdinalIgnoreCase));
//    if (userEntry is null)
//    {
//        return BadRequest(new Generic()
//        {
//            success = false,
//            details = "user doesn't exist, please try again."
//        });
//    }
//    if (userEntry.accountType is not Database.Models.AccountType.Staff)
//    {
//        return Unauthorized(new Generic()
//        {
//            details = "user doesn't have access to this resource.",
//            success = false
//        });
//    }
//    var userToRemove = await database.users.FirstOrDefaultAsync(x => x.username == user || x.email == user);
//    if (userToRemove is null)
//    {
//        return BadRequest(new Generic()
//        {
//            success = false,
//            details = "user doesn't esxist, please try again."
//        });
//    }
//    database.RemoveRange(userToRemove.globalBlacklist);
//    foreach(var bot in userToRemove.bots)
//    {
//        var members = await database.members.Where(x => x.botUsed == bot).ToArrayAsync();
//        database.members.RemoveRange(members);
//    }
//    database.RemoveRange(userToRemove.bots);
//    foreach (var backup in userToRemove.backups)
//    {
//        foreach (var catChannel in backup.catgeoryChannels)
//        {
//            database.RemoveRange(catChannel.permissions);
//        }
//        foreach (var txtChannel in backup.textChannels)
//        {
//            database.RemoveRange(txtChannel.permissions);
//            database.RemoveRange(txtChannel.messages);
//        }
//        foreach (var vipChannel in backup.voiceChannels)
//        {
//            database.RemoveRange(vipChannel.permissions);
//        }
//        foreach (var guildUser in backup.users)
//        {
//            database.RemoveRange(guildUser.assignedRoles);
//        }
//        database.RemoveRange(backup.stickers);
//        database.RemoveRange(backup.users);
//        database.RemoveRange(backup.catgeoryChannels);
//        database.RemoveRange(backup.emojis);
//        database.RemoveRange(backup.roles);
//        database.RemoveRange(backup.voiceChannels);
//        database.RemoveRange(backup.textChannels);
//    }
//    var migrations = await database.migrations.Where(x => x.user ==  userToRemove).ToArrayAsync();
//    var servers = await database.servers.Where(x => x.owner == userToRemove).ToArrayAsync();
//    database.migrations.RemoveRange(migrations);
//    database.servers.RemoveRange(servers);
//    database.RemoveRange(userToRemove.backups);
//    database.users.Remove(userToRemove);
//    await database.ApplyChangesAsync();
//    return Ok(new Generic()
//    {
//        details = $"successfully deleted {userToRemove.username}",
//        success = true
//    });
//}
