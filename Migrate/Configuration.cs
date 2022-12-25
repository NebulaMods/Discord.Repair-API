using System.Collections.Concurrent;

namespace DiscordRepair.Api.MigrationMaster;

/// <summary>
/// 
/// </summary>
public class Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public Configuration()
    {
        _runningMigrations = new();
        _clientSecret = "";
        _clientId = "";
        _token = "";
    }
    public readonly string _token;
    public readonly string _clientId;
    public readonly string _clientSecret;
    public ConcurrentDictionary<Database.Models.LogModels.Statistics, Task> _runningMigrations;
}
