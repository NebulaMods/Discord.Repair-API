namespace DiscordRepair.Api.MigrationMaster;

/// <summary>
/// 
/// </summary>
public class Restore
{
    private readonly Configuration _configuration;

    public Restore(Configuration configuration)
    {
        _configuration = configuration;
    }

    //public async Task DeleteGuildBackupAsync(Server server, DatabaseContext database)
    //{
    //    if (server.backup is null)
    //        return;
    //    //server.backup.rulesChannel = null;
    //    //server.backup.publicUpdatesChannel = null;
    //    //server.backup.systemChannel = null;
    //    //server.backup.widgetChannel = null;
    //    //await database.ApplyChangesAsync(server);
    //    foreach (TextChannel? channel in server.backup.textChannels)
    //    {
    //        foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? permission in channel.permissions)
    //        {
    //            channel.permissions.Remove(permission);
    //            database.Remove(permission);
    //        }
    //        await DeleteTextChannelAsync(server.backup, database, channel);
    //    }
    //    foreach (VoiceChannel? channel in server.backup.voiceChannels)
    //    {
    //        foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? permission in channel.permissions)
    //        {
    //            channel.permissions.Remove(permission);
    //            database.Remove(permission);
    //        }
    //        await DeleteVoiceChannelAsync(server.backup, database, channel);
    //    }
    //    foreach (CategoryChannel? channel in server.backup.catgeoryChannels)
    //    {
    //        foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? permission in channel.permissions)
    //        {
    //            channel.permissions.Remove(permission);
    //            database.Remove(permission);
    //        }
    //        await DeleteCategroyChannelAsync(server.backup, database, channel);
    //    }
    //    foreach (GuildUser? guilduser in server.backup.users)
    //    {
    //        foreach (GuildUserRole? role in guilduser.assignedRoles)
    //        {
    //            guilduser.assignedRoles.Remove(role);
    //            database.Remove(role);
    //        }
    //        server.backup.users.Remove(guilduser);
    //        database.Remove(guilduser);
    //    }
    //    foreach (Role? role in server.backup.roles)
    //    {
    //        DeleteRole(server.backup, database, role);
    //    }
    //    foreach (Database.Models.BackupModels.Emoji? emoji in server.backup.emojis)
    //    {
    //        server.backup.emojis.Remove(emoji);
    //        database.Remove(emoji);
    //    }
    //    //messages
    //    database.Remove(server.backup);
    //}
    //private async Task RestoreEmotes(Server server, ShardedInteractionContext context, bool clearEmotes = false)
    //{
    //    int emojiCount = 0;
    //    if (clearEmotes)
    //    {
    //        var emotes = await context.Guild.GetEmotesAsync();
    //        foreach (var emote in emotes)
    //        {
    //            try
    //            {
    //                await context.Guild.DeleteEmoteAsync(emote);
    //                await Task.Delay(TimeSpan.FromSeconds(2));
    //            }
    //            catch { }
    //        }
    //    }
    //    using var _httpClient = new HttpClient();
    //    var emojis = server.backup.emojis.ToList();
    //    foreach (var emoji in emojis)
    //    {
    //        try
    //        {
    //            if (emojiCount == 50)
    //            {
    //                emojiCount = 0;
    //                await Task.Delay(TimeSpan.FromHours(1));
    //            }
    //            _ = context.Guild.CreateEmoteAsync(emoji.name, new Image(await _httpClient.GetStreamAsync(emoji.url)));
    //            emojiCount++;
    //            emojis.Remove(emoji);
    //            await Task.Delay(TimeSpan.FromSeconds(1));
    //        }
    //        catch { }
    //    }
    //    emojis.Clear();
    //}
    //private async Task RestoreRoles(Server server, ShardedInteractionContext context, bool clearRoles = false)
    //{
    //    Discord.Rest.RestGuild? restGuild = await context.Client.Rest.GetGuildAsync(context.Guild.Id);
    //    Discord.Rest.RestRole? highestGuildRole = restGuild.Roles.MaxBy(x => x.Position);

