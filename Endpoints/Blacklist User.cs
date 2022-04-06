using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoreCord.Database;
using RestoreCord.Records.Responses;

namespace RestoreCord.Endpoints;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/guild/")]
[ApiExplorerSettings(GroupName = "Guild Endpoints")]
public class BlacklistUser : ControllerBase
{
    private readonly DiscordShardedClient _client;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="client"></param>
    public BlacklistUser(DiscordShardedClient client)
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
            Database.Models.Member? userEntry = await database.members.FirstOrDefaultAsync(x => x.userid == userId && x.server == guildId);
            if (userEntry is null)
            {
                return NotFound(new GenericResponse()
                {
                    success = false,
                    details = "user does not exist in database"
                });
            }
            await database.blacklist.AddAsync(new Database.Models.Blacklist()
            {
                ip = userEntry.ip,
                server = userEntry.server,
                userid = userId,
            });
            await database.ApplyChangesAsync();
            if (request is null)
                request = new();
            try
            {
                if (request.banUser)
                {
                    SocketGuild? guildSocket = _client.GetGuild(guildId);
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
