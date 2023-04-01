using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Api.Records.Requests.Server;

public record CreateServerRequest
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(50)]
    [Required]
    public string name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [Required]
    public ulong guildId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ulong? roleId { get; set; }

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
    [Required]
    [StringLength(150)]
    public string pic { get; set; } = "https://i.imgur.com/w65Dpnw.png";

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
    public bool vpnCheck { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public bool captchaCheck { get; set; } = true;

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
    [Required]
    public string mainBot { get; set; }

    [StringLength(200)]
    [DataType(DataType.Url)]
    public string? vanityURL { get; set; }
}
