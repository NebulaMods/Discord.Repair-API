using DiscordRepair.Api.Database;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Endpoints.V1.Server;

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
    [AllowAnonymous]
    [ProducesResponseType(typeof(Records.Responses.Server.GetServerResponse), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Records.Responses.Server.GetServerResponse>> HandleAsync(string server)
    {
        if (string.IsNullOrEmpty(server))
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters"
            });
        }
        if (server.Length > 64)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters"
            });
        }
        await using var database = new DatabaseContext();
        var user = await database.users.FirstOrDefaultAsync(x => x.username == HttpContext.WhoAmI());
        var serverEntry = user is null ? await database.servers.FirstOrDefaultAsync(x => x.key.ToString() == server) : await database.servers.FirstOrDefaultAsync(x => x.key.ToString() == server || (x.name == server && x.owner == user));
        return serverEntry is null
            ? (ActionResult<Records.Responses.Server.GetServerResponse>)BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            })
            : serverEntry.banned
            ? (ActionResult<Records.Responses.Server.GetServerResponse>)BadRequest(new Generic()
            {
                success = false,
                details = "server is banned."
            })
            : (ActionResult<Records.Responses.Server.GetServerResponse>)(user is null
            ? Ok(new Records.Responses.Server.GetVerifyPageResponse()
            {
                serverName = serverEntry.name,
                backgroundImage = serverEntry.settings.backgroundImage,
                guildId = serverEntry.guildId.ToString(),
                pic = serverEntry.settings.pic,
                captcha = serverEntry.settings.captcha,

            })
            : Ok(new Records.Responses.Server.GetServerResponse()
            {
                backgroundImage = serverEntry.settings.backgroundImage,
                guildId = serverEntry.guildId.ToString(),
                key = serverEntry.key,
                mainBotName = serverEntry.settings.mainBot.name,
                name = serverEntry.name,
                pic = serverEntry.settings.pic,
                redirectUrl = serverEntry.settings.redirectUrl,
                roleId = serverEntry.roleId.ToString(),
                vanityUrl = serverEntry.settings.vanityUrl,
                vpnCheck = serverEntry.settings.vpnCheck,
                webhook = serverEntry.settings.webhook,
                webhookLogType = serverEntry.settings.webhookLogType,
                captcha = serverEntry.settings.captcha,
            }));
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
        var servers = await database.servers.Where(x => x.owner.username == HttpContext.WhoAmI()).ToListAsync();
        if (servers.Any() is false)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "no servers exist, please try again."
            });
        }
        List<Records.Responses.Server.GetServerResponse> serverList = new();
        foreach (var server in servers)
        {
            serverList.Add(new Records.Responses.Server.GetServerResponse()
            {
                backgroundImage = server.settings.backgroundImage,
                guildId = server.guildId.ToString(),
                key = server.key,
                mainBotName = server.settings.mainBot.name,
                name = server.name,
                pic = server.settings.pic,
                redirectUrl = server.settings.redirectUrl,
                roleId = server.roleId.ToString(),
                vanityUrl = server.settings.vanityUrl,
                vpnCheck = server.settings.vpnCheck,
                webhook = server.settings.webhook,
                webhookLogType = server.settings.webhookLogType,
                captcha = server.settings.captcha,
            });
        }
        return Ok(serverList.ToArray());
    }
}
