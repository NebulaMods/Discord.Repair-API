using Discord;
using Discord.Rest;

using DiscordRepair.Api.Database;
using DiscordRepair.Api.Database.Models;
using DiscordRepair.Api.Database.Models.BackupModels;
using DiscordRepair.Api.Database.Models.BackupModels.Channel;
using DiscordRepair.Api.Database.Models.BackupModels.Permissions;

using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.MigrationMaster;

/// <summary>
/// 
/// </summary>
public static class Backup
{
    #region Backup Roles

    internal static ICollection<Role> BackupRoles(this Database.Models.BackupModels.Backup backup, RestGuild guild)
    {
        backup.roles ??= new HashSet<Role>();
        var guildRoles = guild.Roles.ToList();
        if (guildRoles is not null)
            foreach (IRole? role in guildRoles)
            {
                var updatedRole = UpdateRole(backup, role);
                if (updatedRole is not null)
                    backup.roles.Add(updatedRole);
            }
        return backup.roles;
    }

    internal static Role? UpdateRole(Database.Models.BackupModels.Backup backup, IRole role)
    {
        Role? roleEntry = null;
        if (backup.roles is null)
            return roleEntry;
        roleEntry = backup.roles.FirstOrDefault(x => x.id == role.Id);
        GuildPermissions perm = role.Permissions;

        if (roleEntry is null)
        {
            roleEntry = new Role
            {
                key = new(),
                id = role.Id,
                name = role.Name,
                icon = role.Icon,
                color = role.Color.RawValue,
                isEveryone = role.Guild.EveryoneRole.Id == role.Id,
                isHoisted = role.IsHoisted,
                isManaged = role.IsManaged,
                isMentionable = role.IsMentionable,
                position = role.Position,
                permissions = new RolePermissions
                {
                    AddReactions = role.Permissions.AddReactions,
                    Administrator = role.Permissions.Administrator,
                    AttachFiles = role.Permissions.AttachFiles,
                    ManageEmojisAndStickers = role.Permissions.ManageEmojisAndStickers,
                    StartEmbeddedActivities = role.Permissions.StartEmbeddedActivities,
                    UseApplicationCommands = role.Permissions.UseApplicationCommands,
                    UseVAD = role.Permissions.UseVAD,
                    ViewAuditLog = role.Permissions.ViewAuditLog,
                    BanMembers = role.Permissions.BanMembers,
                    ChangeNickname = role.Permissions.ChangeNickname,
                    Connect = role.Permissions.Connect,
                    CreateInstantInvite = role.Permissions.CreateInstantInvite,
                    CreatePrivateThreads = role.Permissions.CreatePrivateThreads,
                    CreatePublicThreads = role.Permissions.CreatePublicThreads,
                    DeafenMembers = role.Permissions.DeafenMembers,
                    EmbedLinks = role.Permissions.EmbedLinks,
                    KickMembers = role.Permissions.KickMembers,
                    ManageChannels = role.Permissions.ManageChannels,
                    ManageEvents = role.Permissions.ManageEvents,
                    ManageGuild = role.Permissions.ManageGuild,
                    ManageRoles = role.Permissions.ManageRoles,
                    ManageMessages = role.Permissions.ManageMessages,
                    ManageNicknames = role.Permissions.ManageNicknames,
                    ManageThreads = role.Permissions.ManageThreads,
                    ManageWebhooks = role.Permissions.ManageWebhooks,
                    MentionEveryone = role.Permissions.MentionEveryone,
                    MoveMembers = role.Permissions.MoveMembers,
                    MuteMembers = role.Permissions.MuteMembers,
                    PrioritySpeaker = role.Permissions.PrioritySpeaker,
                    ReadMessageHistory = role.Permissions.ReadMessageHistory,
                    RequestToSpeak = role.Permissions.RequestToSpeak,
                    SendMessages = role.Permissions.SendMessages,
                    SendMessagesInThreads = role.Permissions.SendMessagesInThreads,
                    SendTTSMessages = role.Permissions.SendTTSMessages,
                    Speak = role.Permissions.Speak,
                    Stream = role.Permissions.Stream,
                    UseExternalEmojis = role.Permissions.UseExternalEmojis,
                    UseExternalStickers = role.Permissions.UseExternalStickers,
                    ViewChannel = role.Permissions.ViewChannel,
                    ViewGuildInsights = role.Permissions.ViewGuildInsights,
                }
            };
            return roleEntry;
        }
        #region Update
        roleEntry.color = role.Color.RawValue;
        roleEntry.name = role.Name;
        roleEntry.id = role.Id;
        roleEntry.icon = role.Icon;
        roleEntry.isEveryone = role.Guild.EveryoneRole.Id == role.Id;
        roleEntry.isHoisted = role.IsHoisted;
        roleEntry.isManaged = role.IsManaged;
        roleEntry.isMentionable = role.IsMentionable;
        roleEntry.position = role.Position;
        roleEntry.permissions.AddReactions = role.Permissions.AddReactions;
        roleEntry.permissions.Administrator = role.Permissions.Administrator;
        roleEntry.permissions.AttachFiles = role.Permissions.AttachFiles;
        roleEntry.permissions.ManageEmojisAndStickers = role.Permissions.ManageEmojisAndStickers;
        roleEntry.permissions.StartEmbeddedActivities = role.Permissions.StartEmbeddedActivities;
        roleEntry.permissions.UseApplicationCommands = role.Permissions.UseApplicationCommands;
        roleEntry.permissions.UseVAD = role.Permissions.UseVAD;
        roleEntry.permissions.ViewAuditLog = role.Permissions.ViewAuditLog;
        roleEntry.permissions.BanMembers = role.Permissions.BanMembers;
        roleEntry.permissions.ChangeNickname = role.Permissions.ChangeNickname;
        roleEntry.permissions.Connect = role.Permissions.Connect;
        roleEntry.permissions.CreateInstantInvite = role.Permissions.CreateInstantInvite;
        roleEntry.permissions.CreatePrivateThreads = role.Permissions.CreatePrivateThreads;
        roleEntry.permissions.CreatePublicThreads = role.Permissions.CreatePublicThreads;
        roleEntry.permissions.DeafenMembers = role.Permissions.DeafenMembers;
        roleEntry.permissions.EmbedLinks = role.Permissions.EmbedLinks;
        roleEntry.permissions.KickMembers = role.Permissions.KickMembers;
        roleEntry.permissions.ManageChannels = role.Permissions.ManageChannels;
        roleEntry.permissions.ManageEvents = role.Permissions.ManageEvents;
        roleEntry.permissions.ManageGuild = role.Permissions.ManageGuild;
        roleEntry.permissions.ManageRoles = role.Permissions.ManageRoles;
        roleEntry.permissions.ManageMessages = role.Permissions.ManageMessages;
        roleEntry.permissions.ManageNicknames = role.Permissions.ManageNicknames;
        roleEntry.permissions.ManageThreads = role.Permissions.ManageThreads;
        roleEntry.permissions.ManageWebhooks = role.Permissions.ManageWebhooks;
        roleEntry.permissions.MentionEveryone = role.Permissions.MentionEveryone;
        roleEntry.permissions.MoveMembers = role.Permissions.MoveMembers;
        roleEntry.permissions.MuteMembers = role.Permissions.MuteMembers;
        roleEntry.permissions.PrioritySpeaker = role.Permissions.PrioritySpeaker;
        roleEntry.permissions.ReadMessageHistory = role.Permissions.ReadMessageHistory;
        roleEntry.permissions.RequestToSpeak = role.Permissions.RequestToSpeak;
        roleEntry.permissions.SendMessages = role.Permissions.SendMessages;
        roleEntry.permissions.SendMessagesInThreads = role.Permissions.SendMessagesInThreads;
        roleEntry.permissions.SendTTSMessages = role.Permissions.SendTTSMessages;
        roleEntry.permissions.Speak = role.Permissions.Speak;
        roleEntry.permissions.Stream = role.Permissions.Stream;
        roleEntry.permissions.UseExternalEmojis = role.Permissions.UseExternalEmojis;
        roleEntry.permissions.UseExternalStickers = role.Permissions.UseExternalStickers;
        roleEntry.permissions.ViewChannel = role.Permissions.ViewChannel;
        roleEntry.permissions.ViewGuildInsights = role.Permissions.ViewGuildInsights;
        #endregion
        return roleEntry;
    }
    #endregion

