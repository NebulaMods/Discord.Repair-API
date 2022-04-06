using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using RestoreCord.Database;
using RestoreCord.Records.Responses;

namespace RestoreCord.Endpoints;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/guild/")]
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
    public async Task<ActionResult> BackupGuildAsync(ulong guildId)
    {
        try
        {
            if (guildId is 0)
            {
                return BadRequest(new GenericResponse()
                {
                    success = false,
                    details = "invalid guild id"
                });
            }

            return Ok();
        }
        catch (Exception ex)
        {
            await ex.LogErrorAsync();
            return BadRequest(new GenericResponse()
            {
                success = false,
                details = "internal server error"
            });
        }
    }
}
