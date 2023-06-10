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

    [Required]
    public virtual BackupType type { get; set; }

    public ulong guildId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public virtual Guild? guild { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<Role>? roles { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<CategoryChannel>? catgeoryChannels { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<VoiceChannel>? voiceChannels { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<TextChannel>? textChannels { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<GuildUser>? users { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<Emoji>? emojis { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<Sticker>? stickers { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<Message>? messages { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime creationDate { get; set; } = DateTime.UtcNow;

}
public enum BackupType
{
    FULL,
    EMOJIS,
    CHANNELS,
    USER_ROLES,
    ROLES,
    MESSAGES,
    STICKERS,
    GUILD
}
