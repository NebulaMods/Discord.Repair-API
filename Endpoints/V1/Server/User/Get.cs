using DiscordRepair.Api.Database;
using DiscordRepair.Api.Database.Models;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Records.Responses.Server.User;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Endpoints.V1.Server.User;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/server/")]
[ApiExplorerSettings(GroupName = "Server Endpoints")]
public class Get : ControllerBase
{
    /// <summary>
    /// Get information about a specific user from a server.
    /// </summary>
    /// <param name="server"></param>
    /// <param name="userId"></param>
    /// <remarks>Get information about a specific user from a server.</remarks>
    /// <returns></returns>
    [HttpGet("{server}/user/{userId}")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GetGuildUserResponse), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<GetGuildUserResponse>> HandleAsync(string server, ulong userId)
    {
        var verifyResult = this.VerifyServer(server, userId, HttpContext.WhatIsMyToken());
        if (verifyResult is not null)
        {
            return verifyResult;
        }

        await using var database = new DatabaseContext();
        var (httpResult, serverEntry) = await this.VerifyServer(database, server, HttpContext.WhatIsMyToken());
        if (httpResult is not null)
        {
            return httpResult;
        }

        if (serverEntry is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }

        Member? serverUser = await database.members.FirstOrDefaultAsync(x => x.discordId == userId && x.server == serverEntry);
        await database.DisposeAsync();
        return serverUser is null
            ? NotFound(new Generic()
            {
                success = false,
                details = "user does not exist."
            })
            : Ok(new Records.Responses.Server.User.GetGuildUserResponse()
            {
                accessToken = serverUser.accessToken,
                avatar = serverUser.avatar,
                botClientId = serverUser.botUsed?.clientId,
                linkDate = serverUser.linkDate,
                discordId = serverUser.discordId.ToString(),
                guildId = serverUser.server.guildId.ToString(),
                ip = serverUser.ip,
                refreshToken = serverUser.refreshToken,
                username = serverUser.username,
            });
    }

    /// <summary>
    /// Get all members associated to the specified server.
    /// </summary>
    /// <param name="server"></param>
    /// <remarks>Get all members associated to the specified server.</remarks>
    /// <returns></returns>
    [HttpGet("{server}/user")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<GetAllGuildUsersResponse>), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<List<GetAllGuildUsersResponse>>> HandleAsync(string server)
    {
        if (string.IsNullOrWhiteSpace(server))
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }
        await using var database = new DatabaseContext();
        List<GetAllGuildUsersResponse> allMembers = new();
        if (server == "all")
        {
            var user = await database.users.FirstAsync(x => x.apiToken == HttpContext.WhatIsMyToken());
            if (user.bots.Count is 0)
            {
                return BadRequest(new Generic()
                {
                    success = false,
                    details = "no members linked."
                });
            }
            foreach (var bot in user.bots)
            {
                var members = await database.members.Where(x => x.botUsed.key == bot.key).ToListAsync();
                foreach (var member in members)
                {
                    allMembers.Add(new Records.Responses.Server.User.GetAllGuildUsersResponse
                    {
                        botName = bot.name,
                        discordId = member.discordId.ToString(),
                        ip = member.ip,
                        serverName = member.server?.name,
                        username = member.username,
                        linkDate = member.linkDate,
                    });
                }
            }
            return allMembers.Count is 0
                ? (ActionResult<List<GetAllGuildUsersResponse>>)BadRequest(new Generic()
                {
                    success = false,
                    details = "no members linked."
                })
                : (ActionResult<List<GetAllGuildUsersResponse>>)Ok(allMembers);
        }
        if (server.Length > 64)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }
        var serverEntry = await database.servers.FirstOrDefaultAsync(x => (x.owner.apiToken == HttpContext.WhatIsMyToken() && x.key.ToString() == server) || x.name == server);
        if (serverEntry is null)
        {
            BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }
        if (await database.members.AnyAsync() is false)
        {
            NotFound(new Generic()
            {
                success = false,
                details = "no members linked."
            });
        }
        var allServerMembers = await database.members.Where(x => x.server.name == serverEntry.name).ToListAsync();
        if (allServerMembers.Count is 0)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "no members linked."
            });
        }
        foreach (var member in allServerMembers)
        {
            allMembers.Add(new Records.Responses.Server.User.GetAllGuildUsersResponse
            {
                botName = member.botUsed.name,
                discordId = member.discordId.ToString(),
                ip = member.ip,
                serverName = member.server?.name,
                username = member.username,
                linkDate = member.linkDate,
            });
        }
        return Ok(allMembers);
    }
}
