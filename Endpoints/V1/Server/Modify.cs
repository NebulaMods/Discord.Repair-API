
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DiscordRepair.Database;
using DiscordRepair.Records.Requests.Server;
using DiscordRepair.Records.Responses;
using DiscordRepair.Utilities;

namespace DiscordRepair.Endpoints.V1.Server;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/server/")]
[ApiExplorerSettings(GroupName = "Server Endpoints")]
public class Modify : ControllerBase
{
    /// <summary>
    /// Modify a server using the GUID or name.
    /// </summary>
    /// <param name="server"></param>
    /// <param name="serverRequest"></param>
    /// <remarks>Modify a server using the GUID or name.</remarks>
    /// <returns></returns>
    [HttpPatch("{server}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Generic>> HandlesAsync(string server, ModifyServerRequest serverRequest)
    {
        if (string.IsNullOrWhiteSpace(server) || serverRequest is null)
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
        if (string.IsNullOrWhiteSpace(serverRequest.name) is false)
        serverEntry.name = serverRequest.name;
        if (serverRequest.guildId is not null)
            serverEntry.guildId = (ulong)serverRequest.guildId;
        serverEntry.roleId = serverRequest.roleId;
        if (string.IsNullOrWhiteSpace(serverRequest.verifyBackgroundImage) is false)
            serverEntry.settings.backgroundImage = serverRequest.verifyBackgroundImage;
        if (string.IsNullOrWhiteSpace(serverRequest.pic) is false)
            serverEntry.settings.pic = serverRequest.pic;
        if (string.IsNullOrWhiteSpace(serverRequest.redirectUrl) is false)
            serverEntry.settings.redirectUrl = serverRequest.redirectUrl;
        if (serverRequest.vpnCheck is not null)
        serverEntry.settings.vpnCheck = (bool)serverRequest.vpnCheck;
        if (string.IsNullOrWhiteSpace(serverRequest.webhook) is false)
            serverEntry.settings.webhook = serverRequest.webhook;
        if (serverRequest.webhookLogType is not null)
        serverEntry.settings.webhookLogType = (int)serverRequest.webhookLogType;
        await database.ApplyChangesAsync(serverEntry);
        return Ok(new Generic()
        {
            success = true,
            details = "successfully updated server."
        });
    }

}
