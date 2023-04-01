using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Api.Records.Requests.Server;
public record ModifyServerRequest
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(50)]
    public string? name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public ulong? guildId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ulong? roleId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(200)]
    [DataType(DataType.Url)]
    public string? vanityURL { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? webhookLogType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(150)]
    public string? redirectUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(150)]
    public string? pic { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(150)]
    [DataType(DataType.Url)]
    public string? verifyBGImage { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool? vpnCheck { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(150)]
    [DataType(DataType.Url)]
    public string? webhook { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public string? mainBot { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool? captchaCheck { get; set; }
}

