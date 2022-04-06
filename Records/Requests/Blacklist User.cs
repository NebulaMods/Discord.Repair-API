namespace RestoreCord.Records.Requests;

/// <summary>
/// 
/// </summary>
public record BlacklistUser
{
    /// <summary>
    /// 
    /// </summary>
    public bool banUser { get; set; } = true;
    /// <summary>
    /// 
    /// </summary>
    public string? reason { get; set; } = "Blacklisted by RestoreCord Bot!";
    /// <summary>
    /// 
    /// </summary>
    public int banPruneDays { get; set; } = 7;
}
