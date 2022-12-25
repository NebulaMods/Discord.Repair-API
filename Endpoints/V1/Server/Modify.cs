
using DiscordRepair.Api.Database;
using DiscordRepair.Api.Records.Requests.Server;
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
        try
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
            if (string.IsNullOrWhiteSpace(serverRequest.name) is false)
            {
                if (serverEntry.name != serverRequest.name)
                {
                    var idk = await database.servers.FirstOrDefaultAsync(x => x.name == serverRequest.name && x.owner.key == serverEntry.owner.key);
                    if (idk is not null)
                    {
                        if (idk != serverEntry)
                        return BadRequest(new Generic()
                        {
                            success = false,
                            details = "server already exists with that name"
                        });
                    }
                        serverEntry.name = serverRequest.name;
                }
            }
            if (serverRequest.guildId is not null)
            {
                if ((ulong)serverRequest.guildId != serverEntry.guildId)
                {
                    var idk = await database.servers.FirstOrDefaultAsync(x => x.guildId == (ulong)serverRequest.guildId && x.owner.key == serverEntry.owner.key);
                    if (idk is not null)
                    {
                        if (idk != serverEntry)
                        return BadRequest(new Generic()
                        {
                            success = false,
                            details = "server already exists with guild ID"
                        });
                    }
                    serverEntry.guildId = (ulong)serverRequest.guildId;
                }
            }
            serverEntry.roleId = serverRequest.roleId;
            if (string.IsNullOrWhiteSpace(serverRequest.verifyBGImage) is false)
            {
                serverEntry.settings.backgroundImage = serverRequest.verifyBGImage;
            }
            if (string.IsNullOrWhiteSpace(serverRequest.pic) is false)
            {
                serverEntry.settings.pic = serverRequest.pic;
            }
            if (string.IsNullOrWhiteSpace(serverRequest.redirectUrl) is false)
            {
                serverEntry.settings.redirectUrl = serverRequest.redirectUrl;
            }
            if (serverRequest.vpnCheck is not null)
            {
                serverEntry.settings.vpnCheck = (bool)serverRequest.vpnCheck;
            }
            if (string.IsNullOrWhiteSpace(serverRequest.webhook) is false)
            {
                serverEntry.settings.webhook = serverRequest.webhook;
            }
            if (serverRequest.webhookLogType is not null)
            {
                serverEntry.settings.webhookLogType = (int)serverRequest.webhookLogType;
            }
            if (serverRequest.captchaCheck is not null)
            {
                serverEntry.settings.captcha = (bool)serverRequest.captchaCheck;
            }
            if (string.IsNullOrWhiteSpace(serverRequest.mainBot) is false)
            {
                if (serverRequest.mainBot != serverEntry.settings.mainBot.name || serverRequest.mainBot != serverEntry.settings.mainBot.key.ToString())
                {
                    var bot = user.bots.FirstOrDefault(x => x.name == serverRequest.mainBot || x.key.ToString() == serverRequest.mainBot);
                    if (bot is not null)
                    {
                        serverEntry.settings.mainBot = bot;
                    }
                }
            }
            await database.ApplyChangesAsync(serverEntry);
            return Ok(new Generic()
            {
                success = true,
                details = "successfully updated server."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = ex.Message
            });
        }
    }

}
