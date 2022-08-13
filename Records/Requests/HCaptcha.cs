namespace DiscordRepair.Records.Requests;

public record HCaptchaRequest
{
    public string secret { get; set; }
    public string response { get; set; }
    public string? remoteip { get; set; }
    public string? sitekey { get; set; }
}
