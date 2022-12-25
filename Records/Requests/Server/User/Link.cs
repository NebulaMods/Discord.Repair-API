using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Api.Records.Requests.Server.User;

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
    [Required]
    [StringLength(100)]
    public string bot { get; set; }
}

