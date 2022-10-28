using TheOracle2.Data;

namespace TheOracle2;

public class DiscordOracleBuilder : IDiscordEntity
{
    public DiscordOracleBuilder(IEmoteRepository emotes, params OracleRollResult?[] results)
    {
        Results = results.Where(r => r != null).Select(r => r!);
        this.emotes = emotes;
    }

    //Todo: this is the only real non-static member, do we want to make it a method parameter instead?
    private SelectMenuBuilder AddOracleSelect = new SelectMenuBuilder().WithPlaceholder("Add Oracle Roll").WithCustomId("add-oracle-select");

    private readonly IEmoteRepository emotes;

    private IEnumerable<OracleRollResult> Results { get; }
    public bool IsEphemeral { get; set; } = false;
    public string? DiscordMessage { get; set; } = null;

    private static EmbedBuilder GetEmbedBuilder(IEnumerable<OracleRollResult> result)
    {
        var builder = new EmbedBuilder();

        builder.WithAuthor(result.FirstOrDefault()?.Oracle?.Parent?.Name)
            .WithTitle("Oracle Result");

        foreach (var node in result)
        {
            AddFieldsToBuilder(node, builder);
        }

        return builder;
    }

    public static EmbedBuilder AddFieldsToBuilder(OracleRollResult node, EmbedBuilder builder)
    {
        var category = node.Oracle?.Parent?.Categories?.FirstOrDefault(cat => cat.Oracles.Any(o => o.Id == node.Oracle.Id));

        var rollString = (node.Roll != null) ? $" [{node.Roll}]" : string.Empty;
        var catString = category != null ? $" - {category.Name}" : string.Empty;

        builder.AddField($"{node.Oracle?.Name}{catString}{rollString}", node.Description, true);

        if (node.Image != null) builder.WithThumbnailUrl(node.Image);

        foreach (var child in node.ChildResults)
        {
            AddFieldsToBuilder(child, builder);
        }

        return builder;
    }

    private ComponentBuilder? GetComponentBuilder(IEnumerable<OracleRollResult> root)
    {
        var builder = new ComponentBuilder();

        foreach (var result in root)
        {
            AddComponents(builder, result, result);
        }

        if (AddOracleSelect.Options.Count > 0)
        {
            builder.WithSelectMenu(AddOracleSelect);
        }

        if (!builder.ActionRows?.Any(ar => ar.Components.Count > 0) ?? false) return null;

        return builder;
    }

    private ComponentBuilder AddComponents(ComponentBuilder builder, OracleRollResult node, OracleRollResult root)
    {
        foreach (var item in node.FollowUpTables)
        {
            AddOracleSelect.AddOption(item.Name, item.Id, emote: item.Emote);
        }

        foreach (var child in node.ChildResults)
        {
            AddComponents(builder, child, root);
        }

        return builder;
    }

    private bool IsInResultSet(OracleRollResult result, Oracle oracle)
    {
        if (result.Oracle == oracle) return true;

        foreach (var child in result.ChildResults)
        {
            if (IsInResultSet(child, oracle)) return true;
        }
        return false;
    }

    public EmbedBuilder? GetEmbed()
    {
        return GetEmbedBuilder(Results);
    }

    public Task<ComponentBuilder?> GetComponentsAsync()
    {
        return Task.FromResult(GetComponentBuilder(Results));
    }
}
