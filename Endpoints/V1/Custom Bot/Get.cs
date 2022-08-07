using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DiscordRepair.Database;
using DiscordRepair.Records.Responses;
using DiscordRepair.Utilities;

namespace DiscordRepair.Endpoints.V1.CustomBot;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/custom-bot/")]
[ApiExplorerSettings(GroupName = "Custom Bot Endpoints")]
public class Get : ControllerBase
{
    /// <summary>
    /// Get information about a specific custom bot using the GUID or name.
    /// </summary>
    /// <param name="bot"></param>
    /// <remarks>Get information about a specific custom bot using the GUID or name.</remarks>
    /// <returns></returns>
    [HttpGet("{bot}")]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Database.Models.CustomBot), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Database.Models.CustomBot>> HandleAsync(string bot)
    {
        if (string.IsNullOrWhiteSpace(bot))
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "inalivd paramater, please try again."
            });
        }
        if (bot.Length > 50)
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
                details = "bot doesn't exist with that name/guid, please try again."
            });
        }
        return Ok(customBot);
    }

    /// <summary>
    /// Get a list of custom bots.
    /// </summary>
    /// <remarks>Get a list of custom bots.</remarks>
    /// <returns></returns>
    [HttpGet]
    [Consumes("plain/text")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Database.Models.CustomBot[]>> HandleGetAllAsync()
    {
        await using var database = new DatabaseContext();
        var user = await database.users.FirstAsync(x => x.username == HttpContext.WhoAmI());
        return user.bots.Any() is false
            ? (ActionResult<Database.Models.CustomBot[]>)BadRequest(new Generic()
            {
                success = false,
                details = "no bots exist, please try again."
            })
            : (ActionResult<Database.Models.CustomBot[]>)Ok(user.bots.ToArray());
    }


}
