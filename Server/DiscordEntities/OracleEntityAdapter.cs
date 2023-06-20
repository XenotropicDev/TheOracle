using System.Text.RegularExpressions;
using Server.Data;
using Server.OracleRoller;
using Dataforged;
using TheOracle2;

namespace Server.GameInterfaces;

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
