using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models.Statistics;

/// <summary>
/// 
/// </summary>
public record GuildMigration
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
    [DataType(DataType.DateTime)]
    public DateTime startTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Time)]
    public TimeSpan totalTime { get; set; }
}
