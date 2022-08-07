using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Records.Requests.Guild.User;

public record Link
{
    [Required]
    [StringLength(100)]
    public string accessToken { get; set; }
    [Required]
    [StringLength(100)]
    public string refreshToken { get; set; }
    public string? ip { get; set; }
    public string? avatar { get; set; }
    public string? username { get; set; }
    public ulong? creationDate { get; set; }
    [Required]
    [StringLength(100)]
    public string bot { get; set; }
}

