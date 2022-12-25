using System.Collections.Concurrent;

using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Services;

/// <summary>
/// 
/// </summary>
public class TokenLoader
{
    //implement redis
    internal ConcurrentDictionary<string, string> APITokens { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    public TokenLoader()
    {
        Task.Run(LoadTokensIntoMemory);
    }

    internal async Task LoadTokensIntoMemory()
    {
        await using var database = new Database.DatabaseContext();
        var users = await database.users.Where(x => x.banned == false).ToListAsync();
        foreach (var user in users)
        {
            APITokens.TryAdd(user.apiToken, user.username);
        }
        await database.DisposeAsync();
    }
}
