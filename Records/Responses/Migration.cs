namespace RestoreCord.Records.Responses;

/// <summary>
/// 
/// </summary>
public record Migration
{
    public Enums response { get; set; }
    public Utilities.OldMigration.GuildStatisitics? guildStats { get; set; }
    public Utilities.OldMigration.MemberStatisitics? memberStats { get; set; }
}
