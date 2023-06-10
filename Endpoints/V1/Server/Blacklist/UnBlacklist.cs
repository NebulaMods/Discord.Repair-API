using Discord.Rest;

using DiscordRepair.Api.Database;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Mvc;

namespace DiscordRepair.Api.Endpoints.V1.Server.Blacklist;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/server/")]
[ApiExplorerSettings(GroupName = "Server Endpoints")]
public class UnBlacklist : ControllerBase
{
    /// <summary>
    /// Unblacklist a user from a server.
    /// </summary>
    /// <param name="server"></param>
    /// <param name="userId"></param>
    /// <remarks>Unblacklist a user from a server.</remarks>
    /// <returns></returns>
    [HttpDelete("{server}/user/{userId}/blacklist")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Generic>> HandleAsync(string server, ulong userId)
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
        if (blacklistUser is null)
        {
            return NotFound(new Generic()
            {
                success = false,
                details = "user isn't blacklisted."
            });
        }
        serverEntry.settings.blacklist.Remove(blacklistUser);
        await database.ApplyChangesAsync(serverEntry);
        await database.DisposeAsync();
        try
        {
            await using DiscordRestClient client = new();
            await client.LoginAsync(Discord.TokenType.Bot, serverEntry.settings.mainBot.token);
            RestGuild? guildSocket = await client.GetGuildAsync(serverEntry.guildId);

            if (guildSocket is not null)
            {
                await guildSocket.RemoveBanAsync(userId);
            }
        }
        catch { }
        return Ok(new Generic()
        {
            success = true,
            details = $"Successfully unblacklisted {userId} from {serverEntry.name}."
        });
    }
}
