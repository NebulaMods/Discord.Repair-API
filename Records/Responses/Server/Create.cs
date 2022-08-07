namespace DiscordRepair.Records.Responses.Server;

public record CreateServerResponse
{
    public ulong? guildId { get; set; }
    public string name { get; set; }
    public string botName { get; set; }
    public Guid key { get; set; }
    public ulong? roleId { get; set; }
}
