using Discord.WebSocket;
using Discord;
using RestoreCord.Database.Models.BackupModels.Channel;
using RestoreCord.Database.Models.BackupModels.Permissions;
using RestoreCord.Database.Models.BackupModels;
using RestoreCord.Database;
using Discord.Interactions;
using RestoreCord.Database.Models;
using RestoreCord.Utilities;

namespace RestoreCord.Migrations;

/// <summary>
/// 
/// </summary>
public class Backup
{
    private readonly Configuration _configuration;
    public Backup(Configuration configuration)
    {
        _configuration = configuration;
    }

    public async ValueTask<Database.Models.BackupModels.Backup> BackupGuildAsync(Server server, DatabaseContext database, SocketGuild guild, ShardedInteractionContext context)
    {
        var backupEntry = new Database.Models.BackupModels.Backup
        {
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
        };
        server.backup = backupEntry;
        await database.ApplyChangesAsync(server);
        backupEntry.roles = BackupRoles(backupEntry, guild);
        await database.ApplyChangesAsync(server);
        if (await DiscordExtensions.CheckBusinessMembership(database, context, false))
        {
            backupEntry.users = await BackupUsersAsync(backupEntry, guild);
            guild.PurgeUserCache();
        }
        backupEntry.catgeoryChannels = BackupCategories(backupEntry, guild);
        backupEntry.textChannels = BackupTextChannels(backupEntry, guild);
        backupEntry.voiceChannels = BackupVoiceChannels(backupEntry, guild);
        backupEntry.emojis = await BackupEmojisAsync(backupEntry,guild);

        //
        //if (guild.AFKChannel is not null)
        //    backupEntry.afkChannel = backupEntry.voiceChannels.FirstOrDefault(x => x.id == guild.AFKChannel.Id);
        //if (guild.RulesChannel is not null)
        //    backupEntry.rulesChannel = backupEntry.textChannels.FirstOrDefault(x => x.id == guild.RulesChannel.Id);
        //if (guild.DefaultChannel is not null)
        //    backupEntry.defaultChannel = backupEntry.textChannels.FirstOrDefault(x => x.id == guild.DefaultChannel.Id);
        //if (guild.SystemChannel is not null)
        //    backupEntry.systemChannel = backupEntry.textChannels.FirstOrDefault(x => x.id == guild.SystemChannel.Id);
        //if (guild.PublicUpdatesChannel is not null)
        //    backupEntry.publicUpdatesChannel = backupEntry.textChannels.FirstOrDefault(x => x.id == guild.PublicUpdatesChannel.Id);
        //if (guild.WidgetChannel is not null)
        //    backupEntry.widgetChannel = backupEntry.textChannels.FirstOrDefault(x => x.id == guild.WidgetChannel.Id);
        //messages
        await database.ApplyChangesAsync(backupEntry);
        return backupEntry;
    }

    #region Backup Roles

    private ICollection<Role> BackupRoles(Database.Models.BackupModels.Backup backup, IGuild guild)
    {
        var roleList = new HashSet<Role>();
        var guildRoles = guild.Roles.ToList();
        foreach (IRole? role in guildRoles)
        {
            roleList.Add(UpdateRole(backup, role));
        }
        guildRoles.Clear();
        return roleList;
    }

