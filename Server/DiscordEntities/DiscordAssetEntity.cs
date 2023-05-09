using System.Collections.ObjectModel;
using Server.DiscordServer;
using Server.GameInterfaces;
using TheOracle2.Data;

namespace TheOracle2.UserContent;

public class DiscordAssetEntity : IDiscordEntity
{
    public DiscordAssetEntity(Asset asset, AssetData data)
    {
        Asset = asset;
        Data = data;
    }

    public Asset Asset { get; }
    public AssetData Data { get; }
    public bool IsEphemeral { get; set; } = false;
    public string? DiscordMessage { get; set; }

    public EmbedBuilder? GetEmbed()
    {
        EmbedBuilder builder = new EmbedBuilder()
            .WithAuthor($"Asset: {Asset.Parent?.Name ?? Asset.AssetType[Asset.AssetType.LastIndexOf("/")..]}")
            .WithTitle(Asset.Name);

        if (Data.ThumbnailURL != null)
        {
            builder.WithThumbnailUrl(Data.ThumbnailURL);
        }

        for (var i = 0; i < Asset.Inputs?.Count; i++)
        {
            builder.AddField(Asset.Inputs[i].Name, Data.Inputs[i]);
        }

        if (Asset.ConditionMeter != null)
        {
            builder.AddField(Asset.ConditionMeter.Name, Data.ConditionValue, false);
        }

        string description = string.Empty;
        foreach (var ability in Asset.Abilities ?? new ObservableCollection<Ability>())
        {
            if (Data.SelectedAbilities.Contains(ability.JsonId))
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

        if (Asset.Abilities != null)
        {
            var select = new SelectMenuBuilder()
                .WithCustomId($"asset-ability-select:{Data.Id}")
                .WithPlaceholder($"Ability Selection")
                .WithMinValues(0)
                .WithMaxValues(Asset.Abilities.Count);

            foreach (var ability in Asset.Abilities ?? new ObservableCollection<Ability>())
            {
                select.AddOption(new SelectMenuOptionBuilder()
                .WithLabel($"Ability {Asset.Abilities!.IndexOf(ability) + 1}")
                .WithValue(ability.JsonId)
                .WithDefault(Data.SelectedAbilities.Contains(ability.JsonId))
                );
            }

            compBuilder.WithSelectMenu(select);
        }

        if (Asset.ConditionMeter != null)
        {
            //todo: show condition in select?
            var select = new SelectMenuBuilder()
                .WithCustomId($"asset-condition-select:{Data.Id}")
                .WithPlaceholder($"{Asset.ConditionMeter.Name} actions")
                .WithMinValues(1)
                .WithMaxValues(1)
                .AddOption(new SelectMenuOptionBuilder().WithLabel($"+1 {Asset.ConditionMeter.Name}").WithValue("asset-condition-up"))
                .AddOption(new SelectMenuOptionBuilder().WithLabel($"-1 {Asset.ConditionMeter.Name}").WithValue("asset-condition-down"))
                .AddOption(new SelectMenuOptionBuilder().WithLabel($"Roll {Asset.ConditionMeter.Name}").WithValue("asset-condition-roll"));

            if (Asset.ConditionMeter.Conditions?.Count > 0)
            {
                foreach (var condition in Asset.ConditionMeter.Conditions)
                {
                    select.AddOption(new SelectMenuOptionBuilder().WithLabel($"Toggle Condition: {condition}").WithValue($"condition:{condition}"));
                }
            }

            compBuilder.WithSelectMenu(select);
        }

        return Task.FromResult(compBuilder);
    }
}
