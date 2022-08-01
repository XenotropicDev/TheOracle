﻿namespace TheOracle2;

/// <summary>
/// An interface for adapting objects into postable discord messages
/// </summary>
public interface IDiscordEntity
{
    EmbedBuilder? GetEmbed();

    ComponentBuilder? GetComponents();

    bool IsEphemeral { get; set; }

    string? DiscordMessage { get; set; }
}
