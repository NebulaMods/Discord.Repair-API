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
public class UnlinkAccount : ControllerBase
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="guildId"></param>
    /// <returns></returns>
    [HttpPost("{userId}/unlink/{guildId}")]
    public async Task<ActionResult> UnlinkAccountAsync(ulong userId, ulong guildId)
    {
        try
        {
            if (guildId is 0 || userId is 0)
            {
                return BadRequest(new GenericResponse()
                {
                    success = false,
                    details = "invalid user/guild id"
                });
            }
            await using var database = new DatabaseContext();
            Database.Models.Member? userEntry = await database.members.FirstOrDefaultAsync(x => x.userid == userId && x.server == guildId);
            if (userEntry is null)
            {
                return NotFound(new GenericResponse()
                {
                    success = false,
                    details = "user does not exist in database"
                });
            }
            database.Remove(userEntry);
            await database.ApplyChangesAsync();
            return Ok(new GenericResponse()
            {
                success = true,
                details = $"Successfully unlinked {userId} from {guildId}"
            });
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
