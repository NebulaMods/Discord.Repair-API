using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Api.Database.Models.BackupModels;

public record Guild
{
    [Key]
    public Guid key { get; init; } = new();

    public string? guildName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// 
    public string? vanityUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
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
    public string? splashUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public string? iconUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public string? description { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
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
    public string? bannerUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public ulong? afkChannelId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public ulong? defaultChannelId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public ulong? publicUpdatesChannelId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public ulong? rulesChannelId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public ulong? systemChannelId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public ulong? widgetChannelId { get; set; }
}
