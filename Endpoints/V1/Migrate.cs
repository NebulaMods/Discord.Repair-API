using System.Net;
using System.Reflection;

using DiscordRepair.Api.Database;
using DiscordRepair.Api.Database.Models;
using DiscordRepair.Api.Records.Requests;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace DiscordRepair.Api.Endpoints.V1;

/// <summary>
/// 
/// </summary>
//[ApiController]
//[Route("/v1/")]
public class Migrate : ControllerBase
{
    private readonly MigrationMaster.Pull _migration;
    private readonly MigrationMaster.Configuration _migrationConfiguration;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="migration"></param>
    public Migrate(MigrationMaster.Pull migration, MigrationMaster.Configuration migrationConfiguration)
    {
        _migration = migration;
        _migrationConfiguration = migrationConfiguration;
    }


    /// <summary>
    /// Migrate/Pull users to your server using the guild ID.
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="migrationRequest"></param>
    /// <remarks>Migrate/Pull users to your server using the guild ID.</remarks>
    /// <returns></returns>
    //[HttpPost("migrate/{guildId}")]
    //[Consumes("application/json")]
    //[Produces("application/json")]
    //[ProducesResponseType(typeof(Generic), 200)]
    //[ProducesResponseType(typeof(Generic), 404)]
    //[ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult> HandleAsync(ulong guildId, MigrationRequest migrationRequest)
    {
        if (guildId is 0 || migrationRequest is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramters, please try again."
            });
        }

        if (string.IsNullOrWhiteSpace(migrationRequest.bot) && string.IsNullOrWhiteSpace(migrationRequest.server))
        {
            return BadRequest(new Generic()
            {

            });
        }

        var verifyResult = this.VerifyServer(guildId, HttpContext.WhatIsMyToken());
        if (verifyResult is not null)
        {
            return verifyResult;
        }

        await using var database = new DatabaseContext();
        var (httpResult, serverEntry) = await this.VerifyServer(database, guildId, HttpContext.WhatIsMyToken());
        if (httpResult is not null)
        {
            return httpResult;
        }

        if (serverEntry is null)
        {
            var user = await database.users.FirstAsync(x => x.apiToken == HttpContext.WhatIsMyToken());
            var bot = user.bots.FirstOrDefault(x => x.name == migrationRequest.bot || x.key.ToString() == migrationRequest.bot);
            if (bot is null)
            {
                return BadRequest(new Generic()
                {
                    success = false,
                    details = "bot or server does not exist, please try again."
                });
            }
            _ = Task.Run(async () => await MigrateUsersAsync(guildId, bot.key, null, migrationRequest?.verifyRoleId, migrationRequest?.userId, migrationRequest?.amountToMigrate, migrationRequest is null || migrationRequest.random));
            return Ok();
        }
        _ = Task.Run(async () => await MigrateUsersAsync(guildId, null, serverEntry.name, migrationRequest?.verifyRoleId, migrationRequest?.userId, migrationRequest?.amountToMigrate, migrationRequest is null || migrationRequest.random));
        await database.DisposeAsync();
        return Ok(new Generic()
        {
            success = true,
            details = "started migration"
        });
    }


    private async ValueTask<HttpResponseMessage> MigrateUserAsync(Member member, HttpClient httpClient, ulong guildId, ulong? verifyRoleId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(new test()
        {
            access_token = member.accessToken,
            roles = verifyRoleId is not null ? new ulong[] { (ulong)verifyRoleId } : null,
        }));
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        return await httpClient.PutAsync($"https://discord.com/api/guilds/{guildId}/members/{member.discordId}", content);
    }
    private record test
    {
        public string? access_token { get; set; }
        public ulong[]? roles { get; set; }
    }

    private async Task MigrateUsersAsync(ulong guildId, Guid? botKey = null, string? serverName = null, ulong? verifyRoleId = null, ulong? userId = null, int? amount = null, bool random = true)
    {
        try
        {
            await using var database = new DatabaseContext();
            Database.Models.Server? server = null;
            Database.Models.CustomBot? bot = null;
            if (botKey is null && serverName is not null)
            {
                server = await database.servers.FirstAsync(x => x.name == serverName);
                bot = server.settings.mainBot;
            }
            else
            {
                var user = await database.users.FirstAsync(x => x.bots.First(x => x.key == botKey).key == botKey);
                bot = user.bots.First(x => x.key == botKey);
            }
            #region HTTP Client
            var proxy = new WebProxy()
            {
                Address = new Uri("http://p.webshare.io:80"),
                Credentials = new NetworkCredential()
                {
                    UserName = "nebulamods-rotate",
                    Password = Properties.Resources.ProxyPassword
                },
            };
            using var handler = new HttpClientHandler()
            {
                Proxy = proxy,
                PreAuthenticate = true,
                UseDefaultCredentials = false,
                UseProxy = true
            };
            using var httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Remove("User-Agent");
            httpClient.DefaultRequestHeaders.Remove("X-RateLimit-Precision");
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bot", bot.token);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-RateLimit-Precision", "millisecond");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", $"{bot.name} (public release, {Assembly.GetExecutingAssembly().ImageRuntimeVersion})");
            #endregion

            if (userId is not null)
            {
                var singleMember = await database.members.FirstOrDefaultAsync(x => x.discordId == userId);
                if (singleMember is null)
                {
                    return;
                }
                await MigrateUserAsync(singleMember, httpClient, guildId, verifyRoleId);
                return;
            }

            var members = bot is null ? await database.members.Where(x => x.server == server).ToArrayAsync() : await database.members.Where(x => x.botUsed == bot).ToArrayAsync();
            if (members.Length is 0)
            {
                return;
            }
            if (amount is null or 0)
            {
                amount = members.Length;
            }

            for (var i = 0; i < amount; i++)
            {
                var member = members[i];
                if (serverName is not null)
                {
                    //blacklist etc
                }
                var memberResult = await MigrateUserAsync(member, httpClient, guildId, verifyRoleId);
                if (memberResult.IsSuccessStatusCode)
                {
                    var stringResult = await memberResult.Content.ReadAsStringAsync();
                    Console.WriteLine(stringResult);
                }
                else
                {
                    Console.WriteLine(memberResult.Content.ReadAsStringAsync());
                    return;
                }
            }
            return;
        }
        catch (Exception ex)
        {
            await ex.LogErrorAsync();
        }
    }

    //migrate the database


    private async ValueTask<HttpResponseMessage> RefreshTokenRequest(Member member, HttpClient httpClient)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = member.botUsed.clientId,
            ["client_secret"] = member.botUsed.clientSecret,
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = member.refreshToken,
        });
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
        return await httpClient.PostAsync("https://discordapp.com/api/oauth2/token", content);
    }

}
