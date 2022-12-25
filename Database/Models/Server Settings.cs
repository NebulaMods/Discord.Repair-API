using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Api.Database.Models;

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
    public string? backgroundImage { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool vpnCheck { get; set; } = false;

    /// <summary>
    /// 
    /// </summary>
    public bool captcha { get; set; } = true;

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
    [Required]
    [DataType(DataType.Custom)]
    public virtual CustomBot mainBot { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<Blacklist> blacklist { get; set; } = new HashSet<Blacklist>();

    /// <summary>
    /// 
    /// </summary>
    public virtual SuccessVerifyEmbedSettings? verifyEmbedSettings { get; set; } = new();
}

/// <summary>
/// 
/// </summary>
public record SuccessVerifyEmbedSettings
{
    [Key]
    public Guid key { get; init; } = new();
    public string authorName { get; set; } = "Discord.Repair";
    public string iconUrl { get; set; } = "https://discord.repair/content/images/logo.png";
    public string footerIconUrl { get; set; } = "https://discord.repair/content/images/logo.png";
    public string footerText { get; set; } = "Discord.Repair";
    public string title { get; set; } = "Verification Success";
    public bool geoData { get; set; } = true;
}