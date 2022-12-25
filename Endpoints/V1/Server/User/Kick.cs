using Discord;
using Discord.Rest;

using DiscordRepair.Api.Database;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Mvc;

namespace DiscordRepair.Api.Endpoints.V1.Server.User;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/server/")]
[ApiExplorerSettings(GroupName = "Server User Endpoints")]
public class Kick : ControllerBase
{
    /// <summary>
    /// Kick a specific user or all unverified users.
    /// </summary>
    /// <param name="server"></param>
    /// <param name="userId"></param>
    /// <remarks>Kick a specific user or all unverified users.</remarks>
    /// <returns></returns>
    [HttpPost("{server}/kick")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Generic>> HandleAsync(string server, ulong? userId = null)
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
        await using var client = new DiscordRestClient();
        await client.LoginAsync(TokenType.Bot, serverEntry.settings.mainBot.token);
        var guild = await client.GetGuildAsync(serverEntry.guildId);
        if (guild is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "guild is unavailable, please try again."
            });
        }
        if (userId is null)
        {
            if (serverEntry.roleId is null)
            {
                return BadRequest(new Generic()
                {
                    success = false,
                    details = "role is null, please try again."
                });
            }
            var users = await guild.GetUsersAsync().FlattenAsync();
            int usersKicked = 0;
            foreach (var user in users)
            {
                if (user.IsBot)
                    continue;
                bool hasRole = false;
                foreach (var role in user.RoleIds)
                {
                    if (role == serverEntry.roleId)
                    {
                        hasRole = true;
                        break;
                    }
                }
                if (hasRole is false)
                {
                    try { await user.KickAsync(); usersKicked++; } catch { }
                }
            }
            return Ok(new Generic()
            {
                success = true,
                details = $"successsfully kicked: {usersKicked}"
            });
        }
        var guildUser = await guild.GetUserAsync((ulong)userId);
        if (guildUser is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "user is unavailable, please try again."
            });
        }
        try
        {
            await guildUser.KickAsync();
            return Ok(new Generic()
            {
                success = true,
                details = "successfully kicked user."
            });
        }
        catch
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "failed to kick user, please try again."
            });
        }
    }
}
