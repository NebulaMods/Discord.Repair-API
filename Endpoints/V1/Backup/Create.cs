
using Discord.Rest;

using DiscordRepair.Api.Database;
using DiscordRepair.Api.Database.Models.BackupModels;
using DiscordRepair.Api.MigrationMaster;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Endpoints.V1.Backup;


[ApiController]
[Route("/v1/backup/")]
[ApiExplorerSettings(GroupName = "Backup Endpoints")]
public class Create : ControllerBase
{
    [HttpPut("{guildId}")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BackupResponse), 201)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<BackupResponse>> HandleAsync(ulong guildId, BackupRequest backupRequest)
    {
        if (guildId is 0 || backupRequest is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid parameters"
            });
        }

        if (string.IsNullOrWhiteSpace(backupRequest.name))
        {
            return BadRequest(new Generic()
            {

            });
        }

        if (string.IsNullOrWhiteSpace(backupRequest.serverName) && string.IsNullOrWhiteSpace(backupRequest.botName))
        {
            return BadRequest(new Generic()
            {

            });
        }

        var user = await HttpContext.GetCurrentUserAsync();
        if (user is null)
        {
            return BadRequest(new Generic()
            {

            });
        }

        var bot = user.bots.FirstOrDefault(x => x.name == backupRequest.botName);
        if (bot is null)
        {
            return BadRequest(new Generic()
            {

            });
        }

        await using var discordClient = new DiscordRestClient();
        await discordClient.LoginAsync(Discord.TokenType.Bot, bot.token);
        var restGuild = await discordClient.GetGuildAsync(guildId);
        Database.Models.BackupModels.Backup backup = new();
        //check perms on server
        switch (backupRequest.type)
        {
            case BackupType.EMOJIS:
                await backup.BackupEmojisAsync(restGuild);
                backup.type = BackupType.EMOJIS;
                break;
            case BackupType.MESSAGES:
                break;
            case BackupType.CHANNELS:
                break;
            case BackupType.USER_ROLES:
                break;
            case BackupType.ROLES:
                break;
            case BackupType.FULL:
                break;
        }

        user.backups.Add(backup);
        await using var database = new DatabaseContext();
        await database.ApplyChangesAsync(user);
        return Created($"https://api.discord.repair/backup/{backup.name}", new BackupResponse()
        {
            creationTime = backup.creationDate,
            name = backup.name,
            type = backupRequest.type
        });
    }

    public record BackupRequest
    {
        public BackupType type { get; set; }

        public string? botName { get; set; }

        public string? serverName { get; set; }

        public string name { get; set; }
    }

    public record BackupResponse
    {
        public DateTime creationTime { get; set; }
        public string name { get; set; }
        public BackupType type { get; set; }
    }
}
