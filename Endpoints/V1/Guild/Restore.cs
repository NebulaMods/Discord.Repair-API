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
public class Restore : ControllerBase
{
    private readonly DiscordShardedClient _client;
    private readonly MigrationMaster.Restore _restore;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="client"></param>
    public Restore(DiscordShardedClient client, MigrationMaster.Restore restore)
    {
        _client = client;
        _restore = restore;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <returns></returns>
    [HttpPost("{guildId}/restore")]
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
        _ = Task.Run(async () => await RestoreGuildAsync(guildId));
        return Ok();
    }

    private async Task RestoreGuildAsync(ulong guildId)
    {
        DiscordShardedClient? client = _client;
        MigrationMaster.Restore? restore = _restore;
    }
}
