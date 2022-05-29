using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models.LogModels;

/// <summary>
/// 
/// </summary>
public class Statistics
{
    [Key]
    public int key { get; set; }
    public int serverId { get; set; }
    public ulong guildId { get; set; }
    public string? MigratedBy { get; set; }
    public bool active { get; set; }
    public virtual Models.Statistics.MemberMigration? memberStats { get; set; }
    public virtual Models.Statistics.GuildMigration? guildStats { get; set; }
}
