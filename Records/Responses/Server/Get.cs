namespace DiscordRepair.Records.Responses.Server;

/// <summary>
/// 
/// </summary>
public record GetServerResponse
{
    /// <summary>
    /// 
    /// </summary>
    public string name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ulong? guildId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ulong? roleId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid key { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid mainBotKey { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string mainBotName { get; set; }

    /// <summary>
    /// 
    /// </summary>

    public string? vanityUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int webhookLogType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public string? redirectUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string pic { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public string? backgroundImage { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool vpnCheck { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public string? webhook { get; set; }
}