    #region Backup Channels
    internal static async Task<ICollection<CategoryChannel>> BackupCategoriesAsync(this Database.Models.BackupModels.Backup backup, Discord.Rest.RestGuild guild)
    {
        backup.catgeoryChannels ??= new HashSet<CategoryChannel>();
        var guildChannels = await guild.GetCategoryChannelsAsync();
        if (guildChannels is not null)
            foreach (var channel in guildChannels)
            {
                var updatedChannel = UpdateCategoryChannel(backup, channel);
                if (updatedChannel is not null)
                    backup.catgeoryChannels.Add(updatedChannel);
            }
        return backup.catgeoryChannels;
    }
    internal static async Task<ICollection<TextChannel>> BackupTextChannelsAsync(this Database.Models.BackupModels.Backup backup, Discord.Rest.RestGuild guild)
    {
        backup.textChannels ??= new HashSet<TextChannel>();
        var guildChannels = await guild.GetTextChannelsAsync();
        if (guildChannels is not null)
            foreach (var channel in guildChannels)
            {
                if (channel as Discord.Rest.RestThreadChannel is null)
                {
                    var updatedChannel = UpdateTextChannel(backup, channel);
                    if (updatedChannel is not null)
                        backup.textChannels.Add(updatedChannel);
                }
            }
        return backup.textChannels;
    }
    internal static async Task<ICollection<VoiceChannel>> BackupVoiceChannelsAsync(this Database.Models.BackupModels.Backup backup, Discord.Rest.RestGuild guild)
    {
        backup.voiceChannels ??= new HashSet<VoiceChannel>();
        var guildChannels = await guild.GetVoiceChannelsAsync();
        if (guildChannels is not null)
            foreach (var channel in guildChannels)
            {
                var updatedChannel = UpdateVoiceChannel(backup, channel);
                if (updatedChannel is not null)
                    backup.voiceChannels.Add(updatedChannel);
            }
        return backup.voiceChannels;
    }

