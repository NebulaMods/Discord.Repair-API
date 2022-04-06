using System.Collections.Concurrent;
using System.Net;
using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestoreCord.Database;
using RestoreCord.Database.Models;
using RestoreCord.Database.Models.BackupModels;
using RestoreCord.Database.Models.BackupModels.Channel;
using RestoreCord.Database.Models.BackupModels.Permissions;
using RestoreCord.Records.Discord;

namespace RestoreCord.Utilities;
public class OldMigration
{
    public record MemberStatisitics
    {
        public DateTime startTime { get; set; }
        public TimeSpan totalTime { get; set; }
        public int successCount { get; set; }
        public int bannedCount { get; set; }
        public int tooManyGuildsCount { get; set; }
        public int invalidTokenCount { get; set; }
        public int alreadyHereCount { get; set; }
        public int failedCount { get; set; }
        public int totalCount { get; set; }
    }
    public record GuildStatisitics
    {
        public DateTime startTime { get; set; }
        public TimeSpan totalTime { get; set; }
    }
    public ConcurrentDictionary<ulong, MemberStatisitics> ActiveMemberMigrations { get; } = new();
    public ConcurrentDictionary<ulong, GuildStatisitics> ActiveGuildMigrations { get; } = new();
    private readonly string _token = Properties.Resources.Token;
    private readonly string _clientId = Properties.Resources.ClientID;
    private readonly string _clientSecret = Properties.Resources.ClientSecret;
    public OldMigration()
    {

    }

    internal void HttpClientSetup(HttpClient http, string? token = null, string tokenType = "Bot")
    {
        if (http.DefaultRequestHeaders.Contains("User-Agent"))
            http.DefaultRequestHeaders.Remove("User-Agent");
        if (http.DefaultRequestHeaders.Contains("X-RateLimit-Precision"))
            http.DefaultRequestHeaders.Remove("X-RateLimit-Precision");
        if (http.DefaultRequestHeaders.Contains("Authorization"))
            http.DefaultRequestHeaders.Remove("Authorization");
        http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(tokenType, token is null ? _token : token);
        http.DefaultRequestHeaders.TryAddWithoutValidation("X-RateLimit-Precision", "millisecond");
        http.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", $"{Assembly.GetExecutingAssembly().FullName} (public release, {Assembly.GetExecutingAssembly().ImageRuntimeVersion})");
    }

