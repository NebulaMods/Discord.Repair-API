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
public class Get : ControllerBase
{
    /// <summary>
    /// Get information about a specify server using the GUID or name.
    /// </summary>
    /// <param name="server"></param>
    /// <remarks>Get information about a specify server using the GUID or name.</remarks>
    /// <returns></returns>
    [HttpGet("{server}")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Records.Responses.Server.GetServerResponse), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Records.Responses.Server.GetServerResponse>> HandleAsync(string server)
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
        return serverEntry is null
            ? BadRequest(new Generic()
            {
                success = false,
                details = "server doesn't exist with the used name, please try again."
            })
            : Ok(new Records.Responses.Server.GetServerResponse()
            {
                backgroundImage = serverEntry.settings.backgroundImage,
                guildId = serverEntry.guildId,
                key = serverEntry.key,
                mainBotKey = serverEntry.settings.mainBot.key,
                mainBotName = serverEntry.settings.mainBot.name,
                name = serverEntry.name,
                pic = serverEntry.settings.pic,
                redirectUrl = serverEntry.settings.redirectUrl,
                roleId = serverEntry.roleId,
                vanityUrl = serverEntry.settings.vanityUrl,
                vpnCheck = serverEntry.settings.vpnCheck,
                webhook = serverEntry.settings.webhook,
                webhookLogType = serverEntry.settings.webhookLogType,
            });
    }

    /// <summary>
    /// Get a list of all servers.
    /// </summary>
    /// <remarks>Get a list of all servers.</remarks>
    /// <returns></returns>
    [HttpGet]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Records.Responses.Server.GetServerResponse[]), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Records.Responses.Server.GetServerResponse[]>> HandleGetAllAsync()
    {
        await using var database = new DatabaseContext();
        var user = await database.users.FirstAsync(x => x.username == HttpContext.WhoAmI());
        var servers = await database.servers.Where(x => x.owner == user).ToListAsync();
        if (servers.Any() is false)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "no servers exist, please try again."
            });
        }
        List<Records.Responses.Server.GetServerResponse> serverList = new();
        foreach(var server in servers)
        {
            serverList.Add(new Records.Responses.Server.GetServerResponse()
            {
                backgroundImage = server.settings.backgroundImage,
                guildId = server.guildId,
                key = server.key,
                mainBotKey = server.settings.mainBot.key,
                mainBotName = server.settings.mainBot.name,
                name = server.name,
                pic = server.settings.pic,
                redirectUrl = server.settings.redirectUrl,
                roleId = server.roleId,
                vanityUrl = server.settings.vanityUrl,
                vpnCheck = server.settings.vpnCheck,
                webhook = server.settings.webhook,
                webhookLogType = server.settings.webhookLogType,
            });
        }
        return Ok(serverList.ToArray());
    }
}
