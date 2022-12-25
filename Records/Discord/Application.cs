namespace DiscordRepair.Api.Records.Discord;

public record Application
{
    public string id { get; set; }
    public string name { get; set; }
    public string icon { get; set; }
    public string description { get; set; }
    public string summary { get; set; }
    public object type { get; set; }
    public bool hook { get; set; }
    public bool bot_public { get; set; }
    public bool bot_require_code_grant { get; set; }
    public string verify_key { get; set; }
}