    internal static CategoryChannel? UpdateCategoryChannel(this Database.Models.BackupModels.Backup backup, ICategoryChannel channel)
    {
        CategoryChannel? channelEntry = null;
        if (backup.catgeoryChannels is null)
            return channelEntry;
        channelEntry = backup.catgeoryChannels.FirstOrDefault(x => x.id == channel.Id);
        if (channelEntry is null)
        {
            var permissionList = new HashSet<Database.Models.BackupModels.Permissions.ChannelPermissions>();
            var channelPerms2 = channel.PermissionOverwrites.ToList();
            foreach (Overwrite permission in channelPerms2)
            {
                permissionList.Add(CreateChannelPermissionEntry(permission, permission.TargetId));
            }
            channelPerms2.Clear();
            return new CategoryChannel
            {
                key = new(),
                id = channel.Id,
                name = channel.Name,
                position = channel.Position,
                permissions = permissionList,
            };
        }
        channelEntry.name = channel.Name;
        channelEntry.position = channel.Position;
        var channelPerms = channel.PermissionOverwrites.ToList();
        foreach (Overwrite x in channelPerms)
        {
            Database.Models.BackupModels.Permissions.ChannelPermissions? permission = channelEntry.permissions.FirstOrDefault(y => y.targetId == x.TargetId);
            if (permission is not null)
            {
                UpdateChannelPermissionEntry(permission, x);
                continue;
            }
            channelEntry.permissions.Add(CreateChannelPermissionEntry(x, x.TargetId));
        }
        channelPerms.Clear();
        return channelEntry;
    }
    internal static TextChannel? UpdateTextChannel(this Database.Models.BackupModels.Backup backup, ITextChannel channel)
    {
        TextChannel? channelEntry = null;
        if (backup.textChannels is null)
            return channelEntry;
        channelEntry = backup.textChannels.FirstOrDefault(x => x.id == channel.Id);
        if (channelEntry is null)
        {
            var permissionList = new HashSet<Database.Models.BackupModels.Permissions.ChannelPermissions>();
            var channelPerms1 = channel.PermissionOverwrites.ToList();
            foreach (Overwrite x in channelPerms1)
            {
                permissionList.Add(CreateChannelPermissionEntry(x, x.TargetId));
            }
            channelPerms1.Clear();
            return channel as Discord.Rest.RestNewsChannel is not null
            ? new TextChannel
            {
                key = new(),
                id = channel.Id,
                name = channel.Name,
                position = channel.Position,
                category = channel.CategoryId is not null ? backup.catgeoryChannels?.FirstOrDefault(x => x.id == channel.CategoryId) : null,
                archiveAfter = null,
                nsfw = channel.IsNsfw,
                slowModeInterval = 0,
                topic = channel.Topic,
                permissions = permissionList,
            }
            : new TextChannel
            {
                id = channel.Id,
                name = channel.Name,
                position = channel.Position,
                category = channel.CategoryId is not null ? backup.catgeoryChannels?.FirstOrDefault(x => x.id == channel.CategoryId) : null,
                archiveAfter = null,
                nsfw = channel.IsNsfw,
                slowModeInterval = channel.SlowModeInterval,
                topic = channel.Topic,
                permissions = permissionList,
            };
        }
        else
        {
            channelEntry.name = channel.Name;
            channelEntry.position = channel.Position;
            channelEntry.category = channel.CategoryId is not null ? backup.catgeoryChannels?.FirstOrDefault(x => x.id == channel.CategoryId) : null;
            channelEntry.archiveAfter = null;
            channelEntry.nsfw = channel.IsNsfw;
            channelEntry.topic = channel.Topic;
            if (channel as Discord.Rest.RestNewsChannel is null)
            {
                channelEntry.slowModeInterval = channel.SlowModeInterval;
            }

            var channelPerms = channel.PermissionOverwrites.ToList();
            foreach (Overwrite x in channelPerms)
            {
                Database.Models.BackupModels.Permissions.ChannelPermissions? permission = channelEntry.permissions.FirstOrDefault(y => y.targetId == x.TargetId);
                if (permission is not null)
                {
                    UpdateChannelPermissionEntry(permission, x);
                    continue;
                }
                channelEntry.permissions.Add(CreateChannelPermissionEntry(x, x.TargetId));
            }
            channelPerms.Clear();
        }
        return channelEntry;
    }
    internal static VoiceChannel? UpdateVoiceChannel(this Database.Models.BackupModels.Backup backup, IVoiceChannel channel)
    {
        VoiceChannel? channelEntry = null;
        if (backup.voiceChannels is null)
            return channelEntry;
        channelEntry = backup.voiceChannels.FirstOrDefault(x => x.id == channel.Id);
        if (channelEntry is null)
        {
            var permissionList = new HashSet<Database.Models.BackupModels.Permissions.ChannelPermissions>();
            var channelPerms1 = channel.PermissionOverwrites.ToList();
            foreach (Overwrite permission in channelPerms1)
            {
                permissionList.Add(CreateChannelPermissionEntry(permission, permission.TargetId));
            }
            channelPerms1.Clear();
            return new VoiceChannel
            {
                key = new(),
                id = channel.Id,
                name = channel.Name,
                position = channel.Position,
                category = channel.CategoryId is not null ? backup.catgeoryChannels?.FirstOrDefault(x => x.id == channel.CategoryId) : null,
                bitrate = channel.Bitrate,
                userLimit = channel.UserLimit,
                videoQuality = null,
                region = null,
                permissions = permissionList,
            };
        }
        channelEntry.name = channel.Name;
        channelEntry.position = channel.Position;
        channelEntry.category = channel.CategoryId is not null ? backup.catgeoryChannels?.FirstOrDefault(x => x.id == channel.CategoryId) : null;
        channelEntry.bitrate = channel.Bitrate;
        channelEntry.userLimit = channel.UserLimit;
        var channelPerms2 = channel.PermissionOverwrites.ToList();
        foreach (Overwrite x in channelPerms2)
        {
            Database.Models.BackupModels.Permissions.ChannelPermissions? permission = channelEntry.permissions.FirstOrDefault(y => y.targetId == x.TargetId);
            if (permission is not null)
            {
                UpdateChannelPermissionEntry(permission, x);
                continue;
            }
            channelEntry.permissions.Add(CreateChannelPermissionEntry(x, x.TargetId));
        }
        channelPerms2.Clear();
        return channelEntry;
    }
    private static Database.Models.BackupModels.Permissions.ChannelPermissions CreateChannelPermissionEntry(Overwrite x, ulong targetId)
    {
        return new Database.Models.BackupModels.Permissions.ChannelPermissions
        {
            key = new(),
            targetId = targetId,
            permissionTarget = (int)x.TargetType,
            AttachFiles = (PermissionValue)x.Permissions.AttachFiles,
            AddReactions = (PermissionValue)x.Permissions.AddReactions,
            StartEmbeddedActivities = (PermissionValue)x.Permissions.StartEmbeddedActivities,
            UseApplicationCommands = (PermissionValue)x.Permissions.UseApplicationCommands,
            UseVAD = (PermissionValue)x.Permissions.UseVAD,
            Connect = (PermissionValue)x.Permissions.Connect,
            CreateInstantInvite = (PermissionValue)x.Permissions.CreateInstantInvite,
            CreatePrivateThreads = (PermissionValue)x.Permissions.CreatePrivateThreads,
            CreatePublicThreads = (PermissionValue)x.Permissions.CreatePublicThreads,
            DeafenMembers = (PermissionValue)x.Permissions.DeafenMembers,
            EmbedLinks = (PermissionValue)x.Permissions.EmbedLinks,
            ManageChannel = (PermissionValue)x.Permissions.ManageChannel,
            ManageMessages = (PermissionValue)x.Permissions.ManageMessages,
            ManageRoles = (PermissionValue)x.Permissions.ManageRoles,
            ManageThreads = (PermissionValue)x.Permissions.ManageThreads,
            ManageWebhooks = (PermissionValue)x.Permissions.ManageWebhooks,
            MentionEveryone = (PermissionValue)x.Permissions.MentionEveryone,
            MoveMembers = (PermissionValue)x.Permissions.MoveMembers,
            MuteMembers = (PermissionValue)x.Permissions.MuteMembers,
            PrioritySpeaker = (PermissionValue)x.Permissions.PrioritySpeaker,
            ReadMessageHistory = (PermissionValue)x.Permissions.ReadMessageHistory,
            RequestToSpeak = (PermissionValue)x.Permissions.RequestToSpeak,
            SendMessages = (PermissionValue)x.Permissions.SendMessages,
            SendMessagesInThreads = (PermissionValue)x.Permissions.SendMessagesInThreads,
            SendTTSMessages = (PermissionValue)x.Permissions.SendTTSMessages,
            Speak = (PermissionValue)x.Permissions.Speak,
            Stream = (PermissionValue)x.Permissions.Stream,
            UseExternalEmojis = (PermissionValue)x.Permissions.UseExternalEmojis,
            UseExternalStickers = (PermissionValue)x.Permissions.UseExternalStickers,
            ViewChannel = (PermissionValue)x.Permissions.ViewChannel,
        };
    }
    private static void UpdateChannelPermissionEntry(Database.Models.BackupModels.Permissions.ChannelPermissions permission, Overwrite x)
    {
        permission.targetId = x.TargetId;
        permission.permissionTarget = (int)x.TargetType;
        permission.AttachFiles = (PermissionValue)x.Permissions.AttachFiles;
        permission.AddReactions = (PermissionValue)x.Permissions.AddReactions;
        permission.StartEmbeddedActivities = (PermissionValue)x.Permissions.StartEmbeddedActivities;
        permission.UseApplicationCommands = (PermissionValue)x.Permissions.UseApplicationCommands;
        permission.UseVAD = (PermissionValue)x.Permissions.UseVAD;
        permission.Connect = (PermissionValue)x.Permissions.Connect;
        permission.CreateInstantInvite = (PermissionValue)x.Permissions.CreateInstantInvite;
        permission.CreatePrivateThreads = (PermissionValue)x.Permissions.CreatePrivateThreads;
        permission.CreatePublicThreads = (PermissionValue)x.Permissions.CreatePublicThreads;
        permission.DeafenMembers = (PermissionValue)x.Permissions.DeafenMembers;
        permission.EmbedLinks = (PermissionValue)x.Permissions.EmbedLinks;
        permission.ManageChannel = (PermissionValue)x.Permissions.ManageChannel;
        permission.ManageMessages = (PermissionValue)x.Permissions.ManageMessages;
        permission.ManageRoles = (PermissionValue)x.Permissions.ManageRoles;
        permission.ManageThreads = (PermissionValue)x.Permissions.ManageThreads;
        permission.ManageWebhooks = (PermissionValue)x.Permissions.ManageWebhooks;
        permission.MentionEveryone = (PermissionValue)x.Permissions.MentionEveryone;
        permission.MoveMembers = (PermissionValue)x.Permissions.MoveMembers;
        permission.MuteMembers = (PermissionValue)x.Permissions.MuteMembers;
        permission.PrioritySpeaker = (PermissionValue)x.Permissions.PrioritySpeaker;
        permission.ReadMessageHistory = (PermissionValue)x.Permissions.ReadMessageHistory;
        permission.RequestToSpeak = (PermissionValue)x.Permissions.RequestToSpeak;
        permission.SendMessages = (PermissionValue)x.Permissions.SendMessages;
        permission.SendMessagesInThreads = (PermissionValue)x.Permissions.SendMessagesInThreads;
        permission.SendTTSMessages = (PermissionValue)x.Permissions.SendTTSMessages;
        permission.Speak = (PermissionValue)x.Permissions.Speak;
        permission.Stream = (PermissionValue)x.Permissions.Stream;
        permission.UseExternalEmojis = (PermissionValue)x.Permissions.UseExternalEmojis;
        permission.UseExternalStickers = (PermissionValue)x.Permissions.UseExternalStickers;
        permission.ViewChannel = (PermissionValue)x.Permissions.ViewChannel;
    }
    #endregion

