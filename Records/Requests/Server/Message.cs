namespace DiscordRepair.Api.Records.Requests.Server;

public record Message
{
    public string? text { get; set; }
    public VerifyMessage verifyMessage { get; set; }
    public bool isTts { get; set; } = false;
    //public Embed[]? embeds { get; set; }
    //public MessageComponent? component { get; set; }
}

public record VerifyMessage
{
    public string embedColour { get; set; } = "random";
    public string? imageUrl { get; set; }
    public string embedDescription { get; set; } = "\"Click the \\\"Verify\\\" button and press Authorize to view the rest of the server\"";
    public string footerIconUrl { get; set; } = "https://discord.repair/content/logo.png";
    public string footerText { get; set; } = "Discord.Repair";
    public string title { get; set; } = "Verification";
}
