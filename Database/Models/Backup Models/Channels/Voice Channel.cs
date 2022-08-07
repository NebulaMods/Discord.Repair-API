using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Database.Models.BackupModels.Channel;

/// <summary>
/// 
/// </summary>
public record VoiceChannel
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public Guid key { get; init; } = new();

    /// <summary>
    /// 
    /// </summary>
    public ulong id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? userLimit { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int bitrate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(50)]
    public string? region { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(50)]
    public string? videoQuality { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int position { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Custom)]
    public virtual CategoryChannel? category { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool synced { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<Permissions.ChannelPermissions> permissions { get; set; } = new HashSet<Permissions.ChannelPermissions>();
}