    internal static async Task<ICollection<GuildUser>> BackupUsersAsync(this Database.Models.BackupModels.Backup backup, RestGuild guild)
    {
        backup.users ??= new HashSet<GuildUser>();
        var users = await guild.GetUsersAsync().FlattenAsync();
        if (users is not null)
            foreach (var guildUser in users)
            {
                IReadOnlyCollection<ulong>? userRoles = guildUser.RoleIds;
                if (userRoles.Any() is false)
                {
                    continue;
                }

                var newGuildUser = new GuildUser
                {
                    key = new(),
                    id = guildUser.Id,
                    avatarUrl = guildUser.GetAvatarUrl(),
                    username = guildUser.Username,
                };
                foreach (ulong role in userRoles)
                {
                    Role? idkRole = backup.roles?.FirstOrDefault(x => x.id == role);
                    if (idkRole is null)
                    {
                        continue;
                    }

                    newGuildUser.assignedRoles.Add(idkRole);
                }
                if (newGuildUser.assignedRoles.Any() is false)
                {
                    continue;
                }

                backup.users.Add(newGuildUser);
            }
        return backup.users;
    }

    internal static async Task<ICollection<Database.Models.BackupModels.Emoji>> BackupEmojisAsync(this Database.Models.BackupModels.Backup backup, RestGuild guild)
    {
        backup.emojis ??= new HashSet<Database.Models.BackupModels.Emoji>();
        var emojis = await guild.GetEmotesAsync();
        if (emojis is not null)
            foreach (GuildEmote? emoji in emojis)
            {
                var newEmoji = new Database.Models.BackupModels.Emoji
                {
                    key = new(),
                    name = emoji.Name,
                    url = emoji.Url,
                };
                backup.emojis.Add(newEmoji);
            }
        return backup.emojis;
    }
    internal static async Task<ICollection<Database.Models.BackupModels.Sticker>> BackupStickersAsync(this Database.Models.BackupModels.Backup backup, RestGuild guild)
    {
        backup.stickers ??= new HashSet<Database.Models.BackupModels.Sticker>();
        var stickers = await guild.GetStickersAsync();
        if (stickers is not null)
            if (stickers.Any())
                foreach (var sticker in stickers)
                {
                    if (sticker.Type == StickerType.Guild)
                    {
                        var newSticker = new Database.Models.BackupModels.Sticker
                        {
                            key = new(),
                            name = sticker.Name,
                            url = sticker.GetStickerUrl(),
                            isAvailable = sticker.IsAvailable,
                            description = sticker.Description,
                            format = sticker.Format,
                            packId = sticker.PackId,
                            sortOrder = sticker.SortOrder,
                            tags = sticker.Tags.ToArray()
                        };
                        backup.stickers.Add(newSticker);
                    }
                }
        return backup.stickers;
    }

