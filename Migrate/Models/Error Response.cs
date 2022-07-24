namespace RestoreCord.MigrationMaster.Models;

internal record ErrorResponse
{
    public string? message { get; set; }
    public long code { get; set; }
}
