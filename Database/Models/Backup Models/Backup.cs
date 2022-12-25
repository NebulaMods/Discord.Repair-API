using System.ComponentModel.DataAnnotations;

using DiscordRepair.Api.Database.Models.BackupModels.Channel;

namespace DiscordRepair.Api.Database.Models.BackupModels;

/// <summary>
/// 
/// </summary>
public record Backup
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public Guid key { get; init; } = new();

    /// <summary>
    /// 
    /// </summary>
    [Required]
    [StringLength(100)]
    public string name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? guildName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<Role> roles { get; set; } = new HashSet<Role>();

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<CategoryChannel> catgeoryChannels { get; set; } = new HashSet<CategoryChannel>();

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<VoiceChannel> voiceChannels { get; set; } = new HashSet<VoiceChannel>();

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<TextChannel> textChannels { get; set; } = new HashSet<TextChannel>();

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<GuildUser> users { get; set; } = new HashSet<GuildUser>();

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<Emoji> emojis { get; set; } = new HashSet<Emoji>();

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<Sticker> stickers { get; set; } = new HashSet<Sticker>();

    /// <summary>
    /// 
    /// </summary>
    /// 
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime creationDate { get; set; } = DateTime.UtcNow;

    #region Not Important

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? vanityUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? preferredLocale { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int verificationLevel { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int systemChannelMessageDeny { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int defaultMessageNotifications { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int explicitContentFilterLevel { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? splashUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? iconUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(300)]
    public string? description { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? discoverySplashUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool? isWidgetEnabled { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool? isBoostProgressBarEnabled { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? afkTimeout { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? bannerUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Custom)]
    public virtual VoiceChannel? afkChannel { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Custom)]
    public virtual TextChannel? defaultChannel { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Custom)]
    public virtual TextChannel? publicUpdatesChannel { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Custom)]
    public virtual TextChannel? rulesChannel { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Custom)]
    public virtual TextChannel? systemChannel { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Custom)]
    public virtual TextChannel? widgetChannel { get; set; }
    #endregion
}
