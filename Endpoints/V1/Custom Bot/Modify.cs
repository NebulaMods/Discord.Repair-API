using DiscordRepair.Api.Database;
using DiscordRepair.Api.Database.Models;
using DiscordRepair.Api.Records.Requests.CustomBot;
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
public class Modify : ControllerBase
{
    /// <summary>
    /// Modify a custom bot using the GUID or name.
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="botRequest"></param>
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
                details = "bot does not exist."
            });
        }
        if (bot.Length > 64)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "bot does not exist."
            });
        }
        if (botRequest is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid parameters, please try again."
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
        {
            if (botRequest.name != customBot.name)
            {
                if (user.bots.FirstOrDefault(x => x.name == botRequest.name) is not null)
                {
                    return BadRequest(new Generic()
                    {
                        success = false,
                        details = "bot already exists with that name, please try again."
                    });
                }
                customBot.name = botRequest.name;
            }
        }
        if (string.IsNullOrWhiteSpace(botRequest.clientSecret) is false)
        {
            customBot.clientSecret = botRequest.clientSecret;
        }

        if (string.IsNullOrWhiteSpace(botRequest.token) is false)
        {
            if (customBot.token != botRequest.token)
            {
                if (user.bots.FirstOrDefault(x => x.token == botRequest.token) is not null)
                {
                    return BadRequest(new Generic()
                    {
                        success = false,
                        details = "bot already exists with that token, please try again."
                    });
                }
                customBot.token = botRequest.token;
            }
        }
        if (string.IsNullOrWhiteSpace(botRequest.clientId) is false)
        {
            customBot.clientId = botRequest.clientId;
        }

        if (botRequest.botType is not null)
        {
            customBot.botType = (BotType)botRequest.botType;
        }

        await database.ApplyChangesAsync(user);
        return Ok(new Generic()
        {
            success = true,
            details = "successfully updated bot."
        });
    }

}
