namespace DiscordRepair.Records.Responses.Guild.User;

public record GetUserResponse
{
    public ulong discordId { get; set; }
    public ulong guildId { get; set; }
    public string? accessToken { get; set; }
    public string? refreshToken { get; set; }
    public string? ip { get; set; }
    public string? avatar { get; set; }
    public string? username { get; set; }
    public ulong? creationDate { get; set; }
    public string? botClientId { get; set; }
}
