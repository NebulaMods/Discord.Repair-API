using DiscordRepair.Api.Database;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Mvc;

namespace DiscordRepair.Api.Endpoints.V1.Server;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/server/")]
[ApiExplorerSettings(GroupName = "Server Endpoints")]
public class GetBlacklist : ControllerBase
{
    /// <summary>
    /// Get a list of blacklisted users for the specified server.
    /// </summary>
    /// <param name="server"></param>
    /// <remarks>Get a list of blacklisted users for the specified server.</remarks>
    /// <returns></returns>
    [HttpGet("{server}/blacklist")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<Database.Models.Blacklist>), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<List<Database.Models.Blacklist>>> HandleAsync(string server)
    {
        var verifyResult = this.VerifyServer(server, HttpContext.WhatIsMyToken());
        if (verifyResult is not null)
        {
            return verifyResult;
        }

        await using var database = new DatabaseContext();
        var (httpResult, serverEntry) = await this.VerifyServer(database, server, HttpContext.WhatIsMyToken());
        return httpResult is not null
            ? (ActionResult<List<Database.Models.Blacklist>>)httpResult
            : serverEntry is null
            ? (ActionResult<List<Database.Models.Blacklist>>)BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            })
            : serverEntry.settings.blacklist.Any() is false
            ? NotFound(new Records.Responses.Generic()
            {
                success = false,
                details = "no one is blacklisted."
            })
            : Ok(serverEntry.settings.blacklist.ToList());
    }
}
