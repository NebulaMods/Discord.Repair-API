namespace DiscordRepair.Api.MigrationMaster.Models;

internal record TokenResponse
{
    public string? access_token { get; set; }
    public long expires_in { get; set; }
    public string? refresh_token { get; set; }
    public string? scope { get; set; }
    public string? token_type { get; set; }
}
