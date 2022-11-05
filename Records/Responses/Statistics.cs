namespace DiscordRepair.Records.Responses;

public record StatsResponse
{
    public int serverCount { get; set; }
    public int userCount { get; set; }
    public int linkedMemberCount { get; set; }
    public int backupCount { get; set; }
    public int backedupRoleCount { get; set; }
    public int backedupChannelCount { get; set; }
    public int backedupMemberCount { get; set; }
}