    //    if (clearRoles)
    //        foreach(var role in restGuild.Roles)
    //    {
    //        try
    //        {
    //            if (highestGuildRole == role)
    //                continue;
    //            if (role.IsManaged is false && role.IsEveryone is false)
    //                await role.DeleteAsync();
    //        }
    //        catch { }
    //    }

    //    if (server.backup.roles.Any())
    //    {
    //        foreach (Role role in server.backup.roles.OrderByDescending(x => x.position).ToList())
    //        {
    //            if (role.isManaged)
    //                continue;
    //            if (role.isEveryone)
    //            {
    //                await context.Guild.EveryoneRole.ModifyAsync(x =>
    //                {
    //                    x.Position = role.position;
    //                    x.Mentionable = role.isMentionable;
    //                    x.Hoist = role.isHoisted;
    //                    x.Permissions = ReturnRolePermissions(role);
    //                    x.Color = new Color(role.color);
    //                });
    //                foreach (CategoryChannel? channel in server.backup.catgeoryChannels)
    //                {
    //                    foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
    //                    {
    //                        if (perm.targetId == role.id)
    //                            perm.targetId = context.Guild.EveryoneRole.Id;
    //                    }
    //                }
    //                foreach (TextChannel? channel in server.backup.textChannels)
    //                {
    //                    foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
    //                    {
    //                        if (perm.targetId == role.id)
    //                            perm.targetId = context.Guild.EveryoneRole.Id;
    //                    }
    //                }
    //                foreach (VoiceChannel? channel in server.backup.voiceChannels)
    //                {
    //                    foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
    //                    {
    //                        if (perm.targetId == role.id)
    //                            perm.targetId = context.Guild.EveryoneRole.Id;
    //                    }
    //                }
    //                if (server.roleId == role.id)
    //                    server.roleId = context.Guild.EveryoneRole.Id;
    //                role.id = context.Guild.EveryoneRole.Id;
    //                continue;
    //            }
    //            GuildPermissions perms = ReturnRolePermissions(role);
    //            Discord.Rest.RestRole? newRole = await context.Guild.CreateRoleAsync(role.name, perms, new Color(role.color), role.isHoisted, role.isMentionable);
    //            foreach (CategoryChannel? channel in server.backup.catgeoryChannels)
    //            {
    //                foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
    //                {
    //                    if (perm.targetId == role.id)
    //                        perm.targetId = newRole.Id;
    //                }
    //            }
    //            foreach (TextChannel? channel in server.backup.textChannels)
    //            {
    //                foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
    //                {
    //                    if (perm.targetId == role.id)
    //                        perm.targetId = newRole.Id;
    //                }
    //            }
    //            foreach (VoiceChannel? channel in server.backup.voiceChannels)
    //            {
    //                foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
    //                {
    //                    if (perm.targetId == role.id)
    //                        perm.targetId = newRole.Id;
    //                }
    //            }
    //            if (server.roleId == role.id)
    //                server.roleId = newRole.Id;
    //            role.id = newRole.Id;
    //        }
    //    }

