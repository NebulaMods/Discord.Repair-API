namespace DiscordRepair.Api.Records.Responses.Server;

/// <summary>
/// 
/// </summary>
public record Statistics
{
    public string? server { get; set; }
    public ulong guildId { get; set; }
    public string? MigratedBy { get; set; }
    public bool active { get; set; }
    public DateTime startDate { get; set; }
    public DateTime? endDate { get; set; }
    public virtual Database.Models.Statistics.MemberMigration? memberStats { get; set; }
    public virtual Database.Models.Statistics.GuildMigration? guildStats { get; set; }
}
