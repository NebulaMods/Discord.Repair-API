using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Api.Database.Models.LogModels;

/// <summary>
/// Represents a recorded error that occurred in the system.
/// </summary>
public record Errors
{
    /// <summary>
    /// The unique identifier of the error record.
    /// </summary>
    [Key]
    public Guid key { get; init; } = new();

    /// <summary>
    /// The location where the error occurred.
    /// </summary>
    [StringLength(50)]
    public string? location { get; set; }

    /// <summary>
    /// The error message that was logged.
    /// </summary>
    [StringLength(200)]
    public string? message { get; set; }

    /// <summary>
    /// The stack trace of the error.
    /// </summary>
    [StringLength(300)]
    public string? stackTrace { get; set; }

    /// <summary>
    /// Additional information about the error.
    /// </summary>
    [StringLength(300)]
    public string? extraInfo { get; set; }

    /// <summary>
    /// The date and time when the error occurred.
    /// </summary>
    [DataType(DataType.DateTime)]
    public DateTime errorTime { get; init; } = DateTime.UtcNow;

}
