
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DiscordRepair.Database;
using DiscordRepair.Records.Requests.Server;
using DiscordRepair.Records.Responses;
using DiscordRepair.Records.Responses.Server;
using DiscordRepair.Utilities;

namespace DiscordRepair.Endpoints.V1.Server;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/server/")]
[ApiExplorerSettings(GroupName = "Server Endpoints")]
public class Create : ControllerBase
{
    /// <summary>
    /// Create a new server
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
        var server = await database.servers.FirstOrDefaultAsync(x => x.owner == user && x.name == serverRequest.name);
        if (server is not null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "server already exists with the used name, please try again."
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
                backgroundImage = serverRequest.verifyBackgroundImage,
                webhook = serverRequest.webhook,
                vpnCheck = serverRequest.vpnCheck,
                pic = serverRequest.pic,
                vanityUrl = serverRequest.vanityUrl,
                redirectUrl = serverRequest.redirectUrl,
                mainBot = customBot
            }
        };
        await database.servers.AddAsync(newServer);
        await database.ApplyChangesAsync();
        return Created($"https://discord.repair/v1/server/{newServer.key}", new CreateServerResponse()
        {
            key = newServer.key,
            botName = customBot.name,
            guildId = newServer.guildId,
            name = newServer.name,
            roleId = newServer.roleId
        });
    }
}
