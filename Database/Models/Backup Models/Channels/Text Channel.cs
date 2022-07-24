using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models.BackupModels.Channel;

/// <summary>
/// 
/// </summary>
public record TextChannel
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
    public int slowModeInterval { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? topic { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Custom)]
    public virtual CategoryChannel? category { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool nsfw { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? archiveAfter { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int position { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool locked { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool archived { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool synced { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<Permissions.ChannelPermissions> permissions { get; set; } = new HashSet<Permissions.ChannelPermissions>();

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<Message> messages { get; set; } = new HashSet<Message>();

}
