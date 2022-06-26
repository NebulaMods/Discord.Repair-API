using Discord;
using Microsoft.AspNetCore.Mvc;
using RestoreCord.Database.Models;
using RestoreCord.Database;
using RestoreCord.Records.Responses;
using Microsoft.EntityFrameworkCore;

namespace RestoreCord.Utilities;

internal static class Miscallenous
{
    internal static Color RandomDiscordColour() => new(new Random().Next(0, 255), new Random().Next(0, 255), new Random().Next(0, 255));

    internal static async Task<Server?> VerifyServer(ControllerBase @base, ulong guildId, ulong userId, DatabaseContext database)
    {
        if (userId is 0 || guildId is 0)
        {
            @base.BadRequest(new GenericResponse()
            {
                success = false,
                details = "invalid parameters, please try again."
            });
            return null;
        }
        Server? server = await database.servers.FirstOrDefaultAsync(x => x.guildId == guildId);
        if (server is null)
        {
            @base.BadRequest(new GenericResponse()
            {
                success = false,
                details = "guild does not exist, please try again."
            });
            return null;
        }
        if (server.banned)
        {
            @base.BadRequest(new GenericResponse()
            {
                success = false,
                details = "guild is banned."
            });
            return null;
        }
        return server;
    }
    internal static async Task<Server?> VerifyServer(ControllerBase @base, ulong guildId, DatabaseContext database)
    {
        if (guildId is 0)
        {
            @base.BadRequest(new GenericResponse()
            {
                success = false,
                details = "invalid parameters, please try again."
            });
            return null;
        }
        Server? server = await database.servers.FirstOrDefaultAsync(x => x.guildId == guildId);
        if (server is null)
        {
            @base.BadRequest(new GenericResponse()
            {
                success = false,
                details = "guild does not exist, please try again."
            });
            return null;
        }
        if (server.banned)
        {
            @base.BadRequest(new GenericResponse()
            {
                success = false,
                details = "guild is banned."
            });
            return null;
        }
        return server;
    }
}
