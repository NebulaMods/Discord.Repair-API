using DiscordRepair.Api.Database;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Endpoints.V1.Server;

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
        var verifyResult = this.VerifyServer(server, HttpContext.WhatIsMyToken());
        if (verifyResult is not null)
            return verifyResult;
        await using var database = new DatabaseContext();
        var (httpResult, serverEntry) = await this.VerifyServer(database, server, HttpContext.WhatIsMyToken());
        if (httpResult is not null)
            return httpResult;
        if (serverEntry is null)
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        var user = await database.users.FirstAsync(x => x.username == HttpContext.WhoAmI());
        serverEntry.settings.blacklist.Clear();
        database.servers.Remove(serverEntry);
        await database.ApplyChangesAsync();
        await database.DisposeAsync();
        return Ok(new Generic()
        {
            success = true,
            details = "successfully deleted server."
        });
    }
}
