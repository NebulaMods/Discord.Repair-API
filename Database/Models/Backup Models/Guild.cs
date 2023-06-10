using System.ComponentModel.DataAnnotations;

using DiscordRepair.Api.Database.Models.BackupModels.Channel;

namespace DiscordRepair.Api.Database.Models.BackupModels;

public record Guild
{
    [Key]
    public Guid key { get; init; } = new();

    [StringLength(100)]
    public string? guildName { get; set; }
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
}
