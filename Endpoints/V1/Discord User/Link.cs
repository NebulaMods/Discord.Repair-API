using Discord;
using Discord.Rest;
using Discord.Webhook;

using DiscordRepair.Api.Database;
using DiscordRepair.Api.Database.Models;
using DiscordRepair.Api.Records.Discord;
#if !DEBUG
using DiscordRepair.Api.Records.Requests;
#endif
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace DiscordRepair.Api.Endpoints.V1.DiscordUser;

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
    public async Task<ActionResult<Generic>> PutAsync(ulong userId, ulong guildId, Records.Requests.Server.User.Link user)
    {
        if (userId == 0 || guildId == 0 || user is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }
        var verifyResult = this.VerifyServer(guildId, userId, HttpContext.WhatIsMyToken());
        if (verifyResult is not null)
            return verifyResult;
        await using var database = new DatabaseContext();
        var (httpResult, server) = await this.VerifyServer(database, guildId, HttpContext.WhatIsMyToken());
        if (httpResult is not null)
            return httpResult;
        if (server is null)
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });

        Blacklist? blacklistUser = server.settings.blacklist.FirstOrDefault(x => x.discordId == userId);
        if (blacklistUser is not null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "user is blacklisted."
            });
        }
        Member? userEntry = await database.members.FirstOrDefaultAsync(x => x.discordId == userId && x.botUsed.name == user.bot);
        Database.Models.User? owner = await database.users.FirstAsync(x => x.username == server.owner.username);
        if (userEntry is not null)
        {
            await database.members.Where(x => x.discordId == userEntry.discordId && x.botUsed.clientId == user.bot).ExecuteUpdateAsync(x =>
                x.SetProperty(x => x.accessToken, x => user.accessToken)
                .SetProperty(x => x.refreshToken, x => user.refreshToken)
                .SetProperty(x => x.avatar, x => user.avatar)
                .SetProperty(x => x.ip, x => user.ip)
                .SetProperty(x => x.username, x => user.username)
            );
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
            ip = user.ip,
            refreshToken = user.refreshToken,
            botUsed = userBot,
            username = user.username,
            server = server,
        };
        await database.members.AddAsync(newMember);
        await database.ApplyChangesAsync();
        if (server.roleId is not null)
        {
            await using var discordClient = new DiscordRestClient();
            await discordClient.LoginAsync(TokenType.Bot, server.settings.mainBot.token);
            await discordClient.AddRoleAsync(server.guildId, newMember.discordId, (ulong)server.roleId);
        }
        await database.DisposeAsync();
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
    /// <param name="captcha"></param>
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
    public async Task<ActionResult<Generic>> GetAsync(string code, string state, string? captcha = null)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state))
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }
        if (Guid.TryParse(state, out var serverKey) is false)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }
        var ipAddy = HttpContext.GetIPAddress();
        if (string.IsNullOrWhiteSpace(ipAddy))
        {
            return NoContent();
        }
        using var http = new HttpClient();
        await using var database = new DatabaseContext();
        var server = await database.servers.FirstOrDefaultAsync(x => x.key.ToString() == state);
        if (server is null)
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });

#if !DEBUG
        if (server.settings.captcha)
        {
            if (string.IsNullOrWhiteSpace(captcha))
            {
                return BadRequest(new Generic()
                {
                    success = false,
                    details = "invalid captcha, please try again."
                });
            }
            //var formContent = new Dictionary<string, string>
            //{
            //    { "response", userRequest.captchaCode },
            //    { "secret", Properties.Resources.HCaptchaKey },
            //    { "sitekey", "0d92223e-505f-4dd9-a808-55378fa9307c" }
            //};
            var formContent = new Dictionary<string, string>
            {
                { "response", captcha },
                { "secret", Properties.Resources.ReCaptchaKey },
            };
            var content = new FormUrlEncodedContent(formContent);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            //var requestResults = await http.PostAsync($"https://hcaptcha.com/siteverify", content);
            var requestResults = await http.PostAsync($"https://www.google.com/recaptcha/api/siteverify", content);
            var captchaResults = JsonConvert.DeserializeObject<ReCaptchaResponse>(await requestResults.Content.ReadAsStringAsync());
            //var captchaResults = JsonConvert.DeserializeObject<HCaptchaResponse>(await requestResults.Content.ReadAsStringAsync());
            if (captchaResults is null)
            {
                return BadRequest(new Generic()
                {
                    success = false,
                    details = "invalid captcha, please try again."
                });
            }
            if (captchaResults.success is false)
            {
                return BadRequest(new Generic()
                {
                    success = false,
                    details = "invalid captcha, please try again."
                });
            }
        }
#endif
        if (server.owner.accountType is not AccountType.Staff or AccountType.Premium)
        {
            if (await database.members.Where(x => x.botUsed == server.settings.mainBot || x.server == server).CountAsync() >= int.Parse(Properties.Resources.FreeUserLimit))
            {
                return BadRequest(new Generic()
                {
                    success = false,
                    details = "too many users are linked, please tell the server owner to upgrade their plan in order to be linked."
                });
            }
        }
