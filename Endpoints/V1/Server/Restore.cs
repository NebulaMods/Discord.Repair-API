
using DiscordRepair.Api.Records.Responses;

using Microsoft.AspNetCore.Mvc;

namespace DiscordRepair.Api.Endpoints.V1.Server;

/// <summary>
/// 
/// </summary>
//[ApiController]
//[Route("/v1/server/")]
//[ApiExplorerSettings(GroupName = "Server Endpoints")]
public class Restore : ControllerBase
{
    private readonly MigrationMaster.Restore _restore;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="restore"></param>
    public Restore(MigrationMaster.Restore restore)
    {
        _restore = restore;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="server"></param>
    /// <returns></returns>
    //[HttpPost("{server}/restore")]
    //[Consumes("plain/text")]
    //[Produces("application/json")]
    //[ProducesResponseType(typeof(Generic), 200)]
    //[ProducesResponseType(typeof(Generic), 404)]
    //[ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult> HandleAsync(string server)
    {
        if (string.IsNullOrWhiteSpace(server))
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid guild id"
            });
        }
        _ = Task.Run(async () => await RestoreGuildAsync(server));
        return Ok();
    }

    private async Task RestoreGuildAsync(string serverName)
    {
        MigrationMaster.Restore? restore = _restore;
    }
}
