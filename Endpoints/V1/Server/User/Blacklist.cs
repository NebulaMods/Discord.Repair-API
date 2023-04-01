using Discord.Rest;

using DiscordRepair.Api.Database;
using DiscordRepair.Api.Database.Models;
using DiscordRepair.Api.Records.Requests.Server.User;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Endpoints.V1.Server.User;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/server/")]
[ApiExplorerSettings(GroupName = "Server User Endpoints")]
public class Blacklist : ControllerBase
{

    /// <summary>
    /// Blacklist a user from a server using their user ID and server.
    /// </summary>
    /// <param name="server"></param>
    /// <param name="userId"></param>
    /// <param name="request"></param>
    /// <remarks>Blacklist a user from a server using their user ID and server.</remarks>
    /// <returns></returns>
    [HttpPut("{server}/blacklist/{userId}")]
    [Consumes("application/json", "plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Generic>> HandleAsync(string server, ulong userId, BlacklistUser? request)
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

        Database.Models.Blacklist? blacklistUser = serverEntry.settings.blacklist.FirstOrDefault(x => x.discordId == userId);
        if (blacklistUser is not null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "user is already blacklisted."
            });
        }
        Member? userEntry = await database.members.FirstOrDefaultAsync(x => x.discordId == userId && x.server == serverEntry);
        request ??= new();
        serverEntry.settings.blacklist.Add(new Database.Models.Blacklist()
        {
            ip = userEntry?.ip,
            discordId = userId,
            reason = request?.reason
        });
        await database.ApplyChangesAsync(serverEntry);
        if (request is not null)
        {
            if (request.banUser)
            {
                await using DiscordRestClient client = new();
                await client.LoginAsync(Discord.TokenType.Bot, serverEntry.settings.mainBot.token);
                RestGuild? guildSocket = await client.GetGuildAsync(serverEntry.guildId);

                if (guildSocket is not null)
                {
                    RestGuildUser? guildUser = await guildSocket.GetUserAsync(userId);
                    if (guildUser is not null)
                    {
                        await guildSocket.AddBanAsync(guildUser, request.banPruneDays, request.reason);
                    }
                }
            }
        }
        await database.DisposeAsync();
        return Ok(new Generic()
        {
            success = true,
            details = $"Successfully blacklisted {(request is not null ? request.banUser ? "& banned " : "" : "")}{userId} from {serverEntry.name}"
        });
    }
}
