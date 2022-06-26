using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoreCord.Database;
using RestoreCord.Records.Responses;

namespace RestoreCord.Endpoints.User;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/user/")]
[ApiExplorerSettings(GroupName = "Account Endpoints")]
public class LinkedGuilds : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("{userId}/guilds")]
    public async Task<ActionResult> GetGuildsAsync(ulong userId)
    {
        try
        {
            if (userId is 0)
            {
                return BadRequest(new GenericResponse()
                {
                    success = false,
                    details = "invalid parameters, please try again."
                });
            }
            await using var database = new DatabaseContext();
            List<ulong>? userEntries = await database.members.Where(x => x.discordId == userId).Select(x => x.guildId).ToListAsync();
            return userEntries.Any() is false
                ? NotFound(new GenericResponse()
                {
                    success = false,
                    details = "not linked to any guilds"
                })
                : Ok(userEntries);
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
