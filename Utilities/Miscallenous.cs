using Discord;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using RestoreCord.Database;
using RestoreCord.Database.Models;
using RestoreCord.Records.Responses;

namespace RestoreCord.Utilities;

internal static class Miscallenous
{
    internal static Color RandomDiscordColour() => new(new Random().Next(0, 255), new Random().Next(0, 255), new Random().Next(0, 255));

    internal static async ValueTask<(ActionResult?, Server?)> VerifyServer(this ControllerBase @base, ulong guildId, ulong userId, DatabaseContext database)
    {
        if (userId is 0 || guildId is 0)
        {
            return (@base.BadRequest(new Generic() { success = false, details = "invalid parameters, please try again." }), null);
        }
        Server? server = await database.servers.FirstOrDefaultAsync(x => x.guildId == guildId);
        return server is null
            ? (@base.BadRequest(new Generic() { success = false, details = "guild does not exist, please try again." }), null)
            : server.banned
            ? (@base.BadRequest(new Generic() { success = false, details = "guild is banned." }), null)
            : ((ActionResult?, Server?))(null, server);
    }
    internal static async ValueTask<(ActionResult?, Server?)> VerifyServer(this ControllerBase @base, ulong guildId, DatabaseContext database)
    {
        if (guildId is 0)
        {
            return (@base.BadRequest(new Generic() { success = false, details = "invalid parameters, please try again." }), null);
        }
        Server? server = await database.servers.FirstOrDefaultAsync(x => x.guildId == guildId);
        return server is null
            ? (@base.BadRequest(new Generic()
            {
                success = false,
                details = "guild does not exist, please try again."
            }), null)
            : server.banned
            ? (@base.BadRequest(new Generic()
            {
                success = false,
                details = "guild is banned."
            }), null)
            : ((ActionResult?, Server?))(null, server);
    }
}