    internal static async ValueTask<bool> JoinUsersToGuild(DatabaseContext database, Server server, HttpClient http, MemberStatisitics statisitics, OldMigration migration, DiscordShardedClient client, ulong guildId)
    {
        SocketGuild? guildSocket = client.GetGuild(guildId);
        Task? task1 = guildSocket.DownloadUsersAsync();
        //download banlist & create a task
        Task<IReadOnlyCollection<Discord.Rest.RestBan>>? task2 = guildSocket.GetBansAsync();
        //grab users from database with specific guild id
        List<Member>? databaseMembers = await database.members.Where(x => x.server == server.guildid && x.access_token != "broken").ToListAsync();
        //embed update saying wassup about how many its migrating
        DateTimeOffset estimatedCompletionTime = DateTime.Now.AddSeconds(databaseMembers.Count * 1.2);
        statisitics.totalCount = databaseMembers.Count;
        //wait for tasks to be done
        await Task.WhenAll(task1, task2);
        //grab blacklisted members
        List<Blacklist>? blacklistMembers = await database.blacklist.Where(x => x.server == server.guildid).ToListAsync();
        //grab the users in the guild, need better way of doing this async
        var guildMembers = guildSocket.Users.ToList();
        //asign var to task
        var bannedMembers = (await task2).ToList();
        //start migrating each member in the list
        foreach (Member? member in databaseMembers)
        {
            //check if blacklist contains any members
            if (blacklistMembers.FirstOrDefault(x => x.userid == member.userid) is not null)
            {
                statisitics.bannedCount++;
                continue;
            }
            //check if the member is already in the guild
            if (guildMembers.FirstOrDefault(x => x.Id == member.userid) is not null)
            {
                statisitics.alreadyHereCount++;
                continue;
            }
            //check banlist contains any members
            if (bannedMembers.FirstOrDefault(x => x.User.Id == member.userid) is not null)
            {
                statisitics.bannedCount++;
                continue;
            }

            //try to join the user to the guild
            ResponseTypes addUserRequest = await AddUserFunction(member, server, database, http, migration);
            switch (addUserRequest)
            {
                case ResponseTypes.Success:
                    statisitics.successCount++;
                    break;
                case ResponseTypes.MissingPermissions:
                    return false;
                case ResponseTypes.Banned:
                    statisitics.bannedCount++;
                    break;
                case ResponseTypes.TooManyGuilds:
                    statisitics.tooManyGuildsCount++;
                    break;
                case ResponseTypes.InvalidAuthToken:
                    statisitics.invalidTokenCount++;
                    break;
                default:
                    statisitics.failedCount++;
                    break;
            }
            //sleep for 0.5 sec in betwen each attempt
            await Task.Delay(TimeSpan.FromMilliseconds(30));
        }
        statisitics.totalTime = DateTime.Now - statisitics.startTime;

        //clear resources
        databaseMembers.Clear();
        guildMembers.Clear();
        bannedMembers.Clear();
        guildSocket.PurgeUserCache();
        return true;
    }
    internal static async ValueTask<ResponseTypes> AddUserFunction(Member member, Server server, DatabaseContext database, HttpClient http, OldMigration migration)
    {
        //var handler = new HttpClientHandler()
        //{
        //    Proxy = new ProxyGenerator(_provider),
        //    PreAuthenticate = true,
        //    UseDefaultCredentials = false,
        //    UseProxy = true
        //};
        ResponseTypes addUserRequest = await migration.AddUserToGuildViaHttp(member, server, http);
        switch (addUserRequest)
        {
            case ResponseTypes.InvalidAuthToken:
                (bool, string?) refreshTokenRequest = await migration.RefreshUserToken(member, database, http);
                if (refreshTokenRequest.Item1)
                {
                    member.access_token = refreshTokenRequest.Item2;
                    //
                    return await migration.AddUserToGuildViaHttp(member, server, http);
                    //goto case ResponseTypes.GenericErrorRetryAttempt;
                }
                return addUserRequest;
            default:
                return addUserRequest;
        }
    }

    internal async ValueTask<(bool, string?)> RefreshUserToken(Member member, DatabaseContext database, HttpClient http)
    {
        HttpResponseMessage? response = await RefreshTokenRequest(member, http);
        return response is null
            ? (false, null)
            : await HandleRefreshTokenRequest(member, database, http, response);
    }

