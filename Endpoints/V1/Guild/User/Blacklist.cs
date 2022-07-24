using Discord.Rest;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using RestoreCord.Database;
using RestoreCord.Database.Models;
using RestoreCord.Records.Responses;
using RestoreCord.Utilities;

namespace RestoreCord.Endpoints.V1.Guild.User;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/guild/")]
[ApiExplorerSettings(GroupName = "Guild User Endpoints")]
public class Blacklist : ControllerBase
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="userId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{guildId}/blacklist/{userId}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Generic>> HandleAsync(ulong guildId, ulong userId, Records.Requests.Guild.User.BlacklistUser? request)
    {
        await using var database = new DatabaseContext();
        var result = await this.VerifyServer(guildId, userId, database);
        if (result.Item1 is not null)
            return result.Item1;
        if (result.Item2 is null)
            return NoContent();
        Database.Models.Server serverEntry = await database.servers.FirstAsync(x => x.guildId == guildId);
        Database.Models.Blacklist? blacklistUser = serverEntry.settings.blacklist.FirstOrDefault(x => x.discordId == userId);
        if (blacklistUser is not null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "user is already blacklisted."
            });
        }
        Member? userEntry = await database.members.FirstOrDefaultAsync(x => x.discordId == userId && x.server.guildId == guildId);
        if (request is null)
            request = new();
        serverEntry.settings.blacklist.Add(new Database.Models.Blacklist()
        {
            ip = userEntry.ip,
            discordId = userId,
            reason = request?.reason
        });
        await database.ApplyChangesAsync(serverEntry);
        if (request is not null)
        {
            if (request.banUser)
            {
                await using DiscordRestClient client = new();
                await client.LoginAsync(Discord.TokenType.Bot, serverEntry.settings.mainBot is null ? Properties.Resources.Token : serverEntry.settings.mainBot.token);
                RestGuild? guildSocket = await client.GetGuildAsync(guildId);

                if (guildSocket is not null)
                {
                    RestGuildUser? guildUser = await guildSocket.GetUserAsync(userId);
                    if (guildUser is not null)
                        await guildSocket.AddBanAsync(guildUser, request.banPruneDays, request.reason);
                }
            }
        }
        return Ok(new Generic()
        {
            success = true,
            details = $"Successfully blacklisted {(request is not null ? request.banUser ? "& banned " : "" : "")}{userId} from {guildId}"
        });
    }
}