    internal static async Task<ICollection<Message>> BackupMessagesAsync(this Database.Models.BackupModels.Backup backup, RestGuild guild, int messageCount = 1000)
    {
        backup.messages ??= new HashSet<Message>();

        var txtChannels = await guild.GetTextChannelsAsync();
        if (txtChannels is not null)
            foreach (var channel in txtChannels)
            {
                var messages = await channel.GetMessagesAsync(messageCount).FlattenAsync();
                if (messages is not null)
                    foreach (var message in messages)
                    {
                        switch (message.Type)
                        {
                            case MessageType.Default:
                            case MessageType.Reply:
                                var txtChannel = backup.textChannels?.FirstOrDefault(x => x.id == message.Channel.Id);
                                if (txtChannel is null) continue;
                                backup.messages.Add(new Message()
                                {
                                    content = message.Content,
                                    authorId = message.Author.Id,
                                    createdAt = message.CreatedAt,
                                    channel = txtChannel,
                                    flags = message.Flags,
                                    isPinned = message.IsPinned,
                                    isSuppressed = message.IsSuppressed,
                                    isTTS = message.IsTTS,
                                    source = message.Source,
                                    threadChannelId = message.Thread?.Id,
                                    type = message.Type,
                                });
                                break;
                        }
                    }
            }

        return backup.messages;
    }

