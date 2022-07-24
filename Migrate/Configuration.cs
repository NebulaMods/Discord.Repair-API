using System.Collections.Concurrent;

namespace RestoreCord.MigrationMaster;

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
        _clientSecret = Properties.Resources.ClientSecret;
        _clientId = Properties.Resources.ClientID;
        _token = Properties.Resources.Token;
    }
    public readonly string _token;
    public readonly string _clientId;
    public readonly string _clientSecret;
    public ConcurrentDictionary<Database.Models.LogModels.Statistics, Task> _runningMigrations;
}
