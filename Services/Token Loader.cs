using System.Collections.Concurrent;

using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Services;

/// <summary>
/// 
/// </summary>
public class TokenLoader
{
    internal ConcurrentDictionary<string, string> APITokens { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    public TokenLoader()
    {
        Task.Run(async () => await LoadTokensIntoMemory());
    }

    internal async Task LoadTokensIntoMemory()
    {
        await using var database = new Database.DatabaseContext();
        var users = await database.users.Where(x => x.banned == false).ToListAsync();
        foreach(var user in users)
        {
            APITokens.TryAdd(user.apiToken, user.username);
        }
    }
}