    internal static async Task<Guild> BackupGuildAsync(this Database.Models.BackupModels.Backup backup, RestGuild guild)
    {
        backup.guild ??= new();

        backup.guild = new Guild
        {
            key = new(),
            guildName = guild.Name,
            afkTimeout = guild.AFKTimeout,
            bannerUrl = guild.BannerUrl,
            description = guild.Description,
            discoverySplashUrl = guild.DiscoverySplashUrl,
            iconUrl = guild.IconUrl is not null ? guild.IconUrl.Replace(".jpg", ".png") : guild.IconUrl,
            splashUrl = guild.SplashUrl,
            vanityUrl = string.IsNullOrWhiteSpace(guild.VanityURLCode) ? guild.VanityURLCode : $"https://discord.gg/{guild.VanityURLCode}",
            isWidgetEnabled = guild.IsWidgetEnabled,
            defaultMessageNotifications = (int)guild.DefaultMessageNotifications,
            explicitContentFilterLevel = (int)guild.ExplicitContentFilter,
            preferredLocale = guild.PreferredLocale,
            isBoostProgressBarEnabled = guild.IsBoostProgressBarEnabled,
            systemChannelMessageDeny = (int)guild.SystemChannelFlags,
            verificationLevel = (int)guild.VerificationLevel,
            afkChannelId = guild.AFKChannelId,
            defaultChannelId = (await guild.GetDefaultChannelAsync())?.Id,
            publicUpdatesChannelId = guild.PublicUpdatesChannelId,
            rulesChannelId = guild.RulesChannelId,
            systemChannelId = guild.SystemChannelId,
            widgetChannelId = guild.WidgetChannelId,
        };
        return backup.guild;
    }


}
