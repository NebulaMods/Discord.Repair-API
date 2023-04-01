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
public class Modify : ControllerBase
{
    /// <summary>
    /// Endpoint for modifying a user's properties.
    /// </summary>
    /// <param name="user">The user to modify.</param>
    /// <param name="modifyRequest">The user properties to modify.</param>
    /// <returns>The modified user object or an error message.</returns>
    [HttpPatch("{user}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult> HandlesAsync(string user, ModifyUserRequest modifyRequest)
    {
        // Get the current user making the request
        var currentUser = await GetCurrentUserAsync();

        // Get the user to modify
        (Database.Models.User? user, ActionResult? httpResult) userToUpdate = await GetUserToUpdateAsync(user, currentUser);

        // If there was an error, return the error response
        if (userToUpdate.httpResult is not null)
        {
            return userToUpdate.httpResult;
        }

        // If the user doesn't exist, return a bad request response
        if (userToUpdate.user is null)
        {
            return BadRequest(new Generic { success = false, details = "User doesn't exist, please try again." });
        }

        try
        {
            // Update the user's properties
            await UpdateUserPropertiesAsync(userToUpdate.user, modifyRequest, currentUser);
        }
        catch (Exception ex)
        {
            // If there was an error updating the user's properties, return a bad request response
            return BadRequest(new Generic { success = false, details = ex.Message });
        }

        // Save changes to the database
        await SaveChangesAsync();

        // Return the updated user object
        return Ok(new Generic { success = true, details = $"Successfully updated {user}" });
    }

    /// <summary>
    /// Gets the current user making the request.
    /// </summary>
    /// <returns>The current user.</returns>
    private async Task<Database.Models.User> GetCurrentUserAsync()
    {
        var username = HttpContext.WhoAmI();
        return await new DatabaseContext().users.FirstOrDefaultAsync(x => x.username == username);
    }

    /// <summary>
    /// Gets the user to modify.
    /// </summary>
    /// <param name="user">The user to modify.</param>
    /// <param name="currentUser">The current user making the request.</param>
    /// <returns>The user to modify or an error message.</returns>
    private async Task<(Database.Models.User? user, ActionResult? httpResult)> GetUserToUpdateAsync(string user, Database.Models.User currentUser)
    {
        return string.IsNullOrWhiteSpace(user) || user.Equals("@me", StringComparison.OrdinalIgnoreCase)
            || user.Equals("me", StringComparison.OrdinalIgnoreCase)
            || user.Equals(currentUser.username, StringComparison.OrdinalIgnoreCase)
            || user.Equals(currentUser.email)
            ? ((Database.Models.User?, ActionResult?))(currentUser, null)
            : currentUser.accountType != AccountType.Staff
                ? (null, Unauthorized(new Generic()
                {
                    success = false,
                    details = "User doesn't have access to this resource."
                }))
                : (await new DatabaseContext().users.FirstOrDefaultAsync(x => x.username == user || x.email == user), null);
    }

    /// <summary>
    /// Updates the properties of a user in the database based on the provided ModifyUserRequest object.
    /// </summary>
    /// <param name="userToUpdate">The user object to update.</param>
    /// <param name="modifyRequest">The ModifyUserRequest object containing the updated properties.</param>
    /// <param name="currentUser">The current user object.</param>
    private async Task UpdateUserPropertiesAsync(Database.Models.User userToUpdate, ModifyUserRequest modifyRequest, Database.Models.User currentUser)
    {
        // Initialize a variable to track if the user's token needs to be updated.
        bool changeToken = false;

        // If the provided profile picture is not null or whitespace, update the user's profile picture.
        if (!string.IsNullOrWhiteSpace(modifyRequest.pfp))
        {
            userToUpdate.pfp = modifyRequest.pfp;
        }

        // If the provided email is not null or whitespace and it's different than the user's current email, update the user's email.
        if (!string.IsNullOrWhiteSpace(modifyRequest.email) && userToUpdate.email != modifyRequest.email)
        {
            // Check if the new email is already taken by another user in the database.
            if (await new DatabaseContext().users.AnyAsync(x => x.email == modifyRequest.email))
            {
                throw new Exception("Email is already taken.");
            }

            userToUpdate.email = modifyRequest.email;
            changeToken = true;
        }

        // If the provided password is not null or whitespace, hash it and update the user's password.
        if (!string.IsNullOrWhiteSpace(modifyRequest.password))
        {
            userToUpdate.password = await Miscallenous.HashPasswordAsync(modifyRequest.password);
            changeToken = true;
        }

        // If the provided username is not null or whitespace and it's different than the user's current username, update the user's username.
        if (!string.IsNullOrWhiteSpace(modifyRequest.username) && userToUpdate.username != modifyRequest.username)
        {
            // Check if the new username is already taken by another user in the database.
            if (await new DatabaseContext().users.AnyAsync(x => x.username == modifyRequest.username))
            {
                throw new Exception("Username is already taken.");
            }

            userToUpdate.username = modifyRequest.username;
        }

        // Update the user's Discord ID.
        userToUpdate.discordId = modifyRequest.discordId;

        // If the current user is a staff member, update additional properties.
        if (currentUser.accountType == AccountType.Staff)
        {
            userToUpdate.expiry = modifyRequest.expiry;

            // If the provided banned property is not null, update the user's banned property.
            if (modifyRequest.banned != null)
            {
                userToUpdate.banned = (bool)modifyRequest.banned;
            }

            // If the provided account type is not null, update the user's account type.
            if (modifyRequest.accountType != null)
            {
                userToUpdate.accountType = (AccountType)modifyRequest.accountType;
            }

            // Update the user's last IP address.
            userToUpdate.lastIP = modifyRequest.lastIP;
        }

        // If any property was updated that requires a new token to be generated, generate a new token and update the user's API token.
        if (changeToken)
        {
            userToUpdate.apiToken = Miscallenous.GenerateApiToken();
        }
    }

    /// <summary>
    /// Saves changes made to the database.
    /// </summary>
    private async Task SaveChangesAsync()
    {
        // Create a new instance of the database context and apply changes.
        await using var database = new DatabaseContext();
        await database.ApplyChangesAsync();
    }


    //public async Task<ActionResult> HandlesAsync(string user, ModifyUserRequest modifyRequest)
    //{
    //    var username = HttpContext.WhoAmI();
    //    await using var database = new DatabaseContext();
    //    var userEntry = await database.users.FirstOrDefaultAsync(x => x.username == username);
    //    if (userEntry is null)
    //    {
    //        return BadRequest(new Generic()
    //        {
    //            success = false,
    //            details = "user doesn't exist, please try again."
    //        });
    //    }
    //    if (string.IsNullOrWhiteSpace(user) || user is "@me" or "me" || user == username)
    //    {
    //        user = username;
    //    }
    //    else
    //    {
    //        if (userEntry.accountType is not Database.Models.AccountType.Staff)
    //        {
    //            return Unauthorized(new Generic()
    //            {
    //                success = false,
    //                details = "user doesn't have access to this resource."
    //            });
    //        }
    //    }
    //    var userToUpdate = await database.users.FirstOrDefaultAsync(x => x.username == user || x.email == user);
    //    if (userToUpdate is null)
    //    {
    //        return BadRequest(new Generic()
    //        {
    //            success = false,
    //            details = "user doesn't exist, please try again."
    //        });
    //    }
    //    bool changeToken = false;

    //    if (string.IsNullOrWhiteSpace(modifyRequest.pfp) is false)
    //        userEntry.pfp = modifyRequest.pfp;
    //    if (string.IsNullOrWhiteSpace(modifyRequest.email) is false)
    //    {
    //        if (await database.users.FirstOrDefaultAsync(x => x.email == modifyRequest.email) is null)
    //        {
    //            userEntry.email = modifyRequest.email;
    //            changeToken = true;
    //        }
    //    }
    //    if (string.IsNullOrWhiteSpace(modifyRequest.password) is false)
    //    {
    //        userEntry.password = await Miscallenous.HashPassword(modifyRequest.password);
    //        changeToken = true;
    //    }
    //    if (string.IsNullOrWhiteSpace(modifyRequest.username) is false)
    //    {
    //        if (await database.users.FirstOrDefaultAsync(x => x.username == modifyRequest.username) is null)
    //            userEntry.username = modifyRequest.username;
    //    }
    //    userEntry.discordId = modifyRequest.discordId;
    //    if (userEntry.accountType is AccountType.Staff)
    //    {
    //        userEntry.expiry = modifyRequest.expiry;
    //        if (modifyRequest.banned is not null)
    //            userEntry.banned = (bool)modifyRequest.banned;
    //        if (modifyRequest.accountType is not null)
    //            userEntry.accountType = (AccountType)modifyRequest.accountType;
    //        userEntry.lastIP = modifyRequest.lastIP;
    //    }
    //    if (changeToken)
    //    {
    //        userEntry.apiToken = Miscallenous.GenerateApiToken();
    //        await database.ApplyChangesAsync(userEntry);
    //        return Ok(new Generic()
    //        {
    //            success = true,
    //            details = userEntry.apiToken
    //        });
    //    }
    //    await database.ApplyChangesAsync(userEntry);
    //    return Ok(new Generic()
    //    {
    //        success = true,
    //        details = $"successfully updated {user}"
    //    });
    //}
}
