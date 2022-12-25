namespace DiscordRepair.Api.Records.Responses.Server.User;

public record GetGuildUserResponse
{
    public string discordId { get; set; }
    public string guildId { get; set; }
    public string? accessToken { get; set; }
    public string? refreshToken { get; set; }
    public string? ip { get; set; }
    public string? avatar { get; set; }
    public string? username { get; set; }
    public DateTime linkDate { get; set; }
    public string? botClientId { get; set; }
}

public record GetAllGuildUsersResponse
{
    public string discordId { get; set; }
    public string? serverName { get; set; }
    public string? botName { get; set; }
    public string? ip { get; set; }
    public string? username { get; set; }
    public DateTime linkDate { get; set; }
}