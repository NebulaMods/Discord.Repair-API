
using Microsoft.AspNetCore.Mvc;

using DiscordRepair.Records.Responses;

namespace DiscordRepair.Endpoints.V1.Guild;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/guild/")]
[ApiExplorerSettings(GroupName = "Guild Endpoints")]
public class Backup : ControllerBase
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="backupName"></param>
    /// <returns></returns>
    [HttpPost("{guildId}/backup")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult> HandleAsync([FromRoute] ulong guildId, [FromQuery] string backupName)
    {
        if (guildId is 0)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid guild id"
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


        _ = Task.Run(async () => await BackupGuildAsync(guildId, backupName));
        return Ok();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="backupNmae"></param>
    /// <returns></returns>
    private async Task BackupGuildAsync(ulong guildId, string backupNmae)
    {

    }
}
