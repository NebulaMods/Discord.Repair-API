namespace DiscordRepair.Records.Discord;

public record User
{
    public ulong id { get; set; }
    public string username { get; set; }
    public string avatar { get; set; }
    public object avatar_decoration { get; set; }
    public string discriminator { get; set; }
    public int public_flags { get; set; }
}
