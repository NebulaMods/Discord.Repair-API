namespace DiscordRepair.Api.MigrationMaster.Models;

internal record UserResponse
{
    public string? id { get; set; }
    public string? username { get; set; }
    public string? avatar { get; set; }
    public long discriminator { get; set; }
    public long public_flags { get; set; }
    public long flags { get; set; }
    public string? locale { get; set; }
    public bool mfa_enabled { get; set; }
    public long premium_type { get; set; }
}
