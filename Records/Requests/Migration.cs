namespace DiscordRepair.Api.Records.Requests;

/// <summary>
/// 
/// </summary>
public record MigrationRequest
{
    public string? server { get; set; }
    public string? bot { get; set; }
    public ulong? verifyRoleId { get; set; }
    public ulong? userId { get; set; }
    public int amountToMigrate { get; set; }
    public bool random { get; set; }
}

