namespace DiscordRepair.Api.Records.Responses;

public record ReCaptchaResponse
{
    public bool success { get; set; }
    public string hostname { get; set; }
    public DateTime challenge_ts { get; set; }
}
