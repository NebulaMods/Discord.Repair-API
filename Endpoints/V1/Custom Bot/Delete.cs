using Microsoft.AspNetCore.Mvc;

using RestoreCord.Records.Responses;

namespace RestoreCord.Endpoints.V1.CustomBot;

[ApiController]
[Route("/v1/custom-bot/")]
[ApiExplorerSettings(GroupName = "Custom Bot Endpoints")]
public class Delete : ControllerBase
{
    [HttpDelete("delete/{bot}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult> HandlesAsync(string bot)
    {
        return Ok(new Generic());
    }
}
