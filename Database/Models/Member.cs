﻿using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models;

/// <summary>
/// 
/// </summary>
public record Member
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public Guid key { get; init; } = new();

    /// <summary>
    /// 
    /// </summary>
    /// 
    [Required]
    public ulong discordId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Custom)]
    public virtual Server? server { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? accessToken { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? refreshToken { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? ip { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? avatar { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? username { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ulong? creationDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Custom)]
    public virtual CustomBot? botUsed { get; set; }
}
