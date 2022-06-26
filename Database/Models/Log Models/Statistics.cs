using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models.LogModels;

/// <summary>
/// 
/// </summary>
public class Statistics
{
    [Key]
    public int key { get; set; }
    public virtual Server server { get; set; }
    public ulong guildId { get; set; }
    public virtual User MigratedBy { get; set; }
    public bool active { get; set; }
    public DateTime startDate { get; set; } = DateTime.Now;
    public DateTime? endDate { get; set; }
    public virtual Models.Statistics.MemberMigration? memberStats { get; set; }
    public virtual Models.Statistics.GuildMigration? guildStats { get; set; }
}