    internal static Role UpdateRole(Database.Models.BackupModels.Backup backup, IRole role)
    {
        Role? roleEntry = backup.roles.FirstOrDefault(x => x.id == role.Id);
        GuildPermissions perm = role.Permissions;

        if (roleEntry is null)
        {
            roleEntry = new Role
            {
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
    private Database.Models.BackupModels.Permissions.ChannelPermissions CreateChannelPermissionEntry(Overwrite x, ulong targetId)
    {
        return new Database.Models.BackupModels.Permissions.ChannelPermissions
        {
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
    private void UpdateChannelPermissionEntry(Database.Models.BackupModels.Permissions.ChannelPermissions permission, Overwrite x)
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

    internal ICollection<CategoryChannel> BackupCategories(Database.Models.BackupModels.Backup backup, SocketGuild guild)
    {
        var channels = new HashSet<CategoryChannel>();
        var guildChannels = guild.CategoryChannels.ToList();
        foreach (SocketCategoryChannel? channel in guildChannels)
        {
            channels.Add(UpdateCategoryChannel(backup, channel));
        }
        guildChannels.Clear();
        return channels;
    }
    internal ICollection<TextChannel> BackupTextChannels(Database.Models.BackupModels.Backup backup, SocketGuild guild)
    {
        var channels = new HashSet<TextChannel>();
        var guildChannels = guild.TextChannels.ToList();
        foreach (SocketTextChannel? channel in guildChannels)
        {
            if (channel as SocketThreadChannel is null)
                channels.Add(UpdateTextChannel(backup, channel));
        }
        guildChannels.Clear();
        return channels;
    }
    internal ICollection<VoiceChannel> BackupVoiceChannels(Database.Models.BackupModels.Backup backup, SocketGuild guild)
    {
        var channels = new HashSet<VoiceChannel>();
        var guildChannels = guild.VoiceChannels.ToList();
        foreach (SocketVoiceChannel? channel in guildChannels)
        {
            channels.Add(UpdateVoiceChannel(backup, channel));
        }
        guildChannels.Clear();
        return channels;
    }

    internal CategoryChannel UpdateCategoryChannel(Database.Models.BackupModels.Backup backup, ICategoryChannel channel)
    {
        CategoryChannel? channelEntry = backup.catgeoryChannels.FirstOrDefault(x => x.id == channel.Id);
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
    internal TextChannel UpdateTextChannel(Database.Models.BackupModels.Backup backup, ITextChannel channel)
    {
        TextChannel? channelEntry = backup.textChannels.FirstOrDefault(x => x.id == channel.Id);
        if (channelEntry is null)
        {
            var permissionList = new HashSet<Database.Models.BackupModels.Permissions.ChannelPermissions>();
            var channelPerms1 = channel.PermissionOverwrites.ToList();
            foreach (Overwrite x in channelPerms1)
            {
                permissionList.Add(CreateChannelPermissionEntry(x, x.TargetId));
            }
            channelPerms1.Clear();
            return channel as SocketNewsChannel is not null
            ? new TextChannel
            {
                id = channel.Id,
                name = channel.Name,
                position = channel.Position,
                category = channel.CategoryId is not null ? backup.catgeoryChannels.FirstOrDefault(x => x.id == channel.CategoryId) : null,
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
                category = channel.CategoryId is not null ? backup.catgeoryChannels.FirstOrDefault(x => x.id == channel.CategoryId) : null,
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
            channelEntry.category = channel.CategoryId is not null ? backup.catgeoryChannels.FirstOrDefault(x => x.id == channel.CategoryId) : null;
            channelEntry.archiveAfter = null;
            channelEntry.nsfw = channel.IsNsfw;
            channelEntry.topic = channel.Topic;
            if (channel as SocketNewsChannel is null)
                channelEntry.slowModeInterval = channel.SlowModeInterval;
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
    internal VoiceChannel UpdateVoiceChannel(Database.Models.BackupModels.Backup backup, IVoiceChannel channel)
    {
        VoiceChannel? channelEntry = backup.voiceChannels.FirstOrDefault(x => x.id == channel.Id);
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
                id = channel.Id,
                name = channel.Name,
                position = channel.Position,
                category = channel.CategoryId is not null ? backup.catgeoryChannels.FirstOrDefault(x => x.id == channel.CategoryId) : null,
                bitrate = channel.Bitrate,
                userLimit = channel.UserLimit,
                videoQuality = null,
                region = null,
                permissions = permissionList,
            };
        }
        channelEntry.name = channel.Name;
        channelEntry.position = channel.Position;
        channelEntry.category = channel.CategoryId is not null ? backup.catgeoryChannels.FirstOrDefault(x => x.id == channel.CategoryId) : null;
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

    #endregion

    internal async ValueTask<ICollection<GuildUser>> BackupUsersAsync(Database.Models.BackupModels.Backup backup, IGuild guild)
    {
        var guildUserList = new HashSet<GuildUser>();
        await guild.DownloadUsersAsync();
        IReadOnlyCollection<IGuildUser>? users = await guild.GetUsersAsync();
        foreach (IGuildUser? guildUser in users)
        {
            IReadOnlyCollection<ulong>? userRoles = guildUser.RoleIds;
            if (userRoles.Any() is false)
                continue;
            var newGuildUser = new GuildUser
            {
                id = guildUser.Id,
                avatarUrl = guildUser.GetAvatarUrl(),
                username = guildUser.Username,
            };
            foreach (ulong role in userRoles)
            {
                Role? idkRole = backup.roles.FirstOrDefault(x => x.id == role);
                if (idkRole is null)
                    continue;
                newGuildUser.assignedRoles.Add(new GuildUserRole
                {
                    role = idkRole
                });
            }
            if (newGuildUser.assignedRoles.Any() is false)
                continue;
            guildUserList.Add(newGuildUser);
        }
        return guildUserList;
    }

    internal async ValueTask<ICollection<Database.Models.BackupModels.Emoji>> BackupEmojisAsync(Database.Models.BackupModels.Backup backup, IGuild guild)
    {
        var guildEmojiList = new HashSet<Database.Models.BackupModels.Emoji>();
        IReadOnlyCollection<GuildEmote>? emojis = await guild.GetEmotesAsync();
        foreach (GuildEmote? emoji in emojis)
        {
            var newEmoji = new Database.Models.BackupModels.Emoji
            {
                name = emoji.Name,
                url = emoji.Url,
            };
            backup.emojis.Add(newEmoji);
            guildEmojiList.Add(newEmoji);
        }
        return guildEmojiList;
    }
}
