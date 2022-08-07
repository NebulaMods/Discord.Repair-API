using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DiscordRepair.Database;
using DiscordRepair.Records.Responses;

namespace DiscordRepair.Endpoints.V1.User;

[ApiController]
[Route("/v1/user/")]
[ApiExplorerSettings(GroupName = "Account Endpoints")]
public class Delete : ControllerBase
{
    //[HttpDelete("{user}")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult> HandlesAsync(string user)
    {
        await using var database = new DatabaseContext();
        var userEntry = await database.users.FirstOrDefaultAsync(x => x.username == user || x.email == user);
        if (userEntry is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "user doesn't exist, please try again."
            });
        }

        return Ok(new Generic());
    }
}
