namespace RestoreCord.Records.Responses;

/// <summary>
/// 
/// </summary>
public record Migration
{
    public bool success { get; set; }
    public string? details { get; set; }
    public Database.Models.Statistics.GuildMigration? guildStats { get; set; }
    public Database.Models.Statistics.MemberMigration? memberStats { get; set; }
}
