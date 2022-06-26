using Microsoft.AspNetCore.Mvc;
using RestoreCord.Database;
using RestoreCord.Records.Responses;

namespace RestoreCord.Endpoints.Guild.Blacklist;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/guild/")]
[ApiExplorerSettings(GroupName = "Guild Blacklist Endpoints")]
public class GetAll : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <returns></returns>
    [HttpGet("{guildId}/blacklist")]
    public async Task<ActionResult> GetBlacklistAsync(ulong guildId)
    {
        try
        {
            await using var database = new DatabaseContext();
            Database.Models.Server? server = await Utilities.Miscallenous.VerifyServer(this, guildId, database);
            return server is null
                ? NoContent()
                : server.settings.blacklist.Any() is false
                ? NotFound(new Records.Responses.GenericResponse()
                {
                    success = false,
                    details = "no one is blacklisted."
                })
                : Ok(server.settings.blacklist.ToList());
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
}
