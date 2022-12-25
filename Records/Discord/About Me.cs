namespace DiscordRepair.Api.Records.Discord;

public record AboutMe
{
    public Application application { get; set; }
    public List<string> scopes { get; set; }
    public DateTime expires { get; set; }
    public User user { get; set; }
}
