using System.Collections.Concurrent;

using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Services;

public class TokenLoader
{
    internal ConcurrentDictionary<string, string> APITokens { get; set; } = new();
    public TokenLoader() => Task.Run(async () => await LoadTokensIntoMemory());

    internal async Task LoadTokensIntoMemory()
    {
        await using var database = new Database.DatabaseContext();
        var users = await database.users.Where(x => x.banned == false).ToListAsync();
        foreach(var user in users)
        {
            APITokens.TryAdd(user.apiToken, user.username);
            //if (APITokens.TryAdd(user.apiToken, user.username))
            //    Console.WriteLine($"{user.username} loaded: {user.apiToken}");
        }
    }
}
