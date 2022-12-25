using Newtonsoft.Json;

namespace DiscordRepair.Api.Records.Responses;

public record HCaptchaResponse
{
    public bool success { get; set; }
    public DateTime? challenge_ts { get; set; }
    public string? hostname { get; set; }
    [JsonProperty("error-codes")]
    public List<string?>? ErrorCodes { get; set; }
    public bool? credit { get; set; }
}
