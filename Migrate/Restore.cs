using Discord;
using Discord.Rest;

using DiscordRepair.Api.Database.Models.BackupModels;
using DiscordRepair.Api.Database.Models.BackupModels.Channel;

namespace DiscordRepair.Api.MigrationMaster;

public static class Restore
{
    public record EmojiReport
    {
        public int successfullyRestoredCount { get; set; } = 0;
    }
    internal static async Task<EmojiReport?> RestoreEmojisAsync(this Database.Models.BackupModels.Backup backup, RestGuild guild, bool clearEmotes = false)
    {
        if (backup.emojis is null)
        {
            return null;
        }
        int emojiCount = 0;
        if (clearEmotes)
        {
            var emotes = await guild.GetEmotesAsync();
            foreach (var emote in emotes)
            {
                try
                {
                    await guild.DeleteEmoteAsync(emote);
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
                catch { }
            }
        }
        var emojiReport = new EmojiReport();
        using var _httpClient = new HttpClient();
        foreach (var emoji in backup.emojis)
        {
            try
            {
                if (emojiCount / 50 == 1)
                {
                    //notify user
                    await Task.Delay(TimeSpan.FromHours(1));
                }
                var newEmoji = await guild.CreateEmoteAsync(emoji.name, new Image(await _httpClient.GetStreamAsync(emoji.url)));
                if (newEmoji is not null)
                    emojiReport.successfullyRestoredCount++;
                emojiCount++;
            }
            catch { }
        }
        return emojiReport;
    }

    public record RolesReport
    {
        public int successfullyRestoredCount { get; set; }
    }

    internal static async Task<RolesReport> RestoreRolesAsync(this Database.Models.BackupModels.Backup backup, RestGuild guild, bool clearRoles = false)
    {
        Discord.Rest.RestRole? highestGuildRole = guild.Roles.MaxBy(x => x.Position);
        var rolesReport = new RolesReport();
        if (clearRoles)
            foreach (var role in guild.Roles)
            {
                try
                {
                    if (highestGuildRole == role)
                        continue;
                    if (role.IsManaged is false && role.IsEveryone is false)
                        await role.DeleteAsync();
                }
                catch { }
            }

        if (backup.roles is not null)
        {
            var roles = backup.roles.OrderByDescending(x => x.position).ToList();
            foreach (Role role in roles)
            {
                if (role.isManaged)
                    continue;
                if (role.isEveryone)
                {
                    await guild.EveryoneRole.ModifyAsync(x =>
                    {
                        x.Position = role.position;
                        x.Mentionable = role.isMentionable;
                        x.Hoist = role.isHoisted;
                        x.Permissions = ReturnRolePermissions(role);
                        x.Color = new Color(role.color);
                    });
                    if (backup.catgeoryChannels is not null)
                    foreach (CategoryChannel? channel in backup.catgeoryChannels)
                    {
                        foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
                        {
                            if (perm.targetId == role.id)
                                perm.targetId = guild.EveryoneRole.Id;
                        }
                    }
                    if (backup.textChannels is not null)
                        foreach (TextChannel? channel in backup.textChannels)
                    {
                        foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
                        {
                            if (perm.targetId == role.id)
                                perm.targetId = guild.EveryoneRole.Id;
                        }
                    }
                    if (backup.voiceChannels is not null)
                        foreach (VoiceChannel? channel in backup.voiceChannels)
                    {
                        foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
                        {
                            if (perm.targetId == role.id)
                                perm.targetId = guild.EveryoneRole.Id;
                        }
                    }
                    role.id = guild.EveryoneRole.Id;
                    continue;
                }
                GuildPermissions perms = ReturnRolePermissions(role);
                Discord.Rest.RestRole? newRole = await guild.CreateRoleAsync(role.name, perms, new Color(role.color), role.isHoisted, role.isMentionable);
                if (backup.catgeoryChannels is not null)
                    foreach (CategoryChannel? channel in backup.catgeoryChannels)
                {
                    foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
                    {
                        if (perm.targetId == role.id)
                            perm.targetId = newRole.Id;
                    }
                }
                if (backup.textChannels is not null)
                    foreach (TextChannel? channel in backup.textChannels)
                {
                    foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
                    {
                        if (perm.targetId == role.id)
                            perm.targetId = newRole.Id;
                    }
                }
                if (backup.voiceChannels is not null)
                    foreach (VoiceChannel? channel in backup.voiceChannels)
                {
                    foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
                    {
                        if (perm.targetId == role.id)
                            perm.targetId = newRole.Id;
                    }
                }

                role.id = newRole.Id;
            }
        }
        return rolesReport;
    }

    public record UserRoleReport
    {
        public int successfullyRestoredCount { get; set; }
    }

    internal static async Task<UserRoleReport?> RestoreUserRolesAsync(this Database.Models.BackupModels.Backup backup, RestGuild guild, bool clearUserRoles = false)
    {
        var userRoleReport = new UserRoleReport();
        if (backup.users is null)
        {
            return null;
        }
        if (clearUserRoles)
        {

        }
        foreach (GuildUser? user in backup.users)
        {
            var guildUser = await guild.GetUserAsync(user.id);
            if (guildUser is null)
                continue;
            List<ulong> roleIds = new();
            foreach (var role in user.assignedRoles)
            {
                if (role.isEveryone || role.isManaged)
                    continue;
                roleIds.Add(role.id);
            }
            await guildUser.AddRolesAsync(roleIds);
            roleIds.Clear();
        }
        return userRoleReport;
    }

    public record ChannelReport
    {

    }

    internal static async Task<ChannelReport> RestoreChannelsAsync(this Database.Models.BackupModels.Backup backup, RestGuild guild, bool updateChannels = false, bool clearChannels = false)
    {
        var channelReport = new ChannelReport();
        if (clearChannels)
        {
            var channels = await guild.GetChannelsAsync();
            foreach (var channel in channels)
            {
                try
                {
                    await channel.DeleteAsync();
                }
                catch { }
            }
        }
        if (backup.catgeoryChannels is not null)
            foreach (CategoryChannel channel in backup.catgeoryChannels)
            {
                Discord.Rest.RestCategoryChannel? newChannel = await guild.CreateCategoryChannelAsync(channel.name, x =>
                {
                    x.Name = channel.name;
                    x.Position = channel.position;
                    var permissions = new List<Overwrite>();
                    foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
                    {
                        permissions.Add(ReturnChannelPermissions(perm));
                    }
                    x.PermissionOverwrites = permissions;
                });
                if (updateChannels)
                    channel.id = newChannel.Id;
            }
        if (backup.textChannels is not null)
            foreach (TextChannel? channel in backup.textChannels)
            {
                Discord.Rest.RestTextChannel? newChannel = await guild.CreateTextChannelAsync(channel.name, x =>
                {
                    x.Name = channel.name;
                    if (channel.topic is not null)
                        x.Topic = channel.topic;
                    //x.Locked = channel.locked;
                    x.IsNsfw = channel.nsfw;
                    x.Position = channel.position;
                    if (channel.category is not null)
                        x.CategoryId = channel.category.id;
                    x.SlowModeInterval = channel.slowModeInterval;
                    //x.Archived = channel.archived;
                    if (channel.archiveAfter is not null)
                        x.AutoArchiveDuration = new Optional<ThreadArchiveDuration>((ThreadArchiveDuration)channel.archiveAfter);
                    var permissions = new List<Overwrite>();
                    foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
                    {
                        permissions.Add(ReturnChannelPermissions(perm));
                    }
                    x.PermissionOverwrites = permissions;
                });
                if (updateChannels)
                    channel.id = newChannel.Id;
            }
        if (backup.voiceChannels is not null)
            foreach (VoiceChannel? channel in backup.voiceChannels)
            {
                Discord.Rest.RestVoiceChannel? newChannel = await guild.CreateVoiceChannelAsync(channel.name, x =>
                {
                    x.Name = channel.name;
                    x.Position = channel.position;
                    x.RTCRegion = channel.region;
                    x.Bitrate = channel.bitrate;
                    x.UserLimit = channel.userLimit;
                    x.Position = channel.position;
                    if (channel.category is not null)
                        x.CategoryId = channel.category.id;
                    var permissions = new List<Overwrite>();
                    foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
                    {
                        permissions.Add(ReturnChannelPermissions(perm));
                    }
                    x.PermissionOverwrites = permissions;
                });
                if (updateChannels)
                    channel.id = newChannel.Id;
            }
        return channelReport;
    }

    public record GuildReport
    {

    }

    internal static async Task<GuildReport?> RestoreGuildSettingsAsync(this Database.Models.BackupModels.Backup backup, RestGuild guild)
    {
        if (backup.guild is null)
            return null;
        var guildReport = new GuildReport();
        using var _httpClient = new HttpClient();
        await guild.ModifyAsync(async x =>
        {
            x.Name = backup.guild.guildName;
            x.Icon = string.IsNullOrWhiteSpace(backup.guild.iconUrl) is false
                ? new Optional<Image?>(new Image(await _httpClient.GetStreamAsync(backup.guild.iconUrl)))
                : (Optional<Image?>)null;
            if (guild.PremiumTier == PremiumTier.Tier1)
                x.Splash = string.IsNullOrWhiteSpace(backup.guild.splashUrl) is false ? new Optional<Image?>(new Image(await _httpClient.GetStreamAsync(backup.guild.splashUrl))) : null;
            if (guild.PremiumTier == PremiumTier.Tier2)
                x.Banner = string.IsNullOrWhiteSpace(backup.guild.bannerUrl) is false ? new Optional<Image?>(new Image(await _httpClient.GetStreamAsync(backup.guild.bannerUrl))) : null;
            if (backup.guild.afkTimeout is not null)
                x.AfkTimeout = new Optional<int>((int)backup.guild.afkTimeout);
            if (backup.guild.afkChannelId is not null)
                x.AfkChannelId = backup.guild.afkChannelId;
            if (backup.guild.systemChannelId is not null)
                x.SystemChannelId = backup.guild.systemChannelId;
            x.ExplicitContentFilter = new Optional<ExplicitContentFilterLevel>((ExplicitContentFilterLevel)backup.guild.explicitContentFilterLevel);
            x.DefaultMessageNotifications = new Optional<DefaultMessageNotifications>((DefaultMessageNotifications)backup.guild.defaultMessageNotifications);
            if (backup.guild.isBoostProgressBarEnabled is not null)
                x.IsBoostProgressBarEnabled = (bool)backup.guild.isBoostProgressBarEnabled;
            x.VerificationLevel = new Optional<VerificationLevel>((VerificationLevel)backup.guild.verificationLevel);
            x.SystemChannelFlags = new Optional<SystemChannelMessageDeny>((SystemChannelMessageDeny)backup.guild.systemChannelMessageDeny);
            x.PreferredLocale = backup.guild.preferredLocale;
        });
        return guildReport;
    }


    //internal static async Task RestoreGuildAsync(Database.Models.BackupModels.Backup server, RestGuild context)
    //{
    //    if (server.backup is null)
    //        return;
    //    await RestoreRolesAsync(server, context, true);
    //    await Task.WhenAll(RestoreUserRoles(server, context,), RestoreChannelsAsync(server, context, true), RestoreGuildSettingsAsync(server, context), RestoreEmojisAsync(server, context));
    //}

    private static GuildPermissions ReturnRolePermissions(Role role)
    {
        return GuildPermissions.None.Modify(
         role.permissions.CreateInstantInvite,
         role.permissions.KickMembers,
         role.permissions.BanMembers,
         role.permissions.Administrator,
         role.permissions.ManageChannels,
         role.permissions.ManageGuild,
         role.permissions.AddReactions,
         role.permissions.ViewAuditLog,
         role.permissions.ViewGuildInsights,
         role.permissions.ViewChannel,
         role.permissions.SendMessages,
         role.permissions.SendTTSMessages,
         role.permissions.ManageMessages,
         role.permissions.EmbedLinks,
         role.permissions.AttachFiles,
         role.permissions.ReadMessageHistory,
         role.permissions.MentionEveryone,
         role.permissions.UseExternalEmojis,
         role.permissions.Connect,
         role.permissions.Speak,
         role.permissions.MuteMembers,
         role.permissions.DeafenMembers,
         role.permissions.MoveMembers,
         role.permissions.useVoiceActivation,
         role.permissions.PrioritySpeaker,
         role.permissions.Stream,
         role.permissions.ChangeNickname,
         role.permissions.ManageNicknames,
         role.permissions.ManageRoles,
         role.permissions.ManageWebhooks,
         role.permissions.ManageEmojisAndStickers,
         role.permissions.UseApplicationCommands,
         role.permissions.RequestToSpeak,
         role.permissions.ManageEvents,
         role.permissions.ManageThreads,
         role.permissions.CreatePublicThreads,
         role.permissions.CreatePrivateThreads,
         role.permissions.UseExternalStickers,
         role.permissions.SendMessagesInThreads,
         role.permissions.StartEmbeddedActivities,
         role.permissions.moderateMembers);
    }
    private static Overwrite ReturnChannelPermissions(Database.Models.BackupModels.Permissions.ChannelPermissions perm)
    {
        return new Overwrite(perm.targetId, (PermissionTarget)perm.permissionTarget,
        new OverwritePermissions().Modify(
        (PermValue)perm.CreateInstantInvite,
        (PermValue)perm.ManageChannel,
        (PermValue)perm.AddReactions,
        (PermValue)perm.ViewChannel,
        (PermValue)perm.SendMessages,
        (PermValue)perm.SendTTSMessages,
        (PermValue)perm.ManageMessages,
        (PermValue)perm.EmbedLinks,
        (PermValue)perm.AttachFiles,
        (PermValue)perm.ReadMessageHistory,
        (PermValue)perm.MentionEveryone,
        (PermValue)perm.UseExternalEmojis,
        (PermValue)perm.Connect,
        (PermValue)perm.Speak,
        (PermValue)perm.MuteMembers,
        (PermValue)perm.DeafenMembers,
        (PermValue)perm.MoveMembers,
        (PermValue)perm.useVoiceActivation,
        (PermValue)perm.ManageRoles,
        (PermValue)perm.ManageWebhooks,
        (PermValue)perm.PrioritySpeaker,
        (PermValue)perm.Stream,
        (PermValue)perm.useSlashCommands,
        (PermValue)perm.UseApplicationCommands,
        (PermValue)perm.RequestToSpeak,
        (PermValue)perm.ManageThreads,
        (PermValue)perm.CreatePublicThreads,
        (PermValue)perm.CreatePrivateThreads,
        (PermValue)perm.UseExternalStickers,
        (PermValue)perm.usePrivateThreads,
        (PermValue)perm.UseExternalStickers,
        (PermValue)perm.SendMessagesInThreads,
        (PermValue)perm.StartEmbeddedActivities
        ));
    }
    //public static async Task MerkRestoreEmbed(SocketTextChannel channel)
    //{
    //    IAsyncEnumerable<IReadOnlyCollection<IMessage>>? messages = channel.GetMessagesAsync(50);
    //    await messages.ForEachAsync(message =>
    //    {
    //        foreach (IMessage? message2 in message)
    //        {
    //            if (message2.Components.Count == 1 && string.IsNullOrWhiteSpace(message2.Content) && message2.Embeds.Count == 1 && message2.Author.IsBot && message2.Type == MessageType.ApplicationCommand)
    //            {
    //                message2.DeleteAsync();
    //                return;
    //            }
    //        }
    //    });
    //}
    //public static async Task SendRestoreConfirmationMessage(ShardedInteractionContext context)
    //{
    //    MessageComponent? components = new ComponentBuilder()
    //    {
    //        ActionRows = new List<ActionRowBuilder>()
    //        {
    //            new ActionRowBuilder()
    //            {
    //                Components = new List<IMessageComponent>
    //                {
    //                    new ButtonBuilder()
    //                    {
    //                        CustomId = "confirm-restore-button",
    //                        Style = ButtonStyle.Success,
    //                        Label = "Confirm Restore",
    //                    }.Build(),
    //                    new ButtonBuilder()
    //                    {
    //                        CustomId = "cancel-restore-button",
    //                        Style = ButtonStyle.Danger,
    //                        Label = "Cancel Restore"
    //                    }.Build(),
    //                }
    //            }
    //        }
    //    }.Build();
    //    Embed? embed = new EmbedBuilder()
    //    {
    //        Title = $"Restore Backup for {context.Guild.Name}",
    //        Color = Miscallenous.RandomDiscordColour(),
    //        Author = new EmbedAuthorBuilder
    //        {
    //            Url = "https://discord.repair",
    //            Name = "Discord.Repair",
    //            IconUrl = "https://i.imgur.com/Nfy4OoG.png"
    //        },
    //        Footer = new EmbedFooterBuilder
    //        {
    //            Text = $"Issued by: {context.User.Username} | {context.User.Id}",
    //            IconUrl = context.User.GetAvatarUrl()
    //        },
    //        Description = "Do you want to load the Backup?\n**This will delete ALL ROLES, CHANNELS AND CATEGORIES**",
    //    }.WithCurrentTimestamp().Build();
    //    await context.Interaction.RespondAsync(embed: embed, components: components);
    //}
}
