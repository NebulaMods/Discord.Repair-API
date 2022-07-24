using Discord.WebSocket;

using Microsoft.AspNetCore.Mvc;

using RestoreCord.Records.Responses;

namespace RestoreCord.Endpoints.V1.Guild;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/guild/")]
[ApiExplorerSettings(GroupName = "Guild Endpoints")]
public class Backup : ControllerBase
{
    private readonly DiscordShardedClient _client;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="client"></param>
    public Backup(DiscordShardedClient client)
    {
        _client = client;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <returns></returns>
    [HttpPost("{guildId}/backup")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult> HandleAsync(ulong guildId)
    {
        if (guildId is 0)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid guild id"
            });
        }


        _ = Task.Run(async () => await BackupGuildAsync(guildId));
        return Ok();
    }
    
    private async Task BackupGuildAsync(ulong guildId)
    {

    }
}
