using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models.Statistics;

public class MemberMigration
{
    [Key]
    public int key { get; set; }
    public DateTime startTime { get; set; } = DateTime.Now;
    public TimeSpan totalTime { get; set; }
    public int successCount { get; set; }
    public int bannedCount { get; set; }
    public int tooManyGuildsCount { get; set; }
    public int invalidTokenCount { get; set; }
    public int alreadyHereCount { get; set; }
    public int failedCount { get; set; }
    public int totalCount { get; set; }
    public int blacklistedCount { get; set; }
    public DateTime estimatedCompletionTime { get; set; }
}
