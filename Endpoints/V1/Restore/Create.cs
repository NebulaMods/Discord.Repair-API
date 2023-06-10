using System.Reflection;

using Discord;
using Discord.Rest;

using DiscordRepair.Api.Database;
using DiscordRepair.Api.Database.Models;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace DiscordRepair.Api.Endpoints.V1.Restore;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/")]
[ApiExplorerSettings(GroupName = "Restore Endpoints")]
public class Restore : ControllerBase
{
    public record MigrationRequest
    {
        //public int amount { get; set; }
        public string? serverName { get; set; }
        public ulong guildId { get; set; }
        public ulong? roleId { get; set; }
        public bool updateEntities { get; set; }
    }

    [HttpPost("restore")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(MigrationResponse), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult> HandleAsync(MigrationRequest migrationRequest)
    {
        if (migrationRequest is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramters, please try again."
            });
        }

        if (migrationRequest.guildId is 0)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramters, please try again."
            });
        }

        var verifyResult = this.VerifyServer(migrationRequest.serverName, HttpContext.WhatIsMyToken());
        if (verifyResult is not null)
        {
            return verifyResult;
        }
        Guid? serverKey = null;
        await using (var database = new DatabaseContext())
        {
            var (httpResult, serverEntry) = await this.VerifyServer(database, migrationRequest.serverName, HttpContext.WhatIsMyToken());
            if (httpResult is not null)
            {
                return httpResult;
            }
            if (serverEntry is null)
            {
                return BadRequest(new Generic()
                {
                    success = false,
                    details = "server doesn't exist, please try again."
                });
            }
            var migrationEntry = await database.migrations.OrderByDescending(x => x.startTime).FirstOrDefaultAsync(x => x.user == serverEntry.owner && (x.status == MigrationStatus.STARTED || x.status == MigrationStatus.INPROGRESS));
            if (migrationEntry is not null)
            {
                return BadRequest(new Generic()
                {
                    success = false,
                    details = "migration is already in progress, please try again later."
                });
            }
            serverKey = serverEntry.key;
            if (await database.members.Where(x => x.server == serverEntry).AnyAsync() is false)
            {
                return BadRequest(new Generic()
                {
                    success = false,
                    details = "no members to migrate, please try again."
                });
            }
        }
        return await CreateBackgroundTaskAsync((Guid)serverKey, migrationRequest);
    }
    private async Task<ActionResult> CreateBackgroundTaskAsync(Guid serverKey, MigrationRequest migrationRequest)
    {
        Task.Factory.StartNew(async () => await MigrateUsersAsync(serverKey, migrationRequest), TaskCreationOptions.LongRunning);
        await Task.Delay(TimeSpan.FromSeconds(5));
        Migration? migrationEntry = null;
        await using (var database = new DatabaseContext())
        {
            migrationEntry = await database.migrations.OrderByDescending(x => x.startTime).FirstOrDefaultAsync(x => x.server.key == serverKey);
            if (migrationEntry is null)
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
                migrationEntry = await database.migrations.FirstOrDefaultAsync(x => x.server.key == serverKey);
                if (migrationEntry is null)
                {
                    return BadRequest(new Generic()
                    {
                        success = false,
                        details = "Failed to start the migration, please try again. If this continues to happen please contact an admin with this error code: #69-420"
                    });
                }
            }
        }
        //check db for migration entry
        return Ok(new MigrationResponse()
        {
            startTime = migrationEntry.startTime,
            estimatedCompletionTime = DateTime.Now.AddSeconds(5 * 60 * migrationEntry.totalMemberAmount),
            totalMemberAmount = migrationEntry.totalMemberAmount,
            serverName = migrationRequest.serverName,
            status = migrationEntry.status,

        });
    }

    private async Task MigrateUsersAsync(Guid serverKey, MigrationRequest migrationRequest, int parallelTaskCount = 1)
    {
        var newMigration = new Migration()
        {
            status = MigrationStatus.STARTED,
            newGuildId = migrationRequest.guildId,
            newRoleId = migrationRequest.roleId,
        };
        try
        {
            Member[] members = Array.Empty<Member>();
            Database.Models.Server? serverEntry = null;
            string botToken = string.Empty;
            await using (var database = new DatabaseContext())
            {
                serverEntry = await database.servers.FirstOrDefaultAsync(x => x.key == serverKey);
                if (serverEntry is null)
                {
                    newMigration.status = MigrationStatus.FAILED;
                    newMigration.extraDetails = "server entry is null";
                    return;
                }
                botToken = serverEntry.settings.mainBot.token;
                members = await database.members.Where(x => x.server == serverEntry && x.accessToken != "invalid").ToArrayAsync();
                newMigration.server = serverEntry;
                newMigration.totalMemberAmount = members.Length;
                newMigration.status = MigrationStatus.INPROGRESS;
                newMigration.user = serverEntry.owner;
                await database.migrations.AddAsync(newMigration);
                await database.ApplyChangesAsync();
            }
            using var client = new DiscordRestClient();
            await client.LoginAsync(TokenType.Bot, botToken);

            var restGuild = await client.GetGuildAsync(migrationRequest.guildId);
            if (restGuild is null)
            {
                newMigration.status = MigrationStatus.FAILED;
                newMigration.extraDetails = "could not find guild";
                return;
            }
            var existingMembers = await restGuild.GetUsersAsync().FlattenAsync();
            await Parallel.ForEachAsync(members, new ParallelOptions()
            {
                MaxDegreeOfParallelism = parallelTaskCount

            }, async (member, cancellationToken) =>
            {
                if (existingMembers.FirstOrDefault(x => x.Id == member.discordId) is not null)
                {
                    newMigration.alreadyMigratedMemberAmount++;
                    return;
                }
                if (member.botUsed.token == botToken)
                {
                    try
                    {
                        var (restUser, discordError) = await MigrateUserAsync(migrationRequest.roleId, restGuild, member);
                        if (discordError is not null)
                        {
                            switch(discordError)
                            {
                                case DiscordErrorCode.InvalidOAuth2Token:
                                    var newToken = await RefreshToken(member);
                                    if (newToken is null)
                                    {
                                        newMigration.invalidTokenAmount++;
                                        break;
                                    }
                                    member.accessToken = newToken.access_token;
                                    member.refreshToken = newToken.refresh_token;
                                    var (restUser2, discordError2) = await MigrateUserAsync(migrationRequest.roleId, restGuild, member);
                                    if (restUser2 is not null)
                                    {
                                        newMigration.successfulMemberAmount++;
                                        break;
                                    }
                                    newMigration.failedMemberAmount++;
                                    break;
                                case DiscordErrorCode.InsufficientPermissions:
                                    newMigration.failedMemberAmount++;
                                    break;
                                case DiscordErrorCode.MaximumGuildsReached:
                                    newMigration.failedMemberAmount++;
                                    break;
                                default:
                                    newMigration.failedMemberAmount++;
                                    break;
                            }
                            return;
                        }
                        newMigration.successfulMemberAmount++;
                        //increase counter
                    }
                    catch (Discord.Net.HttpException discordEx)
                    {
                        switch (discordEx.DiscordCode)
                        {
                            case DiscordErrorCode.InvalidOAuth2Token:
                                var newToken = await RefreshToken(member);
                                if (newToken is null)
                                {
                                    newMigration.invalidTokenAmount++;
                                    return;
                                }
                                member.accessToken = newToken.access_token;
                                member.refreshToken = newToken.refresh_token;
                                var (restUser, discordError) = await MigrateUserAsync(migrationRequest.roleId, restGuild, member);
                                if (restUser is not null)
                                {
                                    newMigration.successfulMemberAmount++;
                                    return;
                                }
                                switch (discordError)
                                {
                                    default:
                                        newMigration.failedMemberAmount++;
                                        return;
                                }
                            case DiscordErrorCode.InsufficientPermissions:
                                newMigration.failedMemberAmount++;
                                return;
                            case DiscordErrorCode.MaximumGuildsReached:
                                newMigration.failedMemberAmount++;
                                return;
                                case DiscordErrorCode.UnknownAccount:
                                    newMigration.failedMemberAmount++;
                                return;
                            default:
                                newMigration.failedMemberAmount++;
                                Console.WriteLine(discordEx.Message);
                                return;
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        newMigration.failedMemberAmount++;
                        return;
                    }
                    catch (Exception ex)
                    {
                        newMigration.failedMemberAmount++;
                        Console.WriteLine(ex.Message);
                        return;
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
            });
            newMigration.status = MigrationStatus.COMPLETED;
        }
        catch (Exception ex)
        {
            newMigration.status = MigrationStatus.FAILED;
            newMigration.extraDetails = ex.Message;
            await ex.LogErrorAsync();
        }
        finally
        {
            //
            if (newMigration.totalMemberAmount > 0)
            {
                newMigration.completionTime = DateTime.UtcNow;
                await using var database = new DatabaseContext();
                await database.ApplyChangesAsync(newMigration);
            }
        }

    }

    private static async Task<(RestUser? restUser, DiscordErrorCode? discordError)> MigrateUserAsync(ulong? roleId, RestGuild restGuild, Member member)
    {
        try
        {
            return roleId is not null
    ? (await restGuild.AddGuildUserAsync(member.discordId, member.accessToken, x =>
    {
        x.RoleIds = new ulong[] { (ulong)roleId };
    }), null)
    : ((RestUser)await restGuild.AddGuildUserAsync(member.discordId, member.accessToken), null);
        }
        catch(Discord.Net.HttpException discordEx)
        {
            return (null, discordEx.DiscordCode);
        }
    }

    private async Task<TokenResponse?> RefreshToken(Member member)
    {
        var httpClient = await CreateHttpClientAsync(member.botUsed.token, member.botUsed.name);

        var response = await RefreshTokenRequest(member, httpClient);
        var gay = await response.Content.ReadAsStringAsync();
        Console.WriteLine(gay);
        if (response is null)
        {
            return null;
        }

        if (response.IsSuccessStatusCode is false)
        {
            if (gay.Contains("grant"))
            {
                await using (var database = new DatabaseContext())
                {
                    await database.members.Where(x => x.discordId == member.discordId && x.accessToken == member.accessToken)
                        .ExecuteUpdateAsync(x =>
                        x.SetProperty(x => x.accessToken, "invalid"));
                    await database.ApplyChangesAsync();
                }
            }
            return null;
        }

        TokenResponse? result = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());

        if (result is null)
        {
            return null;
        }

        await using (var database = new DatabaseContext())
        {
            await database.members.Where(x => x.discordId == member.discordId && x.accessToken == member.accessToken)
                .ExecuteUpdateAsync(x =>
                x.SetProperty(x => x.accessToken, result.access_token)
                .SetProperty(x => x.refreshToken, result.refresh_token));
            await database.ApplyChangesAsync();
        }
        httpClient.Dispose();
        return result;
    }

    private record TokenResponse
    {
        public string? access_token { get; set; }
        public long expires_in { get; set; }
        public string? refresh_token { get; set; }
        public string? scope { get; set; }
        public string? token_type { get; set; }
    }

    public record MigrationResponse
    {
        public string serverName { get; set; }
        public DateTime startTime { get; set; }
        public DateTime estimatedCompletionTime { get; set; }
        public MigrationStatus status { get; set; }
        public long totalMemberAmount { get; set; }
    }

    private async ValueTask<HttpClient> CreateHttpClientAsync(string botToken, string userAgent = "Discord.Repair")
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
    private async ValueTask<HttpResponseMessage> RefreshTokenRequest(Member member, HttpClient http)
    {
        if (string.IsNullOrWhiteSpace(member.refreshToken))
        {
            throw new NullReferenceException();
        }
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = member.server.settings.mainBot.clientId,
            ["client_secret"] = member.server.settings.mainBot.clientSecret,
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = member.refreshToken,
        });
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
        return await http.PostAsync("https://discordapp.com/api/oauth2/token", content);
    }
}
