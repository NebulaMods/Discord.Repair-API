using System.Collections.Concurrent;

using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Services;

/// <summary>
/// A class for loading user API tokens from the database into memory.
/// </summary>
public class TokenLoader
{
    // A thread-safe dictionary for storing API tokens in memory.
    internal ConcurrentDictionary<string, string> APITokens { get; set; } = new();

    /// <summary>
    /// Creates a new instance of the <see cref="TokenLoader"/> class and starts loading API tokens into memory.
    /// </summary>
    public TokenLoader()
    {
        Task.Run(LoadTokensIntoMemory);
    }

    /// <summary>
    /// Loads API tokens from the database into memory.
    /// </summary>
    private async Task LoadTokensIntoMemory()
    {
        // Create a new instance of the database context.
        await using var database = new Database.DatabaseContext();

        // Get all non-banned users from the database.
        var users = await database.users.Where(x => x.banned == false).ToListAsync();

        // Add each user's API token to the dictionary.
        foreach (var user in users)
        {
            APITokens.TryAdd(user.apiToken, user.username);
        }

        // Dispose of the database context.
        await database.DisposeAsync();
    }
}