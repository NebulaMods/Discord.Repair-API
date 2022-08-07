
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DiscordRepair.Database;
using DiscordRepair.Records.Requests.User;
using DiscordRepair.Records.Responses;

namespace DiscordRepair.Endpoints.V1.User;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/user/")]
[ApiExplorerSettings(GroupName = "Account Endpoints")]
[AllowAnonymous]
public class Token : ControllerBase
{
    /// <summary>
    /// Get your api token using your login credentials.
    /// </summary>
    /// <param name="tokenRequest"></param>
    /// <remarks>Get your api token using your login credentials.</remarks>
    /// <returns></returns>
    [HttpPost("token")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Generic>> HandleAsync(TokenRequest tokenRequest)
    {
        if (tokenRequest is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }
        await using var database = new DatabaseContext();
        var user = await database.users.FirstOrDefaultAsync(x => x.username == tokenRequest.username || x.email == tokenRequest.email);
        return user is null
            ? (ActionResult<Generic>)BadRequest(new Generic()
            {
                success = false,
                details = "user doesn't exist, please try again."
            })
            : await Utilities.Miscallenous.VerifyHash(tokenRequest.password, user.password) is false
            ? (ActionResult<Generic>)BadRequest(new Generic()
            {
                success = false,
                details = "invalid password, please try again."
            })
            : (ActionResult<Generic>)Ok(new Generic()
            {
                success = true,
                details = user.apiToken
            });
    }

}
