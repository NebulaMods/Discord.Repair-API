namespace DiscordRepair.Api.Records.Responses.Server;

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
    public string? guildId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? roleId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid key { get; set; }

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
    public bool captcha { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public string? webhook { get; set; }

    //successverifysettings
}

public record GetVerifyPageResponse()
{
    public string serverName { get; set; }
    public string? backgroundImage { get; set; }
    public string pic { get; set; }
    public string guildId { get; set; }
    public bool captcha { get; set; }
}
