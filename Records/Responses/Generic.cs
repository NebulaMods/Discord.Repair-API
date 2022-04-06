namespace RestoreCord.Records.Responses;

/// <summary>
/// 
/// </summary>
public record GenericResponse
{
    /// <summary>
    /// 
    /// </summary>
    public bool success { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? details { get; set; }
}
