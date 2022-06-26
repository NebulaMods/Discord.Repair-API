using Discord.Rest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoreCord.Database;
using RestoreCord.Database.Models;
using RestoreCord.Records.Responses;

namespace RestoreCord.Endpoints.Guild.Blacklist;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/guild/")]
[ApiExplorerSettings(GroupName = "Guild Blacklist Endpoints")]
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
    public async Task<ActionResult> BlacklistAsync(ulong guildId, ulong userId, Records.Requests.BlacklistUser? request)
    {
        try
        {
            await using var database = new DatabaseContext();
            Server? server = await Utilities.Miscallenous.VerifyServer(this, guildId, userId, database);
            if (server is null)
                return NoContent();
            Database.Models.Blacklist? blacklistUser = server.settings.blacklist.FirstOrDefault(x => x.discordId == userId);
            if (blacklistUser is not null)
            {
                return BadRequest(new GenericResponse()
                {
                    success = false,
                    details = "user is already blacklisted."
                });
            }
            Member? userEntry = await database.members.FirstOrDefaultAsync(x => x.discordId == userId && x.guildId == guildId);
            if (request is null)
                request = new();
            server.settings.blacklist.Add(new Database.Models.Blacklist()
            {
                ip = userEntry.ip,
                discordId = userId,
                reason = request?.reason
            });
            await database.ApplyChangesAsync(server);
            if (request is not null)
            {
                if (request.banUser)
                {
                    await using DiscordRestClient client = new();
                    await client.LoginAsync(Discord.TokenType.Bot, server.settings.mainBot is null ? Properties.Resources.Token : server.settings.mainBot.token);
                    RestGuild? guildSocket = await client.GetGuildAsync(guildId);

                    if (guildSocket is not null)
                    {
                        RestGuildUser? guildUser = await guildSocket.GetUserAsync(userId);
                        if (guildUser is not null)
                            await guildSocket.AddBanAsync(guildUser, request.banPruneDays, request.reason);
                    }
                }
            }
            return Ok(new GenericResponse()
            {
                success = true,
                details = $"Successfully blacklisted {(request is not null ? request.banUser ? "& banned " : "" : "")}{userId} from {guildId}"
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
