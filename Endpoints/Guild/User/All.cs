using Microsoft.AspNetCore.Mvc;
using RestoreCord.Database.Models;
using RestoreCord.Database;
using RestoreCord.Records.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace RestoreCord.Endpoints.Guild.User;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/guild/")]
[ApiExplorerSettings(GroupName = "Guild User Endpoints")]
[AllowAnonymous]
public class GetAll : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <returns></returns>
    [HttpGet("{guildId}/users")]
    public async Task<ActionResult> GetAsync(ulong guildId)
    {

        try
        {
            await using var database = new DatabaseContext();
            Server? server = await Utilities.Miscallenous.VerifyServer(this, guildId, database);
            return server is null
                ? NoContent()
                : await database.members.AnyAsync() is false
                ? NotFound(new GenericResponse()
                {
                    success = false,
                    details = "no one is blacklisted."
                })
                : Ok(await database.members.Where(x => x.guildId == guildId).Select(x => x.discordId).ToListAsync());
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