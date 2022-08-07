using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DiscordRepair.Database;
using DiscordRepair.Records.Responses;
using DiscordRepair.Utilities;

namespace DiscordRepair.Endpoints.V1.Server;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/server/")]
[ApiExplorerSettings(GroupName = "Server Endpoints")]
public class Delete : ControllerBase
{
    /// <summary>
    /// Delete a server using the GUID or name.
    /// </summary>
    /// <param name="server"></param>
    /// <remarks>Delete a server using the GUID or name.</remarks>
    /// <returns></returns>
    [HttpDelete("{server}")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Generic>> HandlesAsync(string server)
    {
        if (string.IsNullOrWhiteSpace(server))
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "inalivd paramater, please try again."
            });
        }
        if (server.Length > 50)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "inalivd paramater, please try again."
            });
        }
        await using var database = new DatabaseContext();
        var user = await database.users.FirstAsync(x => x.username == HttpContext.WhoAmI());
        var serverEntry = await database.servers.FirstOrDefaultAsync(x => x.owner == user && (x.name == server || x.key.ToString() == server));
        if (serverEntry is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "server doesn't exist with the used name, please try again."
            });
        }
        serverEntry.settings.blacklist.Clear();
        database.servers.Remove(serverEntry);
        await database.ApplyChangesAsync();
        return Ok(new Generic()
        {
            success = true,
            details = "successfully deleted server."
        });
    }
}
