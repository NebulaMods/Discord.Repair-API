
using Discord.Rest;

using DiscordRepair.Api.Database;
using DiscordRepair.Api.Database.Models;
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
    [HttpPut]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BackupResponse), 201)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<BackupResponse>> HandleAsync(BackupRequest backupRequest)
    {
        if (backupRequest is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid parameters"
            });
        }

        if (backupRequest.guildId is 0 || string.IsNullOrWhiteSpace(backupRequest.name))
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid parameters"
            });
        }

        if (string.IsNullOrWhiteSpace(backupRequest.serverName) && string.IsNullOrWhiteSpace(backupRequest.botName))
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid parameters"
            });
        }

        var user = await HttpContext.GetCurrentUserAsync();
        if (user is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid user"
            });
        }
        await using var database = new DatabaseContext();
        Database.Models.CustomBot? bot = null;
        if (string.IsNullOrWhiteSpace(backupRequest.botName) is false)
        {
            bot = user.bots.FirstOrDefault(x => x.name == backupRequest.botName);
            if (bot is null)
            {
                return BadRequest(new Generic()
                {
                    success = false,
                    details = "invalid bot"
                });
            }
        }
        else
        {
            var server = await database.servers.FirstOrDefaultAsync(x => x.name == backupRequest.serverName);
            if (server is null)
            {
                return BadRequest(new Generic()
                {
                    success = false,
                    details = "invalid server"
                });
            }
            bot = server.settings.mainBot;
        }
        //check if backup exists already
        await using var discordClient = new DiscordRestClient();
        await discordClient.LoginAsync(Discord.TokenType.Bot, bot.token);
        var restGuild = await discordClient.GetGuildAsync(backupRequest.guildId);
        //check perms on server

        Database.Models.BackupModels.Backup backup = new()
        {
            name = backupRequest.name,
            type = backupRequest.type,
            guildId = backupRequest.guildId
        };
        switch (backupRequest.type)
        {
            case BackupType.EMOJIS:
                await backup.BackupEmojisAsync(restGuild);
                break;
            case BackupType.MESSAGES:
                await backup.BackupCategoriesAsync(restGuild);
                await backup.BackupTextChannelsAsync(restGuild);
                await backup.BackupMessagesAsync(restGuild);
                break;
            case BackupType.CHANNELS:
                await backup.BackupCategoriesAsync(restGuild);
                await Task.WhenAll(backup.BackupTextChannelsAsync(restGuild), backup.BackupVoiceChannelsAsync(restGuild));
                break;
            case BackupType.USER_ROLES:
                backup.BackupRoles(restGuild);
                await backup.BackupUsersAsync(restGuild);
                break;
            case BackupType.ROLES:
                backup.BackupRoles(restGuild);
                break;
            case BackupType.STICKERS:
                await backup.BackupStickersAsync(restGuild);
                break;
            case BackupType.GUILD:
                await backup.BackupGuildAsync(restGuild);
                break;
            case BackupType.FULL:
                backup.BackupRoles(restGuild);
                await backup.BackupCategoriesAsync(restGuild);
                var guildBackupTask = backup.BackupGuildAsync(restGuild);
                var txtChannelBackupTask = backup.BackupTextChannelsAsync(restGuild);
                var voiceChannelBackupTask = backup.BackupVoiceChannelsAsync(restGuild);
                var userBackupTask = backup.BackupUsersAsync(restGuild);
                var emojiBackupTask = backup.BackupEmojisAsync(restGuild);
                var stickerBackupTask = backup.BackupStickersAsync(restGuild);
                await txtChannelBackupTask;
                var messageBackupTask = backup.BackupMessagesAsync(restGuild);
                await Task.WhenAll(guildBackupTask, voiceChannelBackupTask, userBackupTask, emojiBackupTask, stickerBackupTask, messageBackupTask);

                break;
            default:
                return BadRequest(new Generic()
                {
                    success = false,
                    details = "invalid backup type"
                });
        }
        user.backups.Add(backup);
        await database.ApplyChangesAsync(user);
        return Created($"https://api.discord.repair/backup/{backup.name}", new BackupResponse()
        {
            creationTime = backup.creationDate,
            name = backup.name,
            type = backupRequest.type,
            channelCount = backup.catgeoryChannels?.Count ?? 0 + backup.textChannels?.Count ?? 0 + backup.voiceChannels?.Count ?? 0,
            emojiCount = backup.emojis?.Count ?? 0,
            messageCount = backup.messages?.Count ?? 0,
            roleCount = backup.roles?.Count ?? 0,
            stickerCount = backup.stickers?.Count ?? 0,
            userRoleCount = backup.users?.Count ?? 0,
            guildId = backup.guildId,
        });
    }

    public record BackupRequest
    {
        public BackupType type { get; set; }

        public string? botName { get; set; }

        public string? serverName { get; set; }

        public string name { get; set; }
        public ulong guildId { get; set; }
    }

    public record BackupResponse
    {
        public DateTime creationTime { get; set; }
        public string name { get; set; }
        public BackupType type { get; set; }
        public int emojiCount { get; set; }
        public int stickerCount { get; set; }
        public int messageCount { get; set; }
        public int roleCount { get; set; }
        public int userRoleCount { get; set; }
        public int channelCount { get; set; }
        public ulong guildId { get; set; }
    }
}
