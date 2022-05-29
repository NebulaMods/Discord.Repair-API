using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoreCord.Database;
using RestoreCord.Database.Models;
using RestoreCord.Records.Responses;

namespace RestoreCord.Endpoints.Guild.User;

[ApiController]
[Route("/guild/")]
[ApiExplorerSettings(GroupName = "Guild Endpoints")]
public class Get : ControllerBase
{
    [HttpGet("{guildId}/user/{userId}")]
    public async Task<ActionResult> GetAsync(ulong guildId, ulong userId)
    {

        try
        {
            if (userId is 0 || guildId is 0)
            {
                return BadRequest(new GenericResponse()
                {
                    success = false,
                    details = "invalid user/guild id"
                });
            }
            await using var database = new DatabaseContext();
            Server? server = await database.servers.FirstOrDefaultAsync(x => x.guildid == guildId);
            if (server is null)
            {
                return BadRequest(new GenericResponse()
                {
                    success = false,
                    details = "guild does not exist in database"
                });
            }
            if (string.IsNullOrWhiteSpace(server.banned) is false)
            {
                return BadRequest(new GenericResponse()
                {
                    success = false,
                    details = "guild is banned"
                });
            }
            Member? serverUser = await database.members.FirstOrDefaultAsync(x => x.userid == userId && x.server == guildId);
            return serverUser is null
                ? NotFound(new GenericResponse()
                {
                    success = false,
                    details = "user isn't blacklisted"
                })
                : Ok(serverUser);
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
