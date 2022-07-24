using Discord;

namespace RestoreCord.Records.Requests.Guild;

public record Message
{
    public string? text { get; set; }
    public VerifyMessage? verifyMessage { get; set; }
    public bool isTts { get; set; } = false;
    public Embed[]? embeds { get; set; }
    public MessageComponent? component { get; set; }
}

public record VerifyMessage
{
    public string? embedColour { get; set; }
    public string? imageUrl { get; set; }
    public string? embedDescription { get; set; }
}
