using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models;

/// <summary>
/// 
/// </summary>
public record ServerSettings
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public Guid key { get; init; } = new();

    /// <summary>
    /// 
    /// </summary>
    public bool autoKickUnVerified { get; set; } = false;

    /// <summary>
    /// 
    /// </summary>
    public int autoKickUnVerifiedTime { get; set; } = 10;

    /// <summary>
    /// 
    /// </summary>
    public bool autoJoin { get; set; } = false;

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(300)]
    public string? verifyDescription { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int redirectTime { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(150)]
    public string? vanityUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int webhookLogType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool dmOnAutoKick { get; set; } = false;

    /// <summary>
    /// 
    /// </summary>
    public bool autoBlacklist { get; set; } = false;

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
    public string? backgroundImage { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool vpnCheck { get; set; } = false;

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(150)]
    public string? webhook { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Custom)]
    public virtual CustomBot? mainBot { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<Blacklist>? blacklist { get; set; }
}
