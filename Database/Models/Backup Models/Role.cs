using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Api.Database.Models.BackupModels;

/// <summary>
/// 
/// </summary>
public record Role
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
    public string? name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public string? icon { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public uint color { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool isHoisted { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool isManaged { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool isMentionable { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int position { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool isEveryone { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Custom)]
    public virtual Permissions.RolePermissions permissions { get; set; }
}
