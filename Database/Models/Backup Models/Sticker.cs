using System.ComponentModel.DataAnnotations;

using Discord;

namespace DiscordRepair.Api.Database.Models.BackupModels;

/// <summary>
/// 
/// </summary>
public record Sticker
{

    [Key]
    public Guid key { get; init; } = new();

    public ulong packId { get; set; }

    public string name { get; set; }

    public string description { get; set; }

    public string[] tags { get; set; }

    public string? url { get; set; }

    public bool? isAvailable { get; set; }

    public int? sortOrder { get; set; }

    public virtual StickerFormatType format { get; set; }
}
