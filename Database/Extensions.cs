using System.Diagnostics;

using Discord.Net;

using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Database;

/// <summary>
/// Contains extension methods for the DatabaseContext class.
/// </summary>
internal static class DatabaseContextExtension
{
    /// <summary>
    /// Asynchronously saves changes to the database and disposes the database context if requested.
    /// </summary>
    /// <param name="database">The database context.</param>
    /// <param name="entity">The entity to update, if any.</param>
    /// <param name="dispose">Whether to dispose the database context after saving changes.</param>
    /// <returns>The number of entities written to the database.</returns>
    internal static async Task<int> ApplyChangesAsync(this DatabaseContext database, object? entity = null, bool dispose = false)
    {
        try
        {
            if (entity?.GetType().IsValueType == true)
            {
                // Entity is a value type, so we can't use Update method.
                database.Entry(entity).State = EntityState.Modified;
            }
            else if (entity is not null)
            {
                // Entity is a reference type, so we can use the Update method.
                database.Update(entity);
            }

            var result = await database.SaveChangesAsync();

            if (dispose)
            {
                await database.DisposeAsync();
            }

            return result;
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{e.Source}\n{e.StackTrace}\n{e.Message}");
            Console.ForegroundColor = ConsoleColor.White;
            return 0;
        }
    }

    /// <summary>
    /// Asynchronously logs an exception to the database and optionally to the console.
    /// </summary>
    /// <param name="e">The exception to log.</param>
    /// <param name="info">Additional information to include in the log.</param>
    /// <param name="logToConsole">Whether to log the exception to the console.</param>
    internal static async Task LogErrorAsync(this Exception e, string? info = null, bool logToConsole = false)
    {
        try
        {
            if (e is HttpException httpError && (httpError.DiscordCode == Discord.DiscordErrorCode.InvalidWebhookToken || httpError.DiscordCode == Discord.DiscordErrorCode.UnknownMessage))
            {
                // Don't log HttpExceptions with certain Discord error codes.
                return;
            }

            if (e.Message == "Cannot respond to an interaction after 3 seconds!")
            {
                // Don't log exceptions caused by Discord interactions timing out.
                return;
            }

            if (Debugger.IsAttached || logToConsole)
            {
                // Log the exception to the console if a debugger is attached or if logging to the console is requested.
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{e.Source}\n{e.StackTrace}\n{e.Message}");
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
            // If an exception occurs while logging the exception, log both exceptions to the console.
            Console.WriteLine($"{e.Message}\n{ex.Message}\n{ex.InnerException?.Message}");
        }
    }
}

