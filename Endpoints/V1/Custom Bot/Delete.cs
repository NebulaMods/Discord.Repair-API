using DiscordRepair.Api.Database;
using DiscordRepair.Api.Database.Models;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Endpoints.V1.CustomBot;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/custom-bot/")]
[ApiExplorerSettings(GroupName = "Custom Bot Endpoints")]
public class Delete : ControllerBase
{
    /// <summary>
    /// Delete a custom bot using the GUID or name.
    /// </summary>
    /// <param name="bot"></param>
    /// <remarks>Delete a custom bot using the GUID or name.</remarks>
    /// <returns></returns>
    [HttpDelete("{bot}")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult> HandlesAsync(string bot)
    {
        if (string.IsNullOrWhiteSpace(bot))
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "inalivd paramater, please try again."
            });
        }
        if (bot.Length > 64)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "inalivd paramater, please try again."
            });
        }
        await using var database = new DatabaseContext();
        var user = await database.users.FirstAsync(x => x.username == HttpContext.WhoAmI());
        var customBot = user.bots.FirstOrDefault(x => x.name == bot || x.key.ToString() == bot);
        if (customBot is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "bot does not exist in database, please try again."
            });
        }
        if (await database.members.AnyAsync(x => x.botUsed == customBot))
        {
            var members = await database.members.Where(x => x.botUsed == customBot).ToListAsync();
            database.RemoveRange(members);
        }
        if (await database.servers.AnyAsync(x => x.settings.mainBot == customBot))
        {
            var servers = await database.servers.Where(x => x.settings.mainBot == customBot).ToListAsync();
            List<ServerSettings> serverSettingsList = new();
            foreach (var server in servers)
            {
                serverSettingsList.Add(server.settings);
            }
            database.RemoveRange(serverSettingsList);
            database.RemoveRange(servers);
        }
        user.bots.Remove(customBot);
        database.Remove(customBot);
        await database.ApplyChangesAsync();
        return Ok(new Generic()
        {
            success = true,
            details = "successfully deleted bot, and all associated data (servers, members)."
        });
    }
}
