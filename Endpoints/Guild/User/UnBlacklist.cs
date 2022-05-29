using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoreCord.Database;
using RestoreCord.Database.Models;
using RestoreCord.Records.Responses;

namespace RestoreCord.Endpoints.Guild.User;

[ApiController]
[Route("/guild/")]
[ApiExplorerSettings(GroupName = "Guild Endpoints")]
public class UnBlacklist : ControllerBase
{
    private readonly DiscordShardedClient _client;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="client"></param>
    public UnBlacklist(DiscordShardedClient client)
    {
        _client = client;
    }

    [HttpPost("{guildId}/unblacklist/{userId}")]
    public async Task<ActionResult> UnBlacklistAsync(ulong guildId, ulong userId)
    {
        try
        {
            if (userId is 0 || guildId is 0)
            {
                return BadRequest(new GenericResponse()
                {
                    success = false,
                    details = "invalid user/guild id"
                });
            }
            await using var database = new DatabaseContext();
            Server? server = await database.servers.FirstOrDefaultAsync(x => x.guildid == guildId);
            if (server is null)
            {
                return BadRequest(new GenericResponse()
                {
                    success = false,
                    details = "guild does not exist in database"
                });
            }
            if (string.IsNullOrWhiteSpace(server.banned) is false)
            {
                return BadRequest(new GenericResponse()
                {
                    success = false,
                    details = "guild is banned"
                });
            }
            Database.Models.Blacklist? blacklistUser = await database.blacklist.FirstOrDefaultAsync(x => x.userid == userId && x.server == guildId);
            if (blacklistUser is null)
            {
                return NotFound(new GenericResponse()
                {
                    success = false,
                    details = "user isn't blacklisted"
                });
            }
            database.blacklist.Remove(blacklistUser);
            await database.ApplyChangesAsync();
            try
            {
                SocketGuild? guildSocket = null;
                if (server.customBotEnabled && server.customBot is not null)
                {
                    using DiscordShardedClient client = new();
                    await client.LoginAsync(Discord.TokenType.Bot, server.customBot.token);
                    guildSocket = client.GetGuild(guildId);
                }
                else
                {
                    guildSocket = _client.GetGuild(guildId);
                }

                if (guildSocket is not null)
                {
                    await guildSocket.RemoveBanAsync(userId);
                }
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
                details = "internal server error"
            });
        }
    }
}
