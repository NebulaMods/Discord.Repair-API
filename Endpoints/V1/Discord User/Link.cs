using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using DiscordRepair.Database;
using DiscordRepair.Database.Models;
using DiscordRepair.Records.Responses;
using DiscordRepair.Utilities;

namespace DiscordRepair.Endpoints.V1.DiscordUser;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/discord-user/")]
[ApiExplorerSettings(GroupName = "Discord Account Endpoints")]
[AllowAnonymous]
public class Link : ControllerBase
{
    /// <summary>
    /// Link a discord user to a guild/server using a JSON payload.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="guildId"></param>
    /// <param name="user"></param>
    /// <remarks>Link a discord user to a guild/server using a JSON payload.</remarks>
    /// <returns></returns>
    [HttpPut("{userId}/link/{guildId}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 201)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Generic>> PutAsync(ulong userId, ulong guildId, Records.Requests.Guild.User.Link user)
    {
        if (userId == 0 || guildId == 0 || user is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }
        await using var database = new DatabaseContext();
        var result = await this.VerifyServer(guildId, userId, database, HttpContext.Request.Headers["Authorization"]);
        if (result.Item1 is not null)
            return result.Item1;
        if (result.Item2 is null)
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });

        Blacklist? blacklistUser = result.Item2.settings.blacklist.FirstOrDefault(x => x.discordId == userId);
        if (blacklistUser is not null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "user is blacklisted."
            });
        }
        Member? userEntry = await database.members.FirstOrDefaultAsync(x => x.discordId == userId && x.botUsed.name == user.bot);
        Database.Models.User? owner = await database.users.FirstAsync(x => x.username == result.Item2.owner.username);
        if (userEntry is not null)
        {
            await database.BatchUpdate<Member>()
                .Set(x => x.accessToken, x => user.accessToken)
                .Set(x => x.refreshToken, x => user.refreshToken)
                .Set(x => x.avatar, x => user.avatar)
                .Set(x => x.ip, x => user.ip)
                .Set(x => x.username, x => user.username)
                .Where(x => x.discordId == userEntry.discordId && x.botUsed.clientId == user.bot)
                .ExecuteAsync();
            await database.ApplyChangesAsync();
            return Ok(new Generic()
            {
                details = "successfully updated user."
            });
        }
        var userBot = owner.bots.FirstOrDefault(x => x.name == user.bot || x.key.ToString() == user.bot);
        if (userBot is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "bot does not exist, please try again."
            });
        }
        var newMember = new Member()
        {
            discordId = userId,
            accessToken = user.accessToken,
            avatar = user.avatar,
            creationDate = user.creationDate,
            ip = user.ip,
            refreshToken = user.refreshToken,
            botUsed = userBot,
            username = user.username,
            server = result.Item2,
        };
        await database.members.AddAsync(newMember);
        await database.ApplyChangesAsync();
        return Created($"https://discord.repair/v1/discord-user/{newMember.discordId}/guilds", new Generic()
        {
            success = true,
            details = "successfully linked user to server."
        });
    }

    /// <summary>
    /// Link a discord user to a guild/server using Discord's OAuth2.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="state"></param>
    /// <remarks>Link a discord user to a guild/server using Discord's OAuth2.</remarks>
    /// <returns></returns>
    [HttpGet("link")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 201)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Generic>> GetAsync(string code, string state)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state))
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }
        if (ulong.TryParse(state, out var guildId) is false)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }
        using var http = new HttpClient();
        await using var database = new DatabaseContext();
        var result = await this.VerifyServer(guildId, database, useToken: false);
        if (result.Item1 is not null)
            return result.Item1;
        if (result.Item2 is null)
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        var results1 = await GetInfo(code, http, result.Item2.settings.mainBot.clientId, result.Item2.settings.mainBot.clientSecret, Properties.Resources.UrlRedirect);
        var results = JsonConvert.DeserializeObject<TokenResponse>(results1);
        var results2 = JsonConvert.DeserializeObject<AboutMe>(await GetAboutMe(results.access_token, http));
        Database.Models.Blacklist? blacklistUser = result.Item2.settings.blacklist.FirstOrDefault(x => x.discordId == results2.user.id);
        if (blacklistUser is not null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "user is blacklisted."
            });
        }
        var newMember = new Member()
        {
            accessToken = results.access_token,
            refreshToken = results.refresh_token,
            server = result.Item2,
            botUsed = result.Item2.settings.mainBot,
            avatar = results2.user.avatar,
            discordId = results2.user.id,
            username = results2.user.username,
        };
        await database.members.AddAsync(newMember);
        await database.ApplyChangesAsync();
        return Created($"https://discord.repair/v1/discord-user/{newMember.discordId}/guilds",new Generic()
        {
            success = true,
            details = $"successfully linked user."
        });
    }

    public static async ValueTask<string?> GetInfo(string code, HttpClient http, string clientId, string clientSecret, string uriRedirect)
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
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> GetAboutMe(string accessToken, HttpClient http)
    {
        http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        HttpResponseMessage? response = await http.GetAsync("https://discordapp.com/api/oauth2/@me");
        return await response.Content.ReadAsStringAsync();
    }

    public record TokenResponse
    {
        public string? access_token { get; set; }
        public long expires_in { get; set; }
        public string? refresh_token { get; set; }
        public string? scope { get; set; }
        public string? token_type { get; set; }
    }
    public record Application
    {
        public string id { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
        public string description { get; set; }
        public string summary { get; set; }
        public object type { get; set; }
        public bool hook { get; set; }
        public bool bot_public { get; set; }
        public bool bot_require_code_grant { get; set; }
        public string verify_key { get; set; }
    }

    public record AboutMe
    {
        public Application application { get; set; }
        public List<string> scopes { get; set; }
        public DateTime expires { get; set; }
        public User user { get; set; }
    }

    public record User
    {
        public ulong id { get; set; }
        public string username { get; set; }
        public string avatar { get; set; }
        public object avatar_decoration { get; set; }
        public string discriminator { get; set; }
        public int public_flags { get; set; }
    }


}
