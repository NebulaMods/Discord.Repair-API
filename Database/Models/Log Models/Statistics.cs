using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models.LogModels;

/// <summary>
/// 
/// </summary>
public record Statistics
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public Guid key { get; init; } = new();

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Custom)]
    public virtual Server? server { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ulong guildId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Custom)]
    public virtual User? MigratedBy { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool active { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.DateTime)]
    public DateTime startDate { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.DateTime)]
    public DateTime? endDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Custom)]
    public virtual Models.Statistics.MemberMigration? memberStats { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Custom)]
    public virtual Models.Statistics.GuildMigration? guildStats { get; set; }
}
