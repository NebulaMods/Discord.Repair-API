using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoreCord.Database;
using RestoreCord.Database.Models;
using RestoreCord.Records.Responses;

namespace RestoreCord.Endpoints.User;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/user/")]
[ApiExplorerSettings(GroupName = "Account Endpoints")]
[AllowAnonymous]
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
            await using var database = new DatabaseContext();
            Server? server = await Utilities.Miscallenous.VerifyServer(this, guildId, userId, database);
            if (server is null)
                return NoContent();

            Member? userEntry = await database.members.FirstOrDefaultAsync(x => x.discordId == userId && x.guildId == guildId);
            if (userEntry is null)
            {
                return NotFound(new GenericResponse()
                {
                    success = false,
                    details = "user does not exist."
                });
            }
            database.Remove(userEntry);
            await database.ApplyChangesAsync();
            return Ok(new GenericResponse()
            {
                success = true,
                details = $"Successfully unlinked {userId} from {guildId}."
            });
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
