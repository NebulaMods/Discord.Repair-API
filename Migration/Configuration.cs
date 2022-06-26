using System.Collections.Concurrent;

namespace RestoreCord.Migrations;

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

    }
    public readonly string _token = Properties.Resources.Token;
    public readonly string _clientId = Properties.Resources.ClientID;
    public readonly string _clientSecret = Properties.Resources.ClientSecret;
    public ConcurrentDictionary<Database.Models.LogModels.Statistics, Task> _runningMigrations = new();
}
