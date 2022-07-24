﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using RestoreCord.Database;
using RestoreCord.Records.Responses;

namespace RestoreCord.Endpoints.V1.DiscordUser;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/discord-user/")]
[ApiExplorerSettings(GroupName = "Discord Account Endpoints")]
public class LinkedGuilds : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("{userId}/guilds")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<ulong>), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<List<ulong>>> HandleAsync(ulong userId)
    {
        if (userId is 0)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid parameters, please try again."
            });
        }
        await using var database = new DatabaseContext();
        var userEntries = await database.members.Where(x => x.discordId == userId).Select(x => x.server.guildId).ToListAsync();
        return userEntries.Any() is false
            ? NotFound(new Generic()
            {
                success = false,
                details = "not linked to any guilds."
            })
            : Ok(userEntries);
    }
}