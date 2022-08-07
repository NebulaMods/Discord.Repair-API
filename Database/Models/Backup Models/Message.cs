using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Database.Models.BackupModels;

/// <summary>
/// 
/// </summary>
public class Message
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public Guid key { get; init; } = new();

    /// <summary>
    /// 
    /// </summary>
    public int position { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(1000)]
    public string? content { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? username { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ulong userId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? userPicture { get; set; }
}
