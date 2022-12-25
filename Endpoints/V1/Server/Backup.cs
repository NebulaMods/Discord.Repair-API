
using DiscordRepair.Api.Records.Responses;

using Microsoft.AspNetCore.Mvc;

namespace DiscordRepair.Api.Endpoints.V1.Server;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/server/")]
[ApiExplorerSettings(GroupName = "Server Endpoints")]
public class Backup : ControllerBase
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="server"></param>
    /// <param name="backupName"></param>
    /// <returns></returns>
    [HttpPost("{server}/backup")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult> HandleAsync([FromRoute] string server, [FromQuery] string backupName)
    {
        if (string.IsNullOrWhiteSpace(server))
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid parameters"
            });
        }
        if (string.IsNullOrWhiteSpace(backupName))
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid backup name"
            });
        }
        //check amount of backups


        _ = Task.Run(async () => await BackupGuildAsync(server, backupName));
        return Ok();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serverName"></param>
    /// <param name="backupNmae"></param>
    /// <returns></returns>
    private async Task BackupGuildAsync(string serverName, string backupNmae)
    {

    }
}
