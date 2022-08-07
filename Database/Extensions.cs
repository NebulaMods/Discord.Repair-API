using System.Diagnostics;

using Discord.Net;

namespace DiscordRepair.Database;

internal static class DatabaseContextExtension
{
    internal static async ValueTask<int> ApplyChangesAsync(this DatabaseContext database, object? entity = null)
    {
        try
        {
            if (entity is not null)
            {
                database.Update(entity);
            }
            return await database.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return 0;
        }
    }

    internal static async Task LogErrorAsync(this Exception e, string? info = null, bool logToConsole = false)
    {
        try
        {
            var httpError = e as HttpException;
            if (httpError is not null)
            {
                switch (httpError.DiscordCode)
                {
                    case Discord.DiscordErrorCode.InvalidWebhookToken:
                        return;
                    case Discord.DiscordErrorCode.UnknownMessage:
                        return;
                }
            }
            if (e.Message == "Cannot respond to an interaction after 3 seconds!")
                return;
            if (Debugger.IsAttached || logToConsole)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }
            await using var database = new DatabaseContext();
            var entry = new Models.LogModels.Errors
            {
                errorTime = DateTime.Now,
                stackTrace = e.StackTrace,
                message = e.Message,
                location = e.Source,
                extraInfo = info
            };
            await database.AddAsync(entry);
            await database.ApplyChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
