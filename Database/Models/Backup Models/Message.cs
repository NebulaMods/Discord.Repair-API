using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

using Discord.Rest;

using Discord;
using DiscordRepair.Api.Database.Models.BackupModels.Channel;

namespace DiscordRepair.Api.Database.Models.BackupModels;

/// <summary>
/// 
/// </summary>
public class Message
{
    [Key]
    public Guid key { get; init; } = new();

    public string? content { get; set; }

    public ulong authorId { get; set; }

    public virtual TextChannel channel { get; set; }

    public virtual MessageSource source { get; set; }

    public DateTimeOffset createdAt { get; set; }

    public bool isTTS {get; set; }

    public bool isPinned {get; set; }

    public bool isSuppressed {get; set; }

    public ulong? threadChannelId { get; set; }

    //public virtual ICollection<Attachment>? attachments {get; set;}

    //public virtual ICollection<Embed>? embeds {  get; set; }

    //public virtual MessageApplication application { get; private set; }

    public virtual MessageFlags? flags { get; set; }

    public virtual MessageType type { get; set; }

    //public virtual ICollection<ActionRowComponent>? components { get; set; }
}
