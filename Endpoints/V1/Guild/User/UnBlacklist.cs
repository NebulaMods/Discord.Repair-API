using Discord.Rest;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using RestoreCord.Database;
using RestoreCord.Records.Responses;
using RestoreCord.Utilities;

namespace RestoreCord.Endpoints.V1.Guild.User;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/guild/")]
[ApiExplorerSettings(GroupName = "Guild User Endpoints")]
public class UnBlacklist : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpPost("{guildId}/unblacklist/{userId}")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Generic>> HandleAsync(ulong guildId, ulong userId)
    {
        await using var database = new DatabaseContext();
        var result = await this.VerifyServer(guildId, userId, database);
        if (result.Item1 is not null)
            return result.Item1;
        if (result.Item2 is null)
            return NoContent();
        Database.Models.Server serverEntry = await database.servers.FirstAsync(x => x.guildId == guildId);
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
        try
        {
            await using DiscordRestClient client = new();
            await client.LoginAsync(Discord.TokenType.Bot, serverEntry.settings.mainBot is null ? Properties.Resources.Token : serverEntry.settings.mainBot.token);
            RestGuild? guildSocket = await client.GetGuildAsync(guildId);

            if (guildSocket is not null)
                await guildSocket.RemoveBanAsync(userId);
        }
        catch { }
        return Ok(new Generic()
        {
            success = true,
            details = $"Successfully unblacklisted {userId} from {guildId}"
        });
    }
}
