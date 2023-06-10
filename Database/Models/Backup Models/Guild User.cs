using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Api.Database.Models.BackupModels;

/// <summary>
/// 
/// </summary>
public record GuildUser
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public Guid key { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    public ulong id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public string? username { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public string? avatarUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<Role> assignedRoles { get; set; } = new HashSet<Role>();
}
