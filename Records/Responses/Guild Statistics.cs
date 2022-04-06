namespace RestoreCord.Records.Responses;

/// <summary>
/// 
/// </summary>
public record GuildStatistics
{
    /// <summary>
    /// 
    /// </summary>
    public Utilities.OldMigration.GuildStatisitics? guildStats { get; set; }
    /// <summary>
    /// /
    /// </summary>
    public Utilities.OldMigration.MemberStatisitics? memberStats { get; set; }
}
