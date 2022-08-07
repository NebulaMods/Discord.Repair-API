using Discord;
using Discord.Rest;

using Microsoft.AspNetCore.Mvc;

using DiscordRepair.Records.Responses;
using DiscordRepair.Utilities;

namespace DiscordRepair.Endpoints.V1.Guild.User;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/guild/")]
[ApiExplorerSettings(GroupName = "Guild User Endpoints")]
public class Kick : ControllerBase
{
    /// <summary>
    /// Kick a specific user or all unverified users.
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="userId"></param>
    /// <remarks>Kick a specific user or all unverified users.</remarks>
    /// <returns></returns>
    [HttpPost("{guildId}/kick")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Generic>> HandleAsync(ulong guildId, ulong? userId = null)
    {
        if (guildId is 0)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }
        await using var database = new Database.DatabaseContext();
        await using var client = new DiscordRestClient();
        var result = await this.VerifyServer(guildId, database, HttpContext.Request.Headers["Authorization"]);
        if (result.Item1 is not null)
            return result.Item1;
        if (result.Item2 is null)
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        await client.LoginAsync(TokenType.Bot, result.Item2.settings.mainBot.token);
        var guild = await client.GetGuildAsync(guildId);
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
            if (result.Item2.roleId is null)
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
                    if (role == result.Item2.roleId)
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
