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
[ApiExplorerSettings(GroupName = "Guild User Endpoints")]
public class Get : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("{guildId}/user/{userId}")]
    public async Task<ActionResult<GetUserResponse>> GetAsync(ulong guildId, ulong userId)
    {
        try
        {
            await using var database = new DatabaseContext();
            Server? server = await Utilities.Miscallenous.VerifyServer(this, guildId, userId, database);
            if (server is null)
                return NoContent();
            Member ? serverUser = await database.members.FirstOrDefaultAsync(x => x.discordId == userId && x.guildId == guildId);
            return serverUser is null
                ? NotFound(new GenericResponse()
                {
                    success = false,
                    details = "user does not exist."
                })
                : Ok(new GetUserResponse()
                {
                    accessToken = serverUser.accessToken,
                    avatar = serverUser.avatar,
                    botClientId = serverUser.botUsed?.clientId,
                    creationDate = serverUser.creationDate,
                    discordId = serverUser.discordId,
                    guildId = guildId,
                    ip = serverUser.ip,
                    refreshToken = serverUser.refreshToken,
                    username = serverUser.username,
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

    public record GetUserResponse
    {
        public ulong discordId { get; set; }
        public ulong guildId { get; set; }
        public string? accessToken { get; set; }
        public string? refreshToken { get; set; }
        public string? ip { get; set; }
        public string? avatar { get; set; }
        public string? username { get; set; }
        public ulong? creationDate { get; set; }
        public string? botClientId { get; set; }
    }
}
