using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models.Statistics;

public class GuildMigration
{
    [Key]
    public int key { get; set; }
    public DateTime startTime { get; set; } = DateTime.Now;
    public TimeSpan totalTime { get; set; }
}
