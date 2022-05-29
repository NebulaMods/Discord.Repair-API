using System.ComponentModel.DataAnnotations;
using RestoreCord.Database.Models.BackupModels.Channel;

namespace RestoreCord.Database.Models.BackupModels;

public class Backup
{
    [Key]
    public int key { get; set; }
    public string? guildName { get; set; }
    public virtual ICollection<Role> roles { get; set; } = new HashSet<Role>();
    public virtual ICollection<CategoryChannel> catgeoryChannels { get; set; } = new HashSet<CategoryChannel>();
    public virtual ICollection<VoiceChannel> voiceChannels { get; set; } = new HashSet<VoiceChannel>();
    public virtual ICollection<TextChannel> textChannels { get; set; } = new HashSet<TextChannel>();
    public virtual ICollection<GuildUser> users { get; set; } = new HashSet<GuildUser>();
    public virtual ICollection<Emoji> emojis { get; set; } = new HashSet<Emoji>();
    public DateTime creationDate { get; set; }

    #region Not Important
    public string? vanityUrl { get; set; }
    public string? preferredLocale { get; set; }
    public int verificationLevel { get; set; }
    public int systemChannelMessageDeny { get; set; }
    public int defaultMessageNotifications { get; set; }
    public int explicitContentFilterLevel { get; set; }
    public string? splashUrl { get; set; }
    public string? iconUrl { get; set; }
    public string? description { get; set; }
    public string? discoverySplashUrl { get; set; }
    public bool? isWidgetEnabled { get; set; }
    public bool? isBoostProgressBarEnabled { get; set; }
    public int? afkTimeout { get; set; }
    public string? bannerUrl { get; set; }
    public virtual VoiceChannel? afkChannel { get; set; }
    public virtual TextChannel? defaultChannel { get; set; }
    public virtual TextChannel? publicUpdatesChannel { get; set; }
    public virtual TextChannel? rulesChannel { get; set; }
    public virtual TextChannel? systemChannel { get; set; }
    public virtual TextChannel? widgetChannel { get; set; }
    #endregion
}