    private async ValueTask<(bool, string?)> HandleRefreshTokenRequest(Member member, DatabaseContext database, HttpClient http, HttpResponseMessage response)
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                string? newToken = await UpdateUserEntries(member, database, response);
                if (newToken is not null)
                    return (true, newToken);
                return (false, null);
            case HttpStatusCode.TooManyRequests:
                System.Net.Http.Headers.RetryConditionHeaderValue? headervalue = response.Headers.RetryAfter;
                if (headervalue is not null)
                {
                    if (headervalue.Delta.HasValue)
                        await Task.Delay(TimeSpan.FromMilliseconds(headervalue.Delta.Value.TotalSeconds));
                    else
                        await Task.Delay(TimeSpan.FromSeconds(10));
                    HttpResponseMessage? newRequest = await RefreshTokenRequest(member, http);
                    if (newRequest is not null)
                    {
                        if (newRequest.StatusCode == HttpStatusCode.OK)
                            await HandleRefreshTokenRequest(member, database, http, newRequest);
                    }
                }
                return (false, null);
            case HttpStatusCode.BadRequest:
                string? discordResponse = await response.Content.ReadAsStringAsync();
                if (discordResponse.Contains("\"error\": \"invalid_grant\""))
                {
                    await database.BatchUpdate<Member>()
                    .Set(x => x.access_token, x => "broken")
                    .Where(x => x.userid == member.userid)
                    .ExecuteAsync();
                }
                return (false, null);
            default:
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.RequestMessage?.Content?.ReadAsStringAsync());
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                return (false, null);
        }
    }

    private static async ValueTask<string?> UpdateUserEntries(Member member, DatabaseContext database, HttpResponseMessage response)
    {
        TokenResponse? result = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());
        if (result is null)
            return null;
        if (result.access_token is null || result.refresh_token is null)
            return null;
        try
        {
            await database.BatchUpdate<Member>()
            .Set(x => x.access_token, x => result.access_token)
            .Set(x => x.refresh_token, x => result.refresh_token)
            .Where(x => x.userid == member.userid)
            .ExecuteAsync();
        }
        catch(Exception e)
        {
            await e.LogErrorAsync($"user info backed up incase of corrupt saving: {member.userid} | {member.access_token} | {member.refresh_token} |NEW|REFRESH|MAYBE| {result.refresh_token}", true);
            throw;
        }
        return result.access_token;
    }

    private async ValueTask<HttpResponseMessage?> RefreshTokenRequest(Member member, HttpClient http)
    {
        if (string.IsNullOrWhiteSpace(member.refresh_token))
        {
            return null;
        }
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _clientId,
            ["client_secret"] = _clientSecret,
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = member.refresh_token,
        });
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
        return await http.PostAsync("https://discordapp.com/api/oauth2/token", content);
    }

    
    
    internal async ValueTask<ResponseTypes> AddUserToGuildViaHttp(Member user, Server server, HttpClient http)
    {
        if (string.IsNullOrWhiteSpace(user.access_token) && string.IsNullOrWhiteSpace(user.refresh_token) is false)
            return ResponseTypes.InvalidAuthToken;
        HttpResponseMessage? response = await AddUserToGuildRequest(user, server, http);
        return await HandleGuildRequestCode(user, server, http, response);
    }

    private async ValueTask<ResponseTypes> HandleGuildRequestCode(Member user, Server server, HttpClient http, HttpResponseMessage response)
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.Created:
            case HttpStatusCode.NoContent:
            case HttpStatusCode.OK:
                return ResponseTypes.Success;
            case HttpStatusCode.TooManyRequests:
                System.Net.Http.Headers.RetryConditionHeaderValue? headervalue = response.Headers.RetryAfter;
                if (headervalue is not null)
                {
                    if (headervalue.Delta.HasValue)
                        await Task.Delay(TimeSpan.FromMilliseconds(headervalue.Delta.Value.TotalSeconds));
                    else
                        await Task.Delay(TimeSpan.FromSeconds(10));
                    HttpResponseMessage? newRequest = await AddUserToGuildRequest(user, server, http);
                    if (newRequest is not null)
                    {
                        return newRequest.IsSuccessStatusCode is false
                            ? ResponseTypes.GenericError
                            : await HandleGuildRequestCode(user, server, http, newRequest);
                    }
                }
                return ResponseTypes.TooManyRequests;
            default:
                ErrorResponse? discordResponse;
                try { discordResponse = JsonConvert.DeserializeObject<ErrorResponse>(await response.Content.ReadAsStringAsync()); }
                catch(Exception e)
                {
                    await e.LogErrorAsync(await response.Content.ReadAsStringAsync());
                    return ResponseTypes.NewJsonError;
                }
                if (discordResponse is not null)
                {
                    switch (discordResponse.code)
                    {
                        case 50013:
                            return ResponseTypes.MissingPermissions;
                        case 40007:
                            return ResponseTypes.Banned;
                        case 50025:
                            return ResponseTypes.InvalidAuthToken;
                        case 50027:
                            return ResponseTypes.GenericError;
                        case 30001:
                            return ResponseTypes.TooManyGuilds;
                        default:
                            Console.WriteLine(discordResponse.message + discordResponse.code);
                            return ResponseTypes.GenericError;
                    }
                }
                return ResponseTypes.GenericError;
        }
    }

    private static async ValueTask<HttpResponseMessage> AddUserToGuildRequest(Member user, Server server, HttpClient http)
    {
        string? data = server.roleid switch
        {
            null => JsonConvert.SerializeObject(new { user.access_token }),
            _ => JsonConvert.SerializeObject(new { user.access_token, roles = new ulong[] { (ulong)server.roleid }, })
        };
        var content = new StringContent(data);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        return await http.PutAsync($"https://discord.com/api/guilds/{server.guildid}/members/{user.userid}", content);
    }


    internal static GuildPermissions ReturnRolePermissions(Role role)
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
    internal static Overwrite ReturnChannelPermissions(Database.Models.BackupModels.Permissions.ChannelPermissions perm)
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
    internal static async Task MerkRestoreEmbed(SocketTextChannel channel)
    {
        IAsyncEnumerable<IReadOnlyCollection<IMessage>>? messages = channel.GetMessagesAsync(50);
        await messages.ForEachAsync(message =>
        {
            foreach (IMessage? message2 in message)
            {
                if (message2.Components.Count == 1 && string.IsNullOrWhiteSpace(message2.Content) && message2.Embeds.Count == 1 && message2.Author.IsBot && message2.Type == MessageType.ApplicationCommand)
                {
                    message2.DeleteAsync();
                    return;
                }
            }
        });
    }

    internal static async Task SendRestoreConfirmationMessage(ShardedInteractionContext context)
    {
        MessageComponent? components = new ComponentBuilder()
        {
            ActionRows = new List<ActionRowBuilder>()
            {
                new ActionRowBuilder()
                {
                    Components = new List<IMessageComponent>
                    {
                        new ButtonBuilder()
                        {
                            CustomId = "confirm-restore-button",
                            Style = ButtonStyle.Success,
                            Label = "Confirm Restore",
                        }.Build(),
                        new ButtonBuilder()
                        {
                            CustomId = "cancel-restore-button",
                            Style = ButtonStyle.Danger,
                            Label = "Cancel Restore"
                        }.Build(),
                    }
                }
            }
        }.Build();
        Embed? embed = new EmbedBuilder()
        {
            Title = $"Restore Backup for {context.Guild.Name}",
            Color = Miscallenous.RandomDiscordColour(),
            Author = new EmbedAuthorBuilder
            {
                Url = "https://restorecord.com",
                Name = "RestoreCord",
                IconUrl = "https://i.imgur.com/Nfy4OoG.png"
            },
            Footer = new EmbedFooterBuilder
            {
                Text = $"Issued by: {context.User.Username} | {context.User.Id}",
                IconUrl = context.User.GetAvatarUrl()
            },
            Description = "Do you want to load the Backup?\n**This will delete ALL ROLES, CHANNELS AND CATEGORIES**",
        }.WithCurrentTimestamp().Build();
        await context.Interaction.RespondAsync(embed: embed, components: components);
    }

    #region Useless
    internal static async ValueTask<(bool, DiscordErrorCode?)> AddUserToGuildViaBot(IInteractionContext context, Member databaseMember, Server databaseServer)
    {
        try
        {
            return await AddUserToGuild(context, databaseMember, databaseServer) ? (true, null) : (false, null);
        }
        catch (HttpException e)
        {
            Console.WriteLine(e.DiscordCode + e.Message);
            switch (e.DiscordCode)
            {
                case DiscordErrorCode.MissingPermissions:
                    return (false, e.DiscordCode);
                case DiscordErrorCode.UserBanned:
                    return (false, e.DiscordCode);
                case DiscordErrorCode.InvalidOAuth2Token:
                    return (false, e.DiscordCode);
                case DiscordErrorCode.MaximumGuildsReached:
                    return (false, e.DiscordCode);
                default:
                    throw;
            }
        }
    }
    internal static async ValueTask<bool> AddUserToGuild(IInteractionContext context, Member user, Server server)
    {
        if (server.roleid is not null)
        {
            await context.Guild.AddGuildUserAsync(user.userid, user.access_token, x => x.RoleIds = new List<ulong>
            {
                (ulong)server.roleid
            });
            return true;
        }
        IGuildUser? newUser = await context.Guild.AddGuildUserAsync(user.userid, user.access_token);
        return true;
    }
    internal static async ValueTask<TokenResponse?> GetInfo(string code, HttpClient http, string clientId, string clientSecret, string uriRedirect)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = uriRedirect,
        });
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
        HttpResponseMessage? response = await http.PostAsync("https://discordapp.com/api/oauth2/token", content);
        return JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());
    }
    internal static async ValueTask<UserResponse?> GrabUser(HttpClient http)
    {
        string? response = await http.GetStringAsync("https://discord.com/api/users/@me");
        return JsonConvert.DeserializeObject<UserResponse>(response);
    }
    #endregion

}

