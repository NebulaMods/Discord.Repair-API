using Microsoft.AspNetCore.Mvc;
using RestoreCord.Records.Responses;

namespace RestoreCord.Endpoints.V1.User;

[ApiController]
[Route("/v1/user/")]
[ApiExplorerSettings(GroupName = "Account Endpoints")]
public class Modify : ControllerBase
{
    [HttpPatch("modify/{username}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult> HandlesAsync(string username)
    {
        return Ok(new Generic());
    }
}
