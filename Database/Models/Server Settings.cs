using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Api.Database.Models;

/// <summary>
/// Represents the settings of a Discord server.
/// </summary>
public record ServerSettings
{
    /// <summary>
    /// Gets or sets the key of the server settings.
    /// </summary>
    [Key]
    public Guid key { get; init; } = new();

    /// <summary>
    /// Gets or sets the vanity URL of the server.
    /// </summary>
    [StringLength(150)]
    public string? vanityUrl { get; set; }

    /// <summary>
    /// Gets or sets the webhook log type of the server.
    /// </summary>
    public int webhookLogType { get; set; }

    /// <summary>
    /// Gets or sets the redirect URL of the server.
    /// </summary>
    [StringLength(150)]
    public string? redirectUrl { get; set; }

    /// <summary>
    /// Gets or sets the profile picture URL of the server.
    /// </summary>
    [Required]
    [StringLength(150)]
    public string pic { get; set; } = "https://i.imgur.com/w65Dpnw.png";

    /// <summary>
    /// Gets or sets the background image URL of the server.
    /// </summary>
    [StringLength(150)]
    [DataType(DataType.Url)]
    public string? backgroundImage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether VPN check is enabled or not.
    /// </summary>
    public bool vpnCheck { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether CAPTCHA is enabled or not.
    /// </summary>
    public bool captcha { get; set; } = true;

    /// <summary>
    /// Gets or sets the webhook URL of the server.
    /// </summary>
    [StringLength(150)]
    public string? webhook { get; set; }

    /// <summary>
    /// Gets or sets the main bot associated with the server.
    /// </summary>
    [Required]
    [DataType(DataType.Custom)]
    public virtual CustomBot mainBot { get; set; }

    /// <summary>
    /// Gets or sets the collection of blacklisted users of the server.
    /// </summary>
    public virtual ICollection<Blacklist> blacklist { get; set; } = new HashSet<Blacklist>();

    /// <summary>
    /// Gets or sets the settings for the success verification embed of the server.
    /// </summary>
    public virtual SuccessVerifyEmbedSettings? verifyEmbedSettings { get; set; } = new();
}

/// <summary>
/// A record that contains settings for a success verification embed.
/// </summary>
public record SuccessVerifyEmbedSettings
{
    /// <summary>
    /// The primary key for the success verification embed settings.
    /// </summary>
    [Key]
    public Guid key { get; init; } = new();
    /// <summary>
    /// The name of the author in the success verification embed.
    /// </summary>
    public string authorName { get; set; } = "Discord.Repair";

    /// <summary>
    /// The URL of the icon in the success verification embed.
    /// </summary>
    public string iconUrl { get; set; } = "https://discord.repair/content/images/logo.png";

    /// <summary>
    /// The URL of the icon in the footer of the success verification embed.
    /// </summary>
    public string footerIconUrl { get; set; } = "https://discord.repair/content/images/logo.png";

    /// <summary>
    /// The text in the footer of the success verification embed.
    /// </summary>
    public string footerText { get; set; } = "Discord.Repair";

    /// <summary>
    /// The title of the success verification embed.
    /// </summary>
    public string title { get; set; } = "Verification Success";

    /// <summary>
    /// Whether to include geo data in the success verification embed.
    /// </summary>
    public bool geoData { get; set; } = true;

}