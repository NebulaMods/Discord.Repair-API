
using DiscordRepair.Api.Database;
using DiscordRepair.Api.Records.Requests.Server;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Records.Responses.Server;
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
public class Create : ControllerBase
{
    /// <summary>
    /// Create a new server.
    /// </summary>
    /// <param name="serverRequest"></param>
    /// <remarks>Create a new server.</remarks>
    /// <returns></returns>
    [HttpPut]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CreateServerResponse), 201)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult> HandlesAsync(CreateServerRequest serverRequest)
    {
        if (serverRequest is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }
        await using var database = new DatabaseContext();
        var user = await database.users.FirstAsync(x => x.username == HttpContext.WhoAmI());
        if (user.accountType is not Database.Models.AccountType.Staff or Database.Models.AccountType.Premium)
            if (await database.servers.Where(x => x.owner == user).CountAsync() >= int.Parse(Properties.Resources.FreeServerLimit))
            {
                return BadRequest(new Generic()
                {
                    success = false,
                    details = "please upgrade in order to create another server"
                });
            }
        var server = await database.servers.FirstOrDefaultAsync(x => x.owner == user && x.name == serverRequest.name);
        if (server is not null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "server already exists with the used name, please try again."
            });
        }
        if ((await database.servers.FirstOrDefaultAsync(x => x.guildId == serverRequest.guildId && x.owner == user)) is not null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "server already exists with guild ID."
            });
        }
        var customBot = user.bots.FirstOrDefault(x => x.name == serverRequest.mainBot || x.key.ToString() == serverRequest.mainBot);
        if (customBot is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "bot specified does not exist, please try again."
            });
        }
        var newServer = new Database.Models.Server()
        {
            owner = user,
            guildId = serverRequest.guildId,
            name = serverRequest.name,
            roleId = serverRequest.roleId,
            settings = new()
            {
                backgroundImage = serverRequest.verifyBGImage,
                webhook = serverRequest.webhook,
                vpnCheck = serverRequest.vpnCheck,
                pic = serverRequest.pic,
                redirectUrl = serverRequest.redirectUrl,
                mainBot = customBot,
                captcha = serverRequest.captchaCheck
            }
        };
        newServer.settings.vanityUrl = $"https://discord.repair/server/{newServer.key}";
        await database.servers.AddAsync(newServer);
        await database.ApplyChangesAsync();
        return Created($"https://api.discord.repair/v1/server/{newServer.key}", new CreateServerResponse()
        {
            key = newServer.key,
            botName = customBot.name,
            guildId = newServer.guildId,
            name = newServer.name,
            roleId = newServer.roleId
        });
    }
}
