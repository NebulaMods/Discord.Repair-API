using DiscordRepair.Api.Database;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Mvc;

namespace DiscordRepair.Api.Endpoints.V1.Backup;

[ApiController]
[Route("/v1/backup/")]
[ApiExplorerSettings(GroupName = "Backup Endpoints")]
public class Delete : ControllerBase
{
    [HttpDelete("{backup}")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Generic>> HandleAsync(string backup)
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
        var backupEntry = currentUser.backups.FirstOrDefault(x => x.name == backup || x.key.ToString() == backup);
        if (backupEntry is null) 
        {
            return NotFound(new Generic()
            {
                success = false,
                details = "backup doesn't exist"
            });
        }
        await using var database = new DatabaseContext();
        if (backupEntry.catgeoryChannels is not null)
            database.RemoveRange(backupEntry.catgeoryChannels.SelectMany(x => x.permissions));
        if (backupEntry.textChannels is not null)
            database.RemoveRange(backupEntry.textChannels.SelectMany(x => x.permissions));
        if (backupEntry.voiceChannels is not null)
            database.RemoveRange(backupEntry.voiceChannels.SelectMany(x => x.permissions));
        if (backupEntry.users is not null)
            database.RemoveRange(backupEntry.users.SelectMany(x => x.assignedRoles));
        if (backupEntry.stickers is not null)
            database.RemoveRange(backupEntry.stickers);
        if (backupEntry.users is not null)
            database.RemoveRange(backupEntry.users);
        if (backupEntry.catgeoryChannels is not null)
            database.RemoveRange(backupEntry.catgeoryChannels);
        if (backupEntry.emojis is not null)
            database.RemoveRange(backupEntry.emojis);
        if (backupEntry.roles is not null)
            database.RemoveRange(backupEntry.roles);
        if (backupEntry.voiceChannels is not null)
            database.RemoveRange(backupEntry.voiceChannels);
        if (backupEntry.textChannels is not null)
            database.RemoveRange(backupEntry.textChannels);
        if (backupEntry.messages is not null)
            database.RemoveRange(backupEntry.messages);
        if (backupEntry.guild is not null)
            database.Remove(backupEntry.guild);
        currentUser.backups.Remove(backupEntry);
        database.Remove(backupEntry);
        await database.ApplyChangesAsync();

        return Ok(new Generic()
        {
            success = true,
            details = $"successfully deleted {backupEntry.name} backup"
        });
    }
}
