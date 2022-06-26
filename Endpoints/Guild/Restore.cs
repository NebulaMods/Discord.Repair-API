using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using RestoreCord.Database;
using RestoreCord.Records.Responses;

namespace RestoreCord.Endpoints.Guild;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/guild/")]
[ApiExplorerSettings(GroupName = "Guild Endpoints")]
public class Restore : ControllerBase
{
    private readonly DiscordShardedClient _client;
    private readonly Migrations.Restore _restore;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="client"></param>
    public Restore(DiscordShardedClient client, Migrations.Restore restore)
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
    public async Task<ActionResult> RestoreAsync(ulong guildId)
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
            _ = Task.Run(async () => await RestoreGuildAsync(guildId));
            return Ok();
        }
        catch (Exception ex)
        {
            await ex.LogErrorAsync();
            return BadRequest(new GenericResponse()
            {
                success = false,
                details = "internal server error."
            });
        }
    }

    private async Task RestoreGuildAsync(ulong guildId)
    {
        DiscordShardedClient? client = _client;
        Migrations.Restore? restore = _restore;
    }
}
