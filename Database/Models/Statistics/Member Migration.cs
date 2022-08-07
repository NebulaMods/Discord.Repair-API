using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Database.Models.Statistics;

/// <summary>
/// 
/// </summary>
public record MemberMigration
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
    public DateTime startTime { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Time)]
    public TimeSpan totalTime { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int successCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int bannedCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int tooManyGuildsCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int invalidTokenCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int alreadyHereCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int failedCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int totalCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int blacklistedCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.DateTime)]
    public DateTime estimatedCompletionTime { get; set; }
}
