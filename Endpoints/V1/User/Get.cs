using DiscordRepair.Api.Database;
using DiscordRepair.Api.Database.Models;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Records.Responses.User;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Endpoints.V1.User;

/// <summary>
/// API Controller for retrieving user information and managing user accounts.
/// </summary>
[ApiController]
[Route("/v1/user/")]
[ApiExplorerSettings(GroupName = "Account Endpoints")]
public class Get : ControllerBase
{
    /// <summary>
    /// Gets the user information for a given user.
    /// </summary>
    /// <param name="user">The username or identifier of the user to get information for.</param>
    /// <returns>An ActionResult containing the user's information or an error response.</returns>
    [HttpGet("{user}")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GetUserResponse), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    [ProducesResponseType(typeof(Generic), 404)]
    public async Task<ActionResult<GetUserResponse>> HandleAsync(string user)
    {
        // Get the current user making the request
        var currentUser = await HttpContext.GetCurrentUserAsync();
        // If the current user doesn't exist, return a bad request error response
        if (currentUser == null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "Current user doesn't exist, please try again."
            });
        }

        // If the user parameter is empty or equal to "@me" or "me", set it to the current user's username
        if (string.IsNullOrWhiteSpace(user) || user.Equals("@me", StringComparison.OrdinalIgnoreCase) || user.Equals("me", StringComparison.OrdinalIgnoreCase))
        {
            user = currentUser.username;
        }
        // If the current user is not a staff member and is attempting to get information for another user, return an unauthorized error response
        else if (currentUser.accountType != AccountType.Staff)
        {
            return Unauthorized(new Generic()
            {
                success = false,
                details = "User doesn't have access to this resource."
            });
        }

        // Get the user information for the specified user
        var userLookup = await Miscallenous.GetUserByUsernameOrEmailAsync(user);

        // If the user doesn't exist, return a not found error response
        if (userLookup == null)
        {
            return NotFound(new Generic()
            {
                success = false,
                details = "User doesn't exist, please try again."
            });
        }

        // Create a response object containing the user's information
        var response = new GetUserResponse
        {
            accountType = userLookup.accountType,
            createdAt = userLookup.createdAt,
            apiToken = userLookup.apiToken,
            banned = userLookup.banned,
            discordId = userLookup.discordId,
            email = userLookup.email,
            expiry = userLookup.expiry,
            lastIP = userLookup.lastIP,
            pfp = userLookup.pfp,
            username = userLookup.username,
        };

        // Return the user information in an Ok response
        return Ok(response);
    }

    /// <summary>
    /// Retrieves all users from the database and returns them as a JSON response.
    /// </summary>
    /// <returns>An ActionResult object that contains a GetAllUsersResponse array in the body of the response.</returns>
    [HttpGet]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GetAllUsersResponse[]), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    [ProducesResponseType(typeof(Generic), 404)]
    public async Task<ActionResult<GetAllUsersResponse[]>> HandleGetAllAsync()
    {
        // Retrieves the current user from the HttpContext by calling the WhoAmI() extension method.
        var currentUser = await Utilities.Miscallenous.GetUserByUsernameOrEmailAsync(HttpContext.WhoAmI());

        // Checks if the current user exists and has staff account type.
        if (currentUser == null || currentUser.accountType != AccountType.Staff)
        {
            // If the user is unauthorized, returns an UnauthorizedResult with an error message.
            return Unauthorized(new Generic()
            {
                success = false,
                details = "User doesn't have access to this resource."
            });
        }

        // Retrieves all users from the database.
        var allUsers = await GetAllUsersAsync();

        // Maps the retrieved users to a new GetAllUsersResponse object and selects only the required properties.
        var response = allUsers.Select(user => new GetAllUsersResponse
        {
            accountType = user.accountType,
            creationDate = user.createdAt,
            discordId = user.discordId,
            email = user.email,
            lastIp = user.lastIP,
            expiry = user.expiry,
            username = user.username,
        });

        // Returns an OK result with the mapped response as a JSON array in the body of the response.
        return Ok(response.ToArray());
    }

    /// <summary>
    /// Retrieves all users from the database asynchronously.
    /// </summary>
    /// <returns>A list of user objects.</returns>
    private async Task<List<Database.Models.User>> GetAllUsersAsync()
    {
        // Creates a new instance of the DatabaseContext class using the "await using" syntax.
        await using var database = new DatabaseContext();

        // Returns a list of all users in the database using the ToListAsync() method.
        return await database.users.ToListAsync();
    }
}
//public async Task<ActionResult> HandleGetAllAsync()
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
//    if (userEntry.accountType is not AccountType.Staff)
//    {
//        return Unauthorized(new Generic()
//        {
//            success = false,
//            details = "user doesn't have access to this resource."
//        });
//    }
//    var users = await database.users.ToListAsync();
//    List<GetAllUsersResponse> allUsers = new();
//    foreach (var user in users)
//    {
//        allUsers.Add(new GetAllUsersResponse
//        {
//            accountType = user.accountType,
//            creationDate = user.createdAt,
//            discordId = user.discordId,
//            email = user.email,
//            lastIp = user.lastIP,
//            expiry = user.expiry,
//            username = user.username,
//        });
//    }
//    return Ok(allUsers.ToArray());
//}
