using Microsoft.AspNetCore.Mvc;
using RestoreCord.Records.Responses;

namespace RestoreCord.Endpoints.V1.Server;

[ApiController]
[Route("/v1/server/")]
[ApiExplorerSettings(GroupName = "Server Endpoints")]
public class Modify : ControllerBase
{
    [HttpPatch("modify/{server}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult> HandlesAsync(string server)
    {
        return Ok(new Generic());
    }
}
