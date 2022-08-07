
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DiscordRepair.Database;
using DiscordRepair.Database.Models;
using DiscordRepair.Records.Requests.CustomBot;
using DiscordRepair.Records.Responses;
using DiscordRepair.Utilities;

namespace DiscordRepair.Endpoints.V1.CustomBot;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/custom-bot/")]
[ApiExplorerSettings(GroupName = "Custom Bot Endpoints")]
public class Modify : ControllerBase
{
    /// <summary>
    /// Modify a custom bot using the GUID or name.
    /// </summary>
    /// <param name="bot"></param>
    /// <remarks>Modify a custom bot using the GUID or name.</remarks>
    /// <returns></returns>
    [HttpPatch("{bot}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult> HandlesAsync(string bot, ModifyCustomBotRequest botRequest)
{
        if (string.IsNullOrWhiteSpace(bot))
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "server does not exist."
            });
        }
        if (bot.Length > 50)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "server does not exist."
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
        if (string.IsNullOrWhiteSpace(botRequest.name) is false)
            customBot.name = botRequest.name;
        if (string.IsNullOrWhiteSpace(botRequest.clientSecret) is false)
            customBot.clientSecret = botRequest.clientSecret;
        if (string.IsNullOrWhiteSpace(botRequest.token) is false)
            customBot.token = botRequest.token;
        if (string.IsNullOrWhiteSpace(botRequest.clientId) is false)
            customBot.clientId = botRequest.clientId;
        if (botRequest.botType is not null)
            customBot.botType = (BotType)botRequest.botType;
        await database.ApplyChangesAsync(user);
            return Ok(new Generic()
            {
                success = true,
                details = "successfully updated bot."
            });
    }

}
