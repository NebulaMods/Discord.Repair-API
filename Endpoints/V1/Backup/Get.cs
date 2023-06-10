using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Mvc;

using static DiscordRepair.Api.Endpoints.V1.Backup.Create;

namespace DiscordRepair.Api.Endpoints.V1.Backup;

[ApiController]
[Route("/v1/backup/")]
[ApiExplorerSettings(GroupName = "Backup Endpoints")]
public class Get : ControllerBase
{
    [HttpGet("{backup}")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BackupResponse), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<BackupResponse>> HandleAsync(string backup)
    {
        var currentUser = await HttpContext.GetCurrentUserAsync();
        if (currentUser is null)
        {
            return NotFound(new Generic()
            {
                success = false,
                details = "user doesn't exist"
            });
        }
        if (currentUser.backups.Any() is false)
        {
            return NotFound(new Generic()
            {
                success = false,
                details = "no backups exist"
            });
        }
        var backupEntry = currentUser.backups.FirstOrDefault(x => x.name == backup || x.key.ToString() == backup);
        if (backupEntry is null)
        {
            return NotFound(new Generic()
            {
                success = false,
                details = "no backup with that name/key exists"
            });
        }
        return Ok(new BackupResponse()
        {
            creationTime = backupEntry.creationDate,
            name = backupEntry.name,
            type = backupEntry.type,
            channelCount = backupEntry.catgeoryChannels?.Count + backupEntry.textChannels?.Count + backupEntry.voiceChannels?.Count ?? 0,
            emojiCount = backupEntry.emojis?.Count ?? 0,
            messageCount = backupEntry.messages?.Count ?? 0,
            roleCount = backupEntry.roles?.Count ?? 0,
            stickerCount = backupEntry.stickers?.Count ?? 0,
            userRoleCount = backupEntry.users?.Count ?? 0,
            guildId = backupEntry.guildId,
        });
    }

    [HttpGet()]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BackupResponse[]), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<BackupResponse[]>> HandleGetAllAsync()
    {
        var currentUser = await HttpContext.GetCurrentUserAsync();
        if (currentUser is null)
        {
            return NotFound(new Generic()
            {
                success = false,
                details = "user doesn't exist"
            });
        }
        if (currentUser.backups.Any() is false)
        {
            return NotFound(new Generic()
            {
                success= false,
                details = "no backups exist"
            });
        }
        List<BackupResponse> backups = new();
        foreach (var backup in currentUser.backups)
        {
            backups.Add(new BackupResponse()
            {
                creationTime = backup.creationDate,
                name = backup.name,
                type = backup.type,
                channelCount = backup.catgeoryChannels?.Count + backup.textChannels?.Count + backup.voiceChannels?.Count ?? 0,
                emojiCount = backup.emojis?.Count ?? 0,
                messageCount = backup.messages?.Count ?? 0,
                roleCount = backup.roles?.Count ?? 0,
                stickerCount = backup.stickers?.Count ?? 0,
                userRoleCount = backup.users?.Count ?? 0,
                guildId = backup.guildId,
            });
        }

        return Ok(backups.ToArray());
    }
}
