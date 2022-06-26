using Discord.Rest;
using Microsoft.AspNetCore.Mvc;
using RestoreCord.Database;
using RestoreCord.Records.Responses;

namespace RestoreCord.Endpoints.Guild.Blacklist;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/guild/")]
[ApiExplorerSettings(GroupName = "Guild Blacklist Endpoints")]
public class UnBlacklist : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpPost("{guildId}/unblacklist/{userId}")]
    public async Task<ActionResult> UnBlacklistAsync(ulong guildId, ulong userId)
    {
        try
        {
            await using var database = new DatabaseContext();
            Database.Models.Server? server = await Utilities.Miscallenous.VerifyServer(this, guildId, userId, database);
            if (server is null)
                return NoContent();
            Database.Models.Blacklist? blacklistUser = server.settings.blacklist.FirstOrDefault(x => x.discordId == userId);
            if (blacklistUser is null)
            {
                return NotFound(new GenericResponse()
                {
                    success = false,
                    details = "user isn't blacklisted."
                });
            }
            server.settings.blacklist.Remove(blacklistUser);
            await database.ApplyChangesAsync(server);
            try
            {
                await using DiscordRestClient client = new();
                await client.LoginAsync(Discord.TokenType.Bot, server.settings.mainBot is null ? Properties.Resources.Token : server.settings.mainBot.token);
                RestGuild? guildSocket = await client.GetGuildAsync(guildId);

                if (guildSocket is not null)
                    await guildSocket.RemoveBanAsync(userId);
            }
            catch { }
            return Ok(new GenericResponse()
            {
                success = true,
                details = $"Successfully unblacklisted {userId} from {guildId}"
            });
        }
        catch (Exception ex)
        {
            await ex.LogErrorAsync();
            return BadRequest(new GenericResponse()
            {
                success = false,
                details = "internal server error."
            });
        }
    }
}
