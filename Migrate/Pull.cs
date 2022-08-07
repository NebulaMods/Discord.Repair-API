using System.Net;
using System.Reflection;

using Discord;
using Discord.Net;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using DiscordRepair.Database;
using DiscordRepair.Database.Models;
using DiscordRepair.MigrationMaster.Models;

namespace DiscordRepair.MigrationMaster;

/// <summary>
/// 
/// </summary>
public class Pull
{
    private readonly Configuration _configuration;
    public Pull(Configuration configuration)
    {
        _configuration = configuration;
    }
    public async ValueTask<HttpClient> CreateHttpClientAsync(string botToken, string userAgent = "RestoreCord")
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Remove("User-Agent");
        httpClient.DefaultRequestHeaders.Remove("X-RateLimit-Precision");
        httpClient.DefaultRequestHeaders.Remove("Authorization");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bot", botToken);
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-RateLimit-Precision", "millisecond");
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", $"{userAgent} (public release, {Assembly.GetExecutingAssembly().ImageRuntimeVersion})");
        return httpClient;
    }

    internal async ValueTask<bool> JoinUsersToGuild(DatabaseContext database, Server server, HttpClient http, Database.Models.Statistics.MemberMigration statisitics, ulong guildId)
    {
        await using Discord.Rest.DiscordRestClient client = new(new Discord.Rest.DiscordRestConfig()
        {
            LogLevel = LogSeverity.Debug,
            UseSystemClock = true,
            UseInteractionSnowflakeDate = true,
            DefaultRetryMode = RetryMode.RetryTimeouts,
            //RestClientProvider = DefaultRestClientProvider.Create(useProxy: true),
        });
        await client.LoginAsync(TokenType.Bot, server.settings.mainBot.token);

        Discord.Rest.RestGuild? guildSocket = await client.GetGuildAsync(guildId);

        Task<IEnumerable<Discord.Rest.RestGuildUser>>? task1 = guildSocket.GetUsersAsync().FlattenAsync();
        //download banlist & create a task
        Task<IEnumerable<Discord.Rest.RestBan>>? task2 = guildSocket.GetBansAsync().FlattenAsync();
        //grab users from database with specific guild id
        List<Member>? databaseMembers = await database.members.Where(x => x.server == server && x.accessToken != "broken").ToListAsync();
        //grab the users in the guild, need better way of doing this async
        IEnumerable<Discord.Rest.RestGuildUser>? guildMembers = await task1;
        //asign var to task
        IEnumerable<Discord.Rest.RestBan>? bannedMembers = await task2;
        //start migrating each member in the list
        foreach (Member? member in databaseMembers)
        {
            //check if blacklist contains any members
            if (server.settings.blacklist.Any())
            {
                if (server.settings.blacklist.FirstOrDefault(x => x.discordId == member.discordId) is not null)
                {
                    statisitics.bannedCount++;
                    continue;
                }
            }
            //check if the member is already in the guild
            if (guildMembers.FirstOrDefault(x => x.Id == member.discordId) is not null)
            {
                statisitics.alreadyHereCount++;
                continue;
            }
            //check banlist contains any members
            if (bannedMembers.FirstOrDefault(x => x.User.Id == member.discordId) is not null)
            {
                statisitics.bannedCount++;
                continue;
            }
            //try to join the user to the guild
            ResponseTypes addUserRequest = await AddUserFunction(member, server, database, http);
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
        await database.ApplyChangesAsync(statisitics);
        //clear resources
        databaseMembers.Clear();
        return true;
    }
    internal async ValueTask<ResponseTypes> AddUserFunction(Member member, Server server, DatabaseContext database, HttpClient http)
    {
        //var handler = new HttpClientHandler()
        //{
        //    Proxy = new ProxyGenerator(_provider),
        //    PreAuthenticate = true,
        //    UseDefaultCredentials = false,
        //    UseProxy = true
        //};
        ResponseTypes addUserRequest = await AddUserToGuildViaHttp(member, server, http);
        switch (addUserRequest)
        {
            case ResponseTypes.InvalidAuthToken:
                (bool, string?) refreshTokenRequest = await RefreshUserToken(member, database, http);
                if (refreshTokenRequest.Item1)
                {
                    member.accessToken = refreshTokenRequest.Item2;
                    //
                    return await AddUserToGuildViaHttp(member, server, http);
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
                    .Set(x => x.accessToken, x => "broken")
                    .Where(x => x.discordId == member.discordId)
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
            .Set(x => x.accessToken, x => result.access_token)
            .Set(x => x.refreshToken, x => result.refresh_token)
            .Where(x => x.discordId == member.discordId && x.botUsed == member.server.settings.mainBot)
            .ExecuteAsync();
        }
        catch (Exception e)
        {
            await e.LogErrorAsync($"user info backed up incase of corrupt saving: {member.discordId} | {member.accessToken} | {member.refreshToken} |NEW|REFRESH|MAYBE| {result.refresh_token}", true);
            throw;
        }
        return result.access_token;
    }

    private async ValueTask<HttpResponseMessage?> RefreshTokenRequest(Member member, HttpClient http)
    {
        if (string.IsNullOrWhiteSpace(member.refreshToken))
        {
            return null;
        }
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _configuration._clientId,
            ["client_secret"] = _configuration._clientSecret,
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = member.refreshToken,
        });
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
        return await http.PostAsync("https://discordapp.com/api/oauth2/token", content);
    }



    internal async ValueTask<ResponseTypes> AddUserToGuildViaHttp(Member user, Server server, HttpClient http)
    {
        if (string.IsNullOrWhiteSpace(user.accessToken) && string.IsNullOrWhiteSpace(user.refreshToken) is false)
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
                catch (Exception e)
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

        var content = new StringContent(JsonConvert.SerializeObject(new test()
        {
            access_token = user.accessToken,
            roles = server.roleId is not null ? new ulong[] { (ulong)server.roleId } : null,
        }));
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        return await http.PutAsync($"https://discord.com/api/guilds/{server.guildId}/members/{user.discordId}", content);
    }
    private record test
    {
        public string? access_token { get; set; }
        public ulong[]? roles { get; set; }
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
        if (server.roleId is not null)
        {
            await context.Guild.AddGuildUserAsync(user.discordId, user.accessToken, x => x.RoleIds = new List<ulong>
            {
                (ulong)server.roleId
            });
            return true;
        }
        IGuildUser? newUser = await context.Guild.AddGuildUserAsync(user.discordId, user.accessToken);
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
