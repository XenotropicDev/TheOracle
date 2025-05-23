using Server.DiscordServer;
using Server.GameInterfaces;
using Server.GameInterfaces.DTOs; // Added for DTOs
using System.Collections.Generic; // Required for List
using System.Linq; // Required for .IndexOf

namespace TheOracle2.UserContent;

public class DiscordAssetEntity : IDiscordEntity
{
    public DiscordAssetEntity(AssetDTO assetDto, AssetData data) // Changed parameter to AssetDTO
    {
        AssetDto = assetDto; // Assign to the new field
        Data = data;
    }

    public AssetDTO AssetDto { get; } // Renamed field and changed type to AssetDTO
    public AssetData Data { get; }
    public bool IsEphemeral { get; set; } = false;
    public string? DiscordMessage { get; set; }

    public EmbedBuilder? GetEmbed()
    {
        EmbedBuilder builder = new EmbedBuilder()
            // Use AssetDto.Source.Name or AssetDto.AssetType for author
            .WithAuthor($"Asset: {AssetDto.Source?.Name ?? AssetDto.AssetType}") 
            .WithTitle(AssetDto.Name); // Use AssetDto

        if (Data.ThumbnailURL != null)
        {
            builder.WithThumbnailUrl(Data.ThumbnailURL);
        }

        // Use AssetDto.Inputs. Assuming Data.Inputs corresponds by index.
        for (var i = 0; i < AssetDto.Inputs?.Count; i++) 
        {
            builder.AddField(AssetDto.Inputs[i].Name, Data.Inputs[i]);
        }

        if (AssetDto.ConditionMeter != null) // Use AssetDto
        {
            builder.AddField(AssetDto.ConditionMeter.Name, Data.ConditionValue, false);
        }

        string description = string.Empty;
        // Use AssetDto.Abilities and AbilityDTO
        foreach (var ability in AssetDto.Abilities ?? new List<AbilityDTO>()) 
        {
            if (Data.SelectedAbilities.Contains(ability.Id))
            {
                description += $"⬢ {ability.Text}\n\n";
            }
        }
        builder.WithDescription(DiscordHelpers.FormatMarkdownLinks(description));

        return builder;
    }

    public Task<ComponentBuilder?> GetComponentsAsync()
    {
        ComponentBuilder compBuilder = new ComponentBuilder();

        if (AssetDto.Abilities != null && AssetDto.Abilities.Any()) // Use AssetDto
        {
            var select = new SelectMenuBuilder()
                .WithCustomId($"asset-ability-select:{Data.Id}")
                .WithPlaceholder($"Ability Selection")
                .WithMinValues(0)
                .WithMaxValues(AssetDto.Abilities.Count); // Use AssetDto

            // Use AssetDto.Abilities and AbilityDTO
            foreach (var ability in AssetDto.Abilities ?? new List<AbilityDTO>()) 
            {
                select.AddOption(new SelectMenuOptionBuilder()
                // Using IndexOf requires AssetDto.Abilities to be a List or array.
                .WithLabel($"Ability {AssetDto.Abilities.IndexOf(ability) + 1}") 
                .WithValue(ability.Id)
                .WithDefault(Data.SelectedAbilities.Contains(ability.Id))
                );
            }

            compBuilder.WithSelectMenu(select);
        }

        if (AssetDto.ConditionMeter != null) // Use AssetDto
        {
            var select = new SelectMenuBuilder()
                .WithCustomId($"asset-condition-select:{Data.Id}")
                .WithPlaceholder($"{AssetDto.ConditionMeter.Name} actions") // Use AssetDto
                .WithMinValues(1)
                .WithMaxValues(1)
                // Use AssetDto.ConditionMeter.Name
                .AddOption(new SelectMenuOptionBuilder().WithLabel($"+1 {AssetDto.ConditionMeter.Name}").WithValue("asset-condition-up"))
                .AddOption(new SelectMenuOptionBuilder().WithLabel($"-1 {AssetDto.ConditionMeter.Name}").WithValue("asset-condition-down"))
                .AddOption(new SelectMenuOptionBuilder().WithLabel($"Roll {AssetDto.ConditionMeter.Name}").WithValue("asset-condition-roll"));
            
            // ConditionMeterDTO does not have a 'Conditions' list like the original Asset.ConditionMeter.
            // The loop for Asset.ConditionMeter.Conditions has been removed.

            compBuilder.WithSelectMenu(select);
        }

        return Task.FromResult(compBuilder);
    }
}
