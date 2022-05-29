using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoreCord.Database;
using RestoreCord.Database.Models;
using RestoreCord.Records.Responses;

namespace RestoreCord.Endpoints.Guild.User;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/guild/")]
[ApiExplorerSettings(GroupName = "Guild Endpoints")]
public class Blacklist : ControllerBase
{
    private readonly DiscordShardedClient _client;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="client"></param>
    public Blacklist(DiscordShardedClient client)
    {
        _client = client;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="userId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{guildId}/blacklist/{userId}")]
    public async Task<ActionResult> BlacklistAsync(ulong guildId, ulong userId, Records.Requests.BlacklistUser? request)
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
            if (blacklistUser is not null)
            {
                return BadRequest(new GenericResponse()
                {
                    success = false,
                    details = "user is already blacklisted"
                });
            }
            Member? userEntry = await database.members.FirstOrDefaultAsync(x => x.userid == userId && x.server == guildId);
            if (userEntry is null)
            {
                return NotFound(new GenericResponse()
                {
                    success = false,
                    details = "user does not exist in database"
                });
            }
            if (request is null)
                request = new();
            await database.blacklist.AddAsync(new Database.Models.Blacklist()
            {
                ip = userEntry.ip,
                server = userEntry.server,
                userid = userId,
                reason = request?.reason
            });
            await database.ApplyChangesAsync();
            try
            {
                if (request.banUser)
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
                        SocketGuildUser? guildUser = guildSocket.GetUser(userId);
                        if (guildUser is not null)
                        {
                            await guildSocket.AddBanAsync(guildUser, request.banPruneDays, request.reason);
                        }
                    }
                }
            }
            catch { }
            return Ok(new GenericResponse()
            {
                success = true,
                details = $"Successfully blacklisted {(request.banUser ? " & banned" : "")} {userId} from {guildId}"
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
