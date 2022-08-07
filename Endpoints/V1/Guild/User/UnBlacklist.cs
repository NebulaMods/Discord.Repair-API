using Discord.Rest;

using Microsoft.AspNetCore.Mvc;

using DiscordRepair.Database;
using DiscordRepair.Records.Responses;
using DiscordRepair.Utilities;

namespace DiscordRepair.Endpoints.V1.Guild.User;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/guild/")]
[ApiExplorerSettings(GroupName = "Guild User Endpoints")]
public class UnBlacklist : ControllerBase
{
    /// <summary>
    /// Unblacklist a user from a guild/server.
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="userId"></param>
    /// <remarks>Unblacklist a user from a guild/server.</remarks>
    /// <returns></returns>
    [HttpDelete("{guildId}/blacklist/{userId}")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Generic>> HandleAsync(ulong guildId, ulong userId)
    {
        if (userId is 0 || guildId is 0)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }
        await using var database = new DatabaseContext();
        var result = await this.VerifyServer(guildId, userId, database, HttpContext.Request.Headers["Authorization"]);
        if (result.Item1 is not null)
            return result.Item1;
        if (result.Item2 is null)
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        Database.Models.Blacklist? blacklistUser = result.Item2.settings.blacklist.FirstOrDefault(x => x.discordId == userId);
        if (blacklistUser is null)
        {
            return NotFound(new Generic()
            {
                success = false,
                details = "user isn't blacklisted."
            });
        }
        result.Item2.settings.blacklist.Remove(blacklistUser);
        await database.ApplyChangesAsync(result.Item2);
        try
        {
            await using DiscordRestClient client = new();
            await client.LoginAsync(Discord.TokenType.Bot, result.Item2.settings.mainBot.token);
            RestGuild? guildSocket = await client.GetGuildAsync(guildId);

            if (guildSocket is not null)
                await guildSocket.RemoveBanAsync(userId);
        }
        catch { }
        return Ok(new Generic()
        {
            success = true,
            details = $"Successfully unblacklisted {userId} from {guildId}."
        });
    }
}
