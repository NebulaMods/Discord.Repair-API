using Discord;
using Discord.Rest;
using Discord.Webhook;

using DiscordRepair.Database;
using DiscordRepair.Database.Models;
using DiscordRepair.Records.Discord;
using DiscordRepair.Records.Requests;
using DiscordRepair.Records.Responses;
using DiscordRepair.Utilities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace DiscordRepair.Endpoints.V1.DiscordUser;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/discord-user/")]
[ApiExplorerSettings(GroupName = "Discord Account Endpoints")]
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
        if (result.Item2.roleId is not null)
        {
            using var discordClient = new DiscordRestClient();
            await discordClient.LoginAsync(Discord.TokenType.Bot, result.Item2.settings.mainBot.token);
            await discordClient.AddRoleAsync(result.Item2.guildId, newMember.discordId, (ulong)result.Item2.roleId);
        }
        return Created($"https://api.discord.repair/v1/discord-user/{newMember.discordId}/guilds", new Generic()
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
    [AllowAnonymous]
    [HttpGet("link")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 201)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    [ProducesResponseType(204)]
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
#if DEBUG
        var results = JsonConvert.DeserializeObject<TokenResponse>(await GetInfo(code, http, result.Item2.settings.mainBot.clientId, result.Item2.settings.mainBot.clientSecret, Properties.Resources.TestUrlRedirect));
#else
        var results = JsonConvert.DeserializeObject<TokenResponse>(await GetInfo(code, http, result.Item2.settings.mainBot.clientId, result.Item2.settings.mainBot.clientSecret, Properties.Resources.UrlRedirect));
#endif
        if (results?.access_token is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid, please try again."
            });
        }
        var userGettingLinked = JsonConvert.DeserializeObject<AboutMe>(await GetAboutMe(results.access_token, http));
        if (userGettingLinked?.user is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid, please try again."
            });
        }
        Blacklist? blacklistUser = result.Item2.settings.blacklist.FirstOrDefault(x => x.discordId == userGettingLinked.user.id);
        if (blacklistUser is not null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "user is blacklisted."
            });
        }
        var ipAddy = HttpContext.GetIPAddress();
        if (string.IsNullOrWhiteSpace(ipAddy))
        {
            return NoContent();
        }
        if (result.Item2.settings.vpnCheck)
        {
            http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", Properties.Resources.APIToken);
            var geoInfo = await http.GetStringAsync($"https://api.nebulamods.ca/network/geolocation/{ipAddy}");
            if (string.IsNullOrWhiteSpace(geoInfo))
            {
                return BadRequest(new Generic()
                {
                    success = false,
                    details = "ip is not valid."
                });
            }
            var geoData = JsonConvert.DeserializeObject<GeoData>(geoInfo);
            if (geoData is null)
            {
                return BadRequest(new Generic()
                {
                    success = false,
                    details = "ip is not valid."
                });
            }
            if (geoData.cloudProvider is not null)
                if ((bool)geoData.cloudProvider)
                {
                    return BadRequest(new Generic()
                    {
                        success = false,
                        details = "VPN detected, please try again."
                    });
                }
            if (geoData.tor is not null)
                if ((bool)geoData.tor)
                {
                    return BadRequest(new Generic()
                    {
                        success = false,
                        details = "VPN detected, please try again."
                    });
                }
            if (geoData.proxy is not null)
                if ((bool)geoData.proxy)
                {
                    return BadRequest(new Generic()
                    {
                        success = false,
                        details = "VPN detected, please try again."
                    });
                }
        }
        var newMember = new Member()
        {
            accessToken = results.access_token,
            refreshToken = results.refresh_token,
            server = result.Item2,
            botUsed = result.Item2.settings.mainBot,
            avatar = userGettingLinked.user.avatar,
            discordId = userGettingLinked.user.id,
            username = userGettingLinked.user.username,
            ip = ipAddy
        };
        await database.members.AddAsync(newMember);
        await database.ApplyChangesAsync();
        if (result.Item2.roleId is not null)
        {
            using var discordClient = new DiscordRestClient();
            await discordClient.LoginAsync(Discord.TokenType.Bot, result.Item2.settings.mainBot.token);
            await discordClient.AddRoleAsync(result.Item2.guildId, newMember.discordId, (ulong)result.Item2.roleId);
        }

        if (string.IsNullOrWhiteSpace(result.Item2.settings.webhook) is false)
        {
            var webhook = new DiscordWebhookClient(result.Item2.settings.webhook);
            await webhook.SendMessageAsync(embeds: new List<Embed>()
        {
            new EmbedBuilder()
            {
                Title = "Verification Success",
                Color = Miscallenous.RandomDiscordColour(),
                Author = new EmbedAuthorBuilder
                {
                    Url = "https://discord.repair",
                    Name = "Discord.Repair",
                    IconUrl = "https://discord.repair/favicon.ico"
                },
                Footer = new EmbedFooterBuilder
                {
                    Text = "Discord.Repair",
                    IconUrl = "https://discord.repair/favicon.ico"
                },
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        Name = "Username",
                        Value = $"{userGettingLinked.user.username}#{userGettingLinked.user.discriminator}",
                        IsInline = true
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "IP Address",
                        Value = $"[{ipAddy}](https://check-host.net/ip-info?host={ipAddy})",
                        IsInline = true
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "User ID",
                        Value = $"{userGettingLinked.user.id}: <@{userGettingLinked.user.id}>",
                        IsInline = true
                    },
                }
            }.WithCurrentTimestamp().Build()
        });
        }

        return Created($"https://api.discord.repair/v1/discord-user/{newMember.discordId}/guilds",new Generic()
        {
            success = true,
            details = $"successfully linked user."
        });
    }

    public static async ValueTask<string> GetInfo(string code, HttpClient http, string clientId, string clientSecret, string uriRedirect)
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
}