public static class BackupMigration
{
    internal static async ValueTask<Backup> BackupGuildAsync(this Server server, DatabaseContext database, SocketGuild guild, ShardedInteractionContext context)
    {
        var backupEntry = new Backup
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
        backupEntry.roles = backupEntry.BackupRoles(guild);
        await database.ApplyChangesAsync(server);
        if (await DiscordExtensions.CheckBusinessMembership(database, context, false))
        {
            backupEntry.users = await backupEntry.BackupUsersAsync(guild);
        }
        backupEntry.catgeoryChannels = backupEntry.BackupCategories(guild);
        backupEntry.textChannels = backupEntry.BackupTextChannels(guild);
        backupEntry.voiceChannels = backupEntry.BackupVoiceChannels(guild);
        backupEntry.emojis = await backupEntry.BackupEmojisAsync(guild);
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

    internal static async Task DeleteGuildBackupAsync(this Server server, DatabaseContext database)
    {
        if (server.backup is null)
            return;
        //server.backup.rulesChannel = null;
        //server.backup.publicUpdatesChannel = null;
        //server.backup.systemChannel = null;
        //server.backup.widgetChannel = null;
        //await database.ApplyChangesAsync(server);
        foreach (TextChannel? channel in server.backup.textChannels)
        {
            foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? permission in channel.permissions)
            {
                channel.permissions.Remove(permission);
                database.Remove(permission);
            }
            await server.backup.DeleteTextChannelAsync(database, channel);
        }
        foreach (VoiceChannel? channel in server.backup.voiceChannels)
        {
            foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? permission in channel.permissions)
            {
                channel.permissions.Remove(permission);
                database.Remove(permission);
            }
            await server.backup.DeleteVoiceChannelAsync(database, channel);
        }
        foreach (CategoryChannel? channel in server.backup.catgeoryChannels)
        {
            foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? permission in channel.permissions)
            {
                channel.permissions.Remove(permission);
                database.Remove(permission);
            }
            await server.backup.DeleteCategroyChannelAsync(database, channel);
        }
        foreach (GuildUser? guilduser in server.backup.users)
        {
            foreach (GuildUserRole? role in guilduser.assignedRoles)
            {
                guilduser.assignedRoles.Remove(role);
                database.Remove(role);
            }
            server.backup.users.Remove(guilduser);
            database.Remove(guilduser);
        }
        foreach (Role? role in server.backup.roles)
        {
            server.backup.DeleteRole(database, role);
        }
        foreach (Database.Models.BackupModels.Emoji? emoji in server.backup.emojis)
        {
            server.backup.emojis.Remove(emoji);
            database.Remove(emoji);
        }
        //messages
        database.Remove(server.backup);
    }

    internal static async Task RestoreGuildAsync(this Server server, ShardedInteractionContext context, DatabaseContext database, HttpClient http)
    {
        if (server.backup is null)
            return;
        Discord.Rest.RestGuild? restGuild = await context.Client.Rest.GetGuildAsync(context.Guild.Id);
        Discord.Rest.RestRole? highestGuildRole = restGuild.Roles.MaxBy(x => x.Position);
        var parallelOptions = new ParallelOptions()
        {
            MaxDegreeOfParallelism = 2
        };
        //delete old roles & channels?
        Task deleteRolesTask = Parallel.ForEachAsync(restGuild.Roles, async (role, token) =>
        {
            try
            {
                if (highestGuildRole == role)
                    return;
                if (role.IsManaged is false && role.IsEveryone is false)
                    await role.DeleteAsync();
            }
            catch { }
        });
        Task deleteChannelsTask = Parallel.ForEachAsync(context.Guild.Channels, async (channel, token) =>
        {
            try
            {
                if (channel.Id == context.Interaction.Channel.Id)
                    return;
                await channel.DeleteAsync();
            }
            catch { }
        });
        //download members
        Task downloadGuildMembers = context.Guild.DownloadUsersAsync();
        await Task.WhenAll(deleteRolesTask, deleteChannelsTask, downloadGuildMembers);
        //create roles
        if (server.backup.roles.Any())
        {
            foreach (Role role in server.backup.roles.OrderByDescending(x => x.position).ToList())
            {
                if (role.isManaged)
                    continue;
                if (role.isEveryone)
                {
                    await context.Guild.EveryoneRole.ModifyAsync(x =>
                    {
                        x.Position = role.position;
                        x.Mentionable = role.isMentionable;
                        x.Hoist = role.isHoisted;
                        x.Permissions = OldMigration.ReturnRolePermissions(role);
                        x.Color = new Color(role.color);
                    });
                    foreach (CategoryChannel? channel in server.backup.catgeoryChannels)
                    {
                        foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
                        {
                            if (perm.targetId == role.id)
                                perm.targetId = context.Guild.EveryoneRole.Id;
                        }
                    }
                    foreach (TextChannel? channel in server.backup.textChannels)
                    {
                        foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
                        {
                            if (perm.targetId == role.id)
                                perm.targetId = context.Guild.EveryoneRole.Id;
                        }
                    }
                    foreach (VoiceChannel? channel in server.backup.voiceChannels)
                    {
                        foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
                        {
                            if (perm.targetId == role.id)
                                perm.targetId = context.Guild.EveryoneRole.Id;
                        }
                    }
                    if (server.roleid == role.id)
                        server.roleid = context.Guild.EveryoneRole.Id;
                    role.id = context.Guild.EveryoneRole.Id;
                    continue;
                }
                GuildPermissions perms = OldMigration.ReturnRolePermissions(role);
                Discord.Rest.RestRole? newRole = await context.Guild.CreateRoleAsync(role.name, perms, new Color(role.color), role.isHoisted, role.isMentionable);
                foreach (CategoryChannel? channel in server.backup.catgeoryChannels)
                {
                    foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
                    {
                        if (perm.targetId == role.id)
                            perm.targetId = newRole.Id;
                    }
                }
                foreach (TextChannel? channel in server.backup.textChannels)
                {
                    foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
                    {
                        if (perm.targetId == role.id)
                            perm.targetId = newRole.Id;
                    }
                }
                foreach (VoiceChannel? channel in server.backup.voiceChannels)
                {
                    foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
                    {
                        if (perm.targetId == role.id)
                            perm.targetId = newRole.Id;
                    }
                }
                if (server.roleid == role.id)
                    server.roleid = newRole.Id;
                role.id = newRole.Id;
            }
            ///user roles
            ////check if business
            if (await DiscordExtensions.CheckBusinessMembership(database, context, false))
            {
                if (server.backup.users.Any())
                {
                    foreach (GuildUser? user in server.backup.users)
                    {
                        SocketGuildUser? guildUser = context.Guild.GetUser(user.id);
                        if (guildUser is null)
                            continue;
                        List<ulong> roleIds = new();
                        foreach (GuildUserRole? role in user.assignedRoles)
                        {
                            if (role.role.isEveryone || role.role.isManaged)
                                continue;
                            roleIds.Add(role.role.id);
                        }
                        await guildUser.AddRolesAsync(roleIds);
                    }
                }
            }
        }

        foreach (CategoryChannel channel in server.backup.catgeoryChannels)
        {
            Discord.Rest.RestCategoryChannel? newChannel = await context.Guild.CreateCategoryChannelAsync(channel.name, x =>
            {
                x.Name = channel.name;
                x.Position = channel.position;
                var permissions = new List<Overwrite>();
                foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
                {
                    permissions.Add(OldMigration.ReturnChannelPermissions(perm));
                }
                x.PermissionOverwrites = permissions;
            });
            channel.id = newChannel.Id;
        }
        foreach (TextChannel? channel in server.backup.textChannels)
        {
            Discord.Rest.RestTextChannel? newChannel = await context.Guild.CreateTextChannelAsync(channel.name, x =>
            {
                x.Name = channel.name;
                if (channel.topic is not null)
                    x.Topic = channel.topic;
                x.Locked = channel.locked;
                x.IsNsfw = channel.nsfw;
                x.Position = channel.position;
                if (channel.category is not null)
                    x.CategoryId = channel.category.id;
                x.SlowModeInterval = channel.slowModeInterval;
                x.Archived = channel.archived;
                if (channel.archiveAfter is not null)
                    x.AutoArchiveDuration = new Optional<ThreadArchiveDuration>((ThreadArchiveDuration)channel.archiveAfter);
                var permissions = new List<Overwrite>();
                foreach (Database.Models.BackupModels.Permissions.ChannelPermissions? perm in channel.permissions)
                {
                    permissions.Add(OldMigration.ReturnChannelPermissions(perm));
                }
                x.PermissionOverwrites = permissions;
            });
            channel.id = newChannel.Id;
        }
        foreach (VoiceChannel? channel in server.backup.voiceChannels)
        {
            Discord.Rest.RestVoiceChannel? newChannel = await context.Guild.CreateVoiceChannelAsync(channel.name, x =>
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
                    permissions.Add(OldMigration.ReturnChannelPermissions(perm));
                }
                x.PermissionOverwrites = permissions;
            });
            channel.id = newChannel.Id;
        }

        await context.Guild.ModifyAsync(async x =>
        {
            x.Name = server.backup.guildName;
            x.Icon = string.IsNullOrWhiteSpace(server.backup.iconUrl) is false ? new Optional<Image?>(new Image(await http.GetStreamAsync(server.backup.iconUrl))) : null;

            x.Splash = string.IsNullOrWhiteSpace(server.backup.splashUrl) is false ? new Optional<Image?>(new Image(await http.GetStreamAsync(server.backup.splashUrl))) : null;
            x.Banner = string.IsNullOrWhiteSpace(server.backup.bannerUrl) is false ? new Optional<Image?>(new Image(await http.GetStreamAsync(server.backup.bannerUrl))) : null;
            if (server.backup.afkTimeout is not null)
                x.AfkTimeout = new Optional<int>((int)server.backup.afkTimeout);
            if (server.backup.afkChannel is not null)
                x.AfkChannelId = server.backup.afkChannel.id;
            if (server.backup.systemChannel is not null)
                x.SystemChannel = context.Guild.GetTextChannel(server.backup.systemChannel.id);
            x.ExplicitContentFilter = new Optional<ExplicitContentFilterLevel>((ExplicitContentFilterLevel)server.backup.explicitContentFilterLevel);
            x.DefaultMessageNotifications = new Optional<DefaultMessageNotifications>((DefaultMessageNotifications)server.backup.defaultMessageNotifications);
            if (server.backup.isBoostProgressBarEnabled is not null)
                x.IsBoostProgressBarEnabled = (bool)server.backup.isBoostProgressBarEnabled;
            x.VerificationLevel = new Optional<VerificationLevel>((VerificationLevel)server.backup.verificationLevel);
            x.SystemChannelFlags = new Optional<SystemChannelMessageDeny>((SystemChannelMessageDeny)server.backup.systemChannelMessageDeny);
            x.PreferredLocale = server.backup.preferredLocale;
        });
    }


    #region Backup Roles
    internal static ICollection<Role> BackupRoles(this Backup backup, IGuild guild)
    {
        var roleList = new HashSet<Role>();
        var guildRoles = guild.Roles.ToList();
        foreach (IRole? role in guildRoles)
        {
            roleList.Add(backup.UpdateRole(role));
        }
        return roleList;
    }
    internal static Role UpdateRole(this Backup backup, IRole role)
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
    internal static bool DeleteRole(this Backup backup, DatabaseContext database, Role role)
    {
        Role? roleEntry = backup.roles.FirstOrDefault(x => x.id == role.id);
        if (roleEntry is null)
            return false;
        database.Remove(roleEntry.permissions);
        database.Remove(roleEntry);
        return true;
    }
    #endregion

    #region Backup Channels
    private static Database.Models.BackupModels.Permissions.ChannelPermissions CreateChannelPermissionEntry(Overwrite x, ulong targetId)
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

    internal static ICollection<CategoryChannel> BackupCategories(this Backup backup, SocketGuild guild)
    {
        var channels = new HashSet<CategoryChannel>();
        var guildChannels = guild.CategoryChannels.ToList();
        foreach (SocketCategoryChannel? channel in guildChannels)
        {
            channels.Add(backup.UpdateCategoryChannel(channel));
        }
        return channels;
    }
    internal static ICollection<TextChannel> BackupTextChannels(this Backup backup, SocketGuild guild)
    {
        var channels = new HashSet<TextChannel>();
        var guildChannels = guild.TextChannels.ToList();
        foreach (SocketTextChannel? channel in guildChannels)
        {
            if (channel as SocketThreadChannel is null)
                channels.Add(backup.UpdateTextChannel(channel));
        }
        return channels;
    }
    internal static ICollection<VoiceChannel> BackupVoiceChannels(this Backup backup, SocketGuild guild)
    {
        var channels = new HashSet<VoiceChannel>();
        var guildChannels = guild.VoiceChannels.ToList();
        foreach (SocketVoiceChannel? channel in guildChannels)
        {
            channels.Add(backup.UpdateVoiceChannel(channel));
        }
        return channels;
    }

    internal static CategoryChannel UpdateCategoryChannel(this Backup backup, ICategoryChannel channel)
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
        return channelEntry;
    }
    internal static TextChannel UpdateTextChannel(this Backup guild, ITextChannel channel)
    {
        TextChannel? channelEntry = guild.textChannels.FirstOrDefault(x => x.id == channel.Id);
        if (channelEntry is null)
        {
            var permissionList = new HashSet<Database.Models.BackupModels.Permissions.ChannelPermissions>();
            var channelPerms1 = channel.PermissionOverwrites.ToList();
            foreach (Overwrite x in channelPerms1)
            {
                permissionList.Add(CreateChannelPermissionEntry(x, x.TargetId));
            }
            return channel as SocketNewsChannel is not null
            ? new TextChannel
            {
                id = channel.Id,
                name = channel.Name,
                position = channel.Position,
                category = channel.CategoryId is not null ? guild.catgeoryChannels.FirstOrDefault(x => x.id == channel.CategoryId) : null,
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
                category = channel.CategoryId is not null ? guild.catgeoryChannels.FirstOrDefault(x => x.id == channel.CategoryId) : null,
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
            channelEntry.category = channel.CategoryId is not null ? guild.catgeoryChannels.FirstOrDefault(x => x.id == channel.CategoryId) : null;
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
        }

        return channelEntry;
    }
    internal static VoiceChannel UpdateVoiceChannel(this Backup guild, IVoiceChannel channel)
    {
        VoiceChannel? channelEntry = guild.voiceChannels.FirstOrDefault(x => x.id == channel.Id);
        if (channelEntry is null)
        {
            var permissionList = new HashSet<Database.Models.BackupModels.Permissions.ChannelPermissions>();
            var channelPerms1 = channel.PermissionOverwrites.ToList();
            foreach (Overwrite permission in channelPerms1)
            {
                permissionList.Add(CreateChannelPermissionEntry(permission, permission.TargetId));
            }
            return new VoiceChannel
            {
                id = channel.Id,
                name = channel.Name,
                position = channel.Position,
                category = channel.CategoryId is not null ? guild.catgeoryChannels.FirstOrDefault(x => x.id == channel.CategoryId) : null,
                bitrate = channel.Bitrate,
                userLimit = channel.UserLimit,
                videoQuality = null,
                region = null,
                permissions = permissionList,
            };
        }
        channelEntry.name = channel.Name;
        channelEntry.position = channel.Position;
        channelEntry.category = channel.CategoryId is not null ? guild.catgeoryChannels.FirstOrDefault(x => x.id == channel.CategoryId) : null;
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

        return channelEntry;
    }

    internal static async ValueTask<bool> DeleteCategroyChannelAsync(this Backup backup, DatabaseContext database, CategoryChannel channel)
    {
        CategoryChannel? channelEntry = backup.catgeoryChannels.FirstOrDefault(x => x.id == channel.id);
        if (channelEntry is null)
            return false;
        await channelEntry.permissions.ToAsyncEnumerable().ForEachAsync(x => channelEntry.permissions.Remove(x));
        database.Remove(channelEntry);
        backup.catgeoryChannels.Remove(channelEntry);
        return true;
    }
    internal static async ValueTask<bool> DeleteTextChannelAsync(this Backup backup, DatabaseContext database, TextChannel channel)
    {
        TextChannel? channelEntry = backup.textChannels.FirstOrDefault(x => x.id == channel.id);
        if (channelEntry is null)
            return false;
        await channelEntry.permissions.ToAsyncEnumerable().ForEachAsync(x => channelEntry.permissions.Remove(x));
        backup.textChannels.Remove(channelEntry);
        database.Remove(channelEntry);
        return true;
    }
    internal static async ValueTask<bool> DeleteVoiceChannelAsync(this Backup backup, DatabaseContext database, VoiceChannel channel)
    {
        VoiceChannel? channelEntry = backup.voiceChannels.FirstOrDefault(x => x.id == channel.id);
        if (channelEntry is null)
            return false;
        await channelEntry.permissions.ToAsyncEnumerable().ForEachAsync(x => channelEntry.permissions.Remove(x));
        backup.voiceChannels.Remove(channelEntry);
        database.Remove(channelEntry);
        return true;
    }
    #endregion

    internal static async ValueTask<ICollection<GuildUser>> BackupUsersAsync(this Backup backup, IGuild guild)
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

    internal static async ValueTask<ICollection<Database.Models.BackupModels.Emoji>> BackupEmojisAsync(this Backup backup, IGuild guild)
    {
        var guildEmojiList = new HashSet<Database.Models.BackupModels.Emoji>();
        IReadOnlyCollection<GuildEmote>? emojis = await guild.GetEmotesAsync();
        foreach(GuildEmote? emoji in emojis)
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