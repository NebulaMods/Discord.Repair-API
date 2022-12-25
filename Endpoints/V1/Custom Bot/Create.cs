using DiscordRepair.Api.Database;
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
public class Create : ControllerBase
{
    /// <summary>
    /// Create a custom bot.
    /// </summary>
    /// <param name="bot"></param>
    /// <remarks>Create a custom bot.</remarks>
    /// <returns></returns>
    [HttpPut]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Database.Models.CustomBot), 201)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Database.Models.CustomBot>> HandlesAsync(CreateCustomBotRequest bot)
    {
        if (bot is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "inalivd paramater, please try again."
            });
        }
        await using var database = new DatabaseContext();
        var user = await database.users.FirstAsync(x => x.username == HttpContext.WhoAmI());
        if (user.accountType is not Database.Models.AccountType.Staff or Database.Models.AccountType.Premium)
            if (user.bots.Count >= int.Parse(Properties.Resources.FreeBotLimit))
            {
                return BadRequest(new Generic()
                {
                    success = false,
                    details = "please upgrade in order to create another bot"
                });
            }
        if (user.bots.FirstOrDefault(x => x.name == bot.name) is not null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "bot already exists with that name, please try again."
            });
        }
        if (user.bots.FirstOrDefault(x => x.token == bot.token) is not null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "bot already exists with that token, please try again."
            });
        }
        var newBot = new Database.Models.CustomBot()
        {
            botType = bot.botType,
            clientId = bot.clientId,
            clientSecret = bot.clientSecret,
            name = bot.name,
            token = bot.token,
        };
        user.bots.Add(newBot);
        await database.ApplyChangesAsync(user);
        return Created($"https://api.discord.repair/v1/custom-bot/{newBot.key}", newBot);
    }

}
