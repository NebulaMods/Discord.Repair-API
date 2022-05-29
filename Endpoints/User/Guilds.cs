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
                    details = "invalid user id"
                });
            }
            await using var database = new DatabaseContext();
            List<Database.Models.Member>? userEntries = await database.members.Where(x => x.userid == userId).ToListAsync();
            return userEntries is null
                ? NotFound(new GenericResponse()
                {
                    success = false,
                    details = "user does not exist in database"
                })
                : Ok(userEntries);
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
