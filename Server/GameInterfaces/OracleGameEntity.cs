﻿using System.Text.RegularExpressions;
using Server.Data;
using Server.OracleRoller;
using TheOracle2;

namespace Server.GameInterfaces;

public class OracleGameEntity
{
    public OracleGameEntity()
    {
        InitialOracles = new();
        FollowUpOracles = new();
    }

    public string Id { get; set; }
    public string DisplayedDescription { get; set; }
    public List<object> ModalInputs { get; set; }
    public string SearchName { get; set; }
    public IronGame Game { get; set; }
    public string Title { get; set; }
    public List<OracleEntityAction> InitialOracles { get; set; }
    public List<OracleEntityAction> FollowUpOracles { get; set; }
    public string? ShortName { get; set; }
    public string? Author { get; set; }
}

public class OracleEntityData
{
    private readonly IOracleRepository oracles;
    private readonly IOracleRoller roller;

    public OracleEntityData(OracleGameEntity entity, IOracleRepository oracleRepository, IOracleRoller oracleRoller, IEmoteRepository emotes)
    {
        Entity = entity;
        oracles = oracleRepository;
        roller = oracleRoller;
        Title = entity.Title;
        Description = entity.DisplayedDescription;

        InitialOracleData = new();
        FollowupOracleData = new();

        foreach (var o in entity.InitialOracles)
        {
            var oracle = oracles.GetOracleById(o.FieldValue);
            if (oracle == null)
            {
                InitialOracleData.Add(o.ShallowCopy());
                continue;
            }

            var result = roller.GetRollResult(oracle);
            this.AddOracleResult(result, o);

            foreach (var followupItem in result.FollowUpTables)
            {
                FollowupOracleData.Add(followupItem);
            }
        }

        foreach (var f in entity.FollowUpOracles)
        {
            FollowupOracleData.Add(new FollowUpItemAdapter(f, emotes));
        }
    }

    private void AddOracleResult(OracleRollResult result, OracleEntityAction o)
    {
        InitialOracleData.Add(new() {FieldName = o.FieldName, FieldValue = result.Description ?? "Unknown oracle result value"});
        foreach(var childResult in result.ChildResults)
        {
            InitialOracleData.Add(new() {FieldName = childResult.Oracle?.Name ?? o.FieldName, FieldValue = childResult.Description! });

            FollowupOracleData.AddRange(childResult.FollowUpTables);
        }
    }

    public OracleGameEntity Entity { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<OracleEntityAction> InitialOracleData { get; set; }
    public List<FollowUpItem> FollowupOracleData { get; set; }
}

internal class FollowUpItemAdapter : FollowUpItem
{
    private OracleEntityAction entityAction;

    public FollowUpItemAdapter(OracleEntityAction FollowupEntityItem, IEmoteRepository emotes) : base(FollowupEntityItem.FieldValue, FollowupEntityItem.FieldName, emotes)
    {
        this.entityAction = FollowupEntityItem;
    }
}

public class OracleEntityAdapter : IDiscordEntity
{
    private readonly OracleGameEntity gameEntity;
    private readonly IOracleRepository oracles;
    private readonly IOracleRoller roller;
    private readonly IEmoteRepository emotes;

    public OracleEntityAdapter(OracleGameEntity gameEntity, IOracleRepository oracleRepository, IOracleRoller roller, IEmoteRepository emotes)
    {
        this.gameEntity = gameEntity;
        this.oracles = oracleRepository;
        this.roller = roller;
        this.emotes = emotes;
    }

    public bool IsEphemeral { get; set; }
    public string? DiscordMessage { get; set; } = null;

    public Task<ComponentBuilder?> GetComponentsAsync()
    {
        var componentBuilder = new ComponentBuilder();
        var selectMenu = new SelectMenuBuilder().WithPlaceholder($"Add Oracle To {gameEntity.ShortName ?? gameEntity.Title}").WithCustomId("add-oracle-select");
        foreach (var item in gameEntity.FollowUpOracles)
        {
            var emoji = (item.Emoji != null) ? new Emoji(item.Emoji) : null;
            selectMenu.AddOption(item.FieldName, item.FieldValue.Replace("[[", "").Replace("]]", ""), emote: emoji ?? emotes.Roll);
        }

        if (selectMenu.Options.Count > 0) componentBuilder.WithSelectMenu(selectMenu);

        return Task.FromResult(componentBuilder);
    }

    public EmbedBuilder? GetEmbed()
    {
        var builder = new EmbedBuilder().WithTitle(gameEntity.Title);
        
        if (gameEntity.Title.Contains("[["))
        {
            var newTitle = ReplaceValuesWithOracleResults(gameEntity.Title);

            builder.WithTitle(newTitle);
        }

        foreach (var fieldTemplate in gameEntity.InitialOracles)
        {
            var fieldText = ReplaceValuesWithOracleResults(fieldTemplate.FieldValue);

            builder.AddField(fieldTemplate.FieldName, fieldText, true);
        }

        builder.WithAuthor(gameEntity.Author);

        return builder;
    }

    private string ReplaceValuesWithOracleResults(string value)
    {
        var oracleMatch = Regex.Matches(value, @"\[\[(.+?)\]\]");

        foreach (var capturedOracle in oracleMatch.Cast<Match>())
        {
            var oracle = oracles.GetOracleById(capturedOracle.Groups[1].Value);
            if (oracle == null)
            {
                continue;
            }
            var result = roller.GetRollResult(oracle);

            var location = value.IndexOf(capturedOracle.Value);

            //replace only the first occurrence of the matching value (just incase people want to make a field that uses multiple values from the same oracle table)
            value = value.Remove(location, capturedOracle.Value.Length).Insert(location, result.Description ?? result.ChildResults.ElementAt(Random.Shared.Next(0, result.ChildResults.Count)).Description ?? "");
        }

        return value;
    }
}

public class OracleEntityAction
{
    public OracleEntityAction()
    {
        ModalSettings = new ModalOverrideSettings();
        FieldName = String.Empty;
        FieldValue = String.Empty;
    }

    public OracleEntityAction ShallowCopy()
    {
        var clone = (OracleEntityAction)this.MemberwiseClone();
        clone.ModalSettings = null;

        return clone;
    }

    public string FieldName { get; set; }
    public string FieldValue { get; set; }
    public bool HasModalOverride { get; set; }
    public ModalOverrideSettings? ModalSettings { get; set; }
    public bool AllowReroll { get; set; }
    public bool AllowFudge { get; set; }
    public string? Emoji { get; set; }
}

public class ModalOverrideSettings
{
    public ModalOverrideSettings()
    {
        TextInputStyle = TextInputStyle.Short;
        PlaceholderText = String.Empty;
    }

    public TextInputStyle TextInputStyle { get; internal set; }
    public string PlaceholderText { get; internal set; }
    public int? MaxLength { get; internal set; }
    public int? MinLength { get; internal set; }
}