#if DEBUG
        var userLinkResults = JsonConvert.DeserializeObject<TokenResponse>(await GetInfo(code, http, server.settings.mainBot.clientId, server.settings.mainBot.clientSecret, Properties.Resources.TestUrlRedirect));
#else
        var userLinkResults = JsonConvert.DeserializeObject<TokenResponse>(await GetInfo(code, http, server.settings.mainBot.clientId, server.settings.mainBot.clientSecret, Properties.Resources.UrlRedirect));
#endif
        if (userLinkResults?.access_token is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid, please try again."
            });
        }
        var userLinkedDetails = JsonConvert.DeserializeObject<AboutMe>(await GetAboutMe(userLinkResults.access_token, http));
        if (userLinkedDetails?.user is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid, please try again."
            });
        }
        Blacklist? blacklistUser = server.settings.blacklist.FirstOrDefault(x => x.discordId == userLinkedDetails.user.id);
        if (blacklistUser is not null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "user is blacklisted."
            });
        }
#if !DEBUG
        if (server.settings.vpnCheck)
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
#endif
        var databaseMember = await database.members.FirstOrDefaultAsync(x => x.discordId == userLinkedDetails.user.id && x.server == server);
        if (databaseMember is not null)
        {
            databaseMember.ip = ipAddy;
            databaseMember.accessToken = userLinkResults.access_token;
            databaseMember.refreshToken = userLinkResults.refresh_token;
            databaseMember.avatar = userLinkedDetails.user.avatar;
            databaseMember.username = userLinkedDetails.user.username;
            databaseMember.botUsed = server.settings.mainBot;
            await database.ApplyChangesAsync(databaseMember);
        }
        else
        {
            var newMember = new Member()
            {
                accessToken = userLinkResults.access_token,
                refreshToken = userLinkResults.refresh_token,
                server = server,
                botUsed = server.settings.mainBot,
                avatar = userLinkedDetails.user.avatar,
                discordId = userLinkedDetails.user.id,
                username = userLinkedDetails.user.username,
                ip = ipAddy,
            };
            await database.members.AddAsync(newMember);
            await database.ApplyChangesAsync();
        }
        //attempt to join if not already joined
        //await discordClient.AddGuildUserAsync(server.guildId, userLinkedDetails.user.id, userLinkResults.access_token);
        if (server.roleId is not null)
        {
            await using var discordClient = new DiscordRestClient();
            await discordClient.LoginAsync(TokenType.Bot, server.settings.mainBot.token);
            await discordClient.AddRoleAsync(server.guildId, userLinkedDetails.user.id, (ulong)server.roleId);
        }
        if (string.IsNullOrWhiteSpace(server.settings.webhook) is false)
        {
            var webhook = new DiscordWebhookClient(server.settings.webhook);
            if (server.settings.verifyEmbedSettings is null)
            {
                server.settings.verifyEmbedSettings = new();
                await database.ApplyChangesAsync(server);
            }
            await webhook.SendMessageAsync(embeds: new List<Embed>()
            {
                new EmbedBuilder()
                {
                    Title = server.settings.verifyEmbedSettings.title,
                    Color = Miscallenous.RandomDiscordColour(),
                    Author = new EmbedAuthorBuilder
                    {
                        Url = "https://discord.repair",
                        Name = server.settings.verifyEmbedSettings.authorName,
                        IconUrl = server.settings.verifyEmbedSettings.iconUrl
                    },
                    Footer = new EmbedFooterBuilder
                    {
                        Text = server.settings.verifyEmbedSettings.footerText,
                        IconUrl = server.settings.verifyEmbedSettings.footerIconUrl
                    },
                    Fields = new List<EmbedFieldBuilder>()
                    {
                        new EmbedFieldBuilder()
                        {
                            Name = "Username",
                            Value = $"{userLinkedDetails.user.username}#{userLinkedDetails.user.discriminator}",
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
                            Value = $"{userLinkedDetails.user.id}",
                            IsInline = true
                        },
                    },
                    Description = $"<@{userLinkedDetails.user.id}>"
                }.WithCurrentTimestamp().Build()
            });
        }
        return databaseMember is not null
            ? (ActionResult<Generic>)Ok(new Generic()
            {
                success = true,
                details = $"successfully updated {databaseMember.discordId}"
            })
            : (ActionResult<Generic>)Created($"https://api.discord.repair/v1/discord-user/{userLinkedDetails.user.id}/guilds", new Generic()
            {
                success = true,
                details = $"successfully linked user."
            });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="code"></param>
    /// <param name="http"></param>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <param name="uriRedirect"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="http"></param>
    /// <returns></returns>
    public static async Task<string> GetAboutMe(string accessToken, HttpClient http)
    {
        http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        HttpResponseMessage? response = await http.GetAsync("https://discordapp.com/api/oauth2/@me");
        return await response.Content.ReadAsStringAsync();
    }
}
