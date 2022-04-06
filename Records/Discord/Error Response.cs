namespace RestoreCord.Records.Discord;

internal record ErrorResponse
{
    public string? message { get; set; }
    public long code { get; set; }
}