    //}
    //private async Task RestoreUserRoles(Server server, ShardedInteractionContext context, DatabaseContext database)
    //{
    //    //download members
    //    if (await DiscordExtensions.CheckBusinessMembership(database, context, false))
    //    {
    //        if (server.backup.users.Any())
    //        {
    //            await context.Guild.DownloadUsersAsync();
    //            var users = server.backup.users.ToList();
    //            foreach (GuildUser? user in users)
    //            {
    //                SocketGuildUser? guildUser = context.Guild.GetUser(user.id);
    //                if (guildUser is null)
    //                    continue;
    //                List<ulong> roleIds = new();
    //                foreach (GuildUserRole? role in user.assignedRoles)
    //                {
    //                    if (role.role.isEveryone || role.role.isManaged)
    //                        continue;
    //                    roleIds.Add(role.role.id);
    //                }
    //                await guildUser.AddRolesAsync(roleIds);
    //                roleIds.Clear();
    //            }
    //            users.Clear();
    //            context.Guild.PurgeUserCache();
    //        }
    //    }
    //}
    //private async Task RestoreChannels(Server server, ShardedInteractionContext context, bool clearChannels = false)
    //{
    //    if (clearChannels)
    //        foreach(var channel in context.Guild.Channels) 
    //    {
    //        try
    //        {
    //            if (channel.Id == context.Interaction.Channel.Id)
    //                continue;
    //            await channel.DeleteAsync();
    //        }
    //        catch { }
    //    }
    //    var categoryChannels = server.backup.catgeoryChannels.ToList();
    //    foreach (CategoryChannel channel in categoryChannels)
    //    {
    //        Discord.Rest.RestCategoryChannel? newChannel = await context.Guild.CreateCategoryChannelAsync(channel.name, x =>
    //        {
    //            x.Name = channel.name;
    //            x.Position = channel.position;
    //            var permissions = new List<Overwrite>();
    //            foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
    //            {
    //                permissions.Add(ReturnChannelPermissions(perm));
    //            }
    //            x.PermissionOverwrites = permissions;
    //        });
    //        channel.id = newChannel.Id;
    //    }
    //    var textChannels = server.backup.textChannels.ToList();
    //    foreach (TextChannel? channel in textChannels)
    //    {
    //        Discord.Rest.RestTextChannel? newChannel = await context.Guild.CreateTextChannelAsync(channel.name, x =>
    //        {
    //            x.Name = channel.name;
    //            if (channel.topic is not null)
    //                x.Topic = channel.topic;
    //            x.Locked = channel.locked;
    //            x.IsNsfw = channel.nsfw;
    //            x.Position = channel.position;
    //            if (channel.category is not null)
    //                x.CategoryId = channel.category.id;
    //            x.SlowModeInterval = channel.slowModeInterval;
    //            x.Archived = channel.archived;
    //            if (channel.archiveAfter is not null)
    //                x.AutoArchiveDuration = new Optional<ThreadArchiveDuration>((ThreadArchiveDuration)channel.archiveAfter);
    //            var permissions = new List<Overwrite>();
    //            foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
    //            {
    //                permissions.Add(ReturnChannelPermissions(perm));
    //            }
    //            x.PermissionOverwrites = permissions;
    //        });
    //        channel.id = newChannel.Id;
    //    }
    //    var voiceChannels = server.backup.voiceChannels.ToList();
    //    foreach (VoiceChannel? channel in voiceChannels)
    //    {
    //        Discord.Rest.RestVoiceChannel? newChannel = await context.Guild.CreateVoiceChannelAsync(channel.name, x =>
    //        {
    //            x.Name = channel.name;
    //            x.Position = channel.position;
    //            x.RTCRegion = channel.region;
    //            x.Bitrate = channel.bitrate;
    //            x.UserLimit = channel.userLimit;
    //            x.Position = channel.position;
    //            if (channel.category is not null)
    //                x.CategoryId = channel.category.id;
    //            var permissions = new List<Overwrite>();
    //            foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
    //            {
    //                permissions.Add(ReturnChannelPermissions(perm));
    //            }
    //            x.PermissionOverwrites = permissions;
    //        });
    //        channel.id = newChannel.Id;
    //    }
    //    voiceChannels.Clear();
    //    textChannels.Clear();
    //    categoryChannels.Clear();
    //}
    //private async Task RestoreGuildSettings(Server server, ShardedInteractionContext context)
    //{
    //    try
    //    {
    //        using var _httpClient = new HttpClient();
    //        await context.Guild.ModifyAsync(async x =>
    //        {
    //            x.Name = server.backup.guildName;
    //            x.Icon = string.IsNullOrWhiteSpace(server.backup.iconUrl) is false
    //                ? new Optional<Image?>(new Image(await _httpClient.GetStreamAsync(server.backup.iconUrl)))
    //                : (Optional<Image?>)null;
    //            if (context.Guild.PremiumTier == PremiumTier.Tier1)
    //                x.Splash = string.IsNullOrWhiteSpace(server.backup.splashUrl) is false ? new Optional<Image?>(new Image(await _httpClient.GetStreamAsync(server.backup.splashUrl))) : null;
    //            if (context.Guild.PremiumTier == PremiumTier.Tier2)
    //                x.Banner = string.IsNullOrWhiteSpace(server.backup.bannerUrl) is false ? new Optional<Image?>(new Image(await _httpClient.GetStreamAsync(server.backup.bannerUrl))) : null;
    //            if (server.backup.afkTimeout is not null)
    //                x.AfkTimeout = new Optional<int>((int)server.backup.afkTimeout);
    //            if (server.backup.afkChannel is not null)
    //                x.AfkChannelId = server.backup.afkChannel.id;
    //            if (server.backup.systemChannel is not null)
    //                x.SystemChannelId = server.backup.systemChannel.id;
    //            x.ExplicitContentFilter = new Optional<ExplicitContentFilterLevel>((ExplicitContentFilterLevel)server.backup.explicitContentFilterLevel);
    //            x.DefaultMessageNotifications = new Optional<DefaultMessageNotifications>((DefaultMessageNotifications)server.backup.defaultMessageNotifications);
    //            if (server.backup.isBoostProgressBarEnabled is not null)
    //                x.IsBoostProgressBarEnabled = (bool)server.backup.isBoostProgressBarEnabled;
    //            x.VerificationLevel = new Optional<VerificationLevel>((VerificationLevel)server.backup.verificationLevel);
    //            x.SystemChannelFlags = new Optional<SystemChannelMessageDeny>((SystemChannelMessageDeny)server.backup.systemChannelMessageDeny);
    //            x.PreferredLocale = server.backup.preferredLocale;
    //        });
    //    }
    //    catch(Exception ex)
    //    {
    //        Console.WriteLine(ex.Message);
    //    }
    //}
    //public async Task RestoreGuildAsync(Server server, ShardedInteractionContext context, DatabaseContext database)
    //{
    //    if (server.backup is null)
    //        return;
    //    await RestoreRoles(server, context, true);
    //    await Task.WhenAll(RestoreUserRoles(server, context, database), RestoreChannels(server, context, true), RestoreGuildSettings(server, context), RestoreEmotes(server, context));
    //}
    //private async ValueTask<bool> DeleteCategroyChannelAsync(Database.Models.BackupModels.Backup backup, DatabaseContext database, CategoryChannel channel)
    //{
    //    CategoryChannel? channelEntry = backup.catgeoryChannels.FirstOrDefault(x => x.id == channel.id);
    //    if (channelEntry is null)
    //        return false;
    //    await channelEntry.permissions.ToAsyncEnumerable().ForEachAsync(x => channelEntry.permissions.Remove(x));
    //    database.Remove(channelEntry);
    //    backup.catgeoryChannels.Remove(channelEntry);
    //    return true;
    //}
    //private async ValueTask<bool> DeleteTextChannelAsync(Database.Models.BackupModels.Backup backup, DatabaseContext database, TextChannel channel)
    //{
    //    TextChannel? channelEntry = backup.textChannels.FirstOrDefault(x => x.id == channel.id);
    //    if (channelEntry is null)
    //        return false;
    //    await channelEntry.permissions.ToAsyncEnumerable().ForEachAsync(x => channelEntry.permissions.Remove(x));
    //    backup.textChannels.Remove(channelEntry);
    //    database.Remove(channelEntry);
    //    return true;
    //}
    //private async ValueTask<bool> DeleteVoiceChannelAsync(Database.Models.BackupModels.Backup backup, DatabaseContext database, VoiceChannel channel)
    //{
    //    VoiceChannel? channelEntry = backup.voiceChannels.FirstOrDefault(x => x.id == channel.id);
    //    if (channelEntry is null)
    //        return false;
    //    await channelEntry.permissions.ToAsyncEnumerable().ForEachAsync(x => channelEntry.permissions.Remove(x));
    //    backup.voiceChannels.Remove(channelEntry);
    //    database.Remove(channelEntry);
    //    return true;
    //}
    //private bool DeleteRole(Database.Models.BackupModels.Backup backup, DatabaseContext database, Role role)
    //{
    //    Role? roleEntry = backup.roles.FirstOrDefault(x => x.id == role.id);
    //    if (roleEntry is null)
    //        return false;
    //    database.Remove(roleEntry.permissions);
    //    database.Remove(roleEntry);
    //    return true;
    //}
    //private  GuildPermissions ReturnRolePermissions(Role role)
    //{
    //    return GuildPermissions.None.Modify(
    //     role.permissions.CreateInstantInvite,
    //     role.permissions.KickMembers,
    //     role.permissions.BanMembers,
    //     role.permissions.Administrator,
    //     role.permissions.ManageChannels,
    //     role.permissions.ManageGuild,
    //     role.permissions.AddReactions,
    //     role.permissions.ViewAuditLog,
    //     role.permissions.ViewGuildInsights,
    //     role.permissions.ViewChannel,
    //     role.permissions.SendMessages,
    //     role.permissions.SendTTSMessages,
    //     role.permissions.ManageMessages,
    //     role.permissions.EmbedLinks,
    //     role.permissions.AttachFiles,
    //     role.permissions.ReadMessageHistory,
    //     role.permissions.MentionEveryone,
    //     role.permissions.UseExternalEmojis,
    //     role.permissions.Connect,
    //     role.permissions.Speak,
    //     role.permissions.MuteMembers,
    //     role.permissions.DeafenMembers,
    //     role.permissions.MoveMembers,
    //     role.permissions.useVoiceActivation,
    //     role.permissions.PrioritySpeaker,
    //     role.permissions.Stream,
    //     role.permissions.ChangeNickname,
    //     role.permissions.ManageNicknames,
    //     role.permissions.ManageRoles,
    //     role.permissions.ManageWebhooks,
    //     role.permissions.ManageEmojisAndStickers,
    //     role.permissions.UseApplicationCommands,
    //     role.permissions.RequestToSpeak,
    //     role.permissions.ManageEvents,
    //     role.permissions.ManageThreads,
    //     role.permissions.CreatePublicThreads,
    //     role.permissions.CreatePrivateThreads,
    //     role.permissions.UseExternalStickers,
    //     role.permissions.SendMessagesInThreads,
    //     role.permissions.StartEmbeddedActivities,
    //     role.permissions.moderateMembers);
    //}
    //private  Overwrite ReturnChannelPermissions(Database.Models.BackupModels.Permissions.ChannelPermissions perm)
    //{
    //    return new Overwrite(perm.targetId, (PermissionTarget)perm.permissionTarget,
    //    new OverwritePermissions().Modify(
    //    (PermValue)perm.CreateInstantInvite,
    //    (PermValue)perm.ManageChannel,
    //    (PermValue)perm.AddReactions,
    //    (PermValue)perm.ViewChannel,
    //    (PermValue)perm.SendMessages,
    //    (PermValue)perm.SendTTSMessages,
    //    (PermValue)perm.ManageMessages,
    //    (PermValue)perm.EmbedLinks,
    //    (PermValue)perm.AttachFiles,
    //    (PermValue)perm.ReadMessageHistory,
    //    (PermValue)perm.MentionEveryone,
    //    (PermValue)perm.UseExternalEmojis,
    //    (PermValue)perm.Connect,
    //    (PermValue)perm.Speak,
    //    (PermValue)perm.MuteMembers,
    //    (PermValue)perm.DeafenMembers,
    //    (PermValue)perm.MoveMembers,
    //    (PermValue)perm.useVoiceActivation,
    //    (PermValue)perm.ManageRoles,
    //    (PermValue)perm.ManageWebhooks,
    //    (PermValue)perm.PrioritySpeaker,
    //    (PermValue)perm.Stream,
    //    (PermValue)perm.useSlashCommands,
    //    (PermValue)perm.UseApplicationCommands,
    //    (PermValue)perm.RequestToSpeak,
    //    (PermValue)perm.ManageThreads,
    //    (PermValue)perm.CreatePublicThreads,
    //    (PermValue)perm.CreatePrivateThreads,
    //    (PermValue)perm.UseExternalStickers,
    //    (PermValue)perm.usePrivateThreads,
    //    (PermValue)perm.UseExternalStickers,
    //    (PermValue)perm.SendMessagesInThreads,
    //    (PermValue)perm.StartEmbeddedActivities
    //    ));
    //}
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
