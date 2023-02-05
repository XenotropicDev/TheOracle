using Discord.Interactions;
using Discord.WebSocket;
using Server.Data;
using Server.Interactions.Helpers;
using Server.OracleRoller;
using TheOracle2.Commands;
using TheOracle2.Data;

namespace TheOracle2;

public class OracleCommand : InteractionModuleBase
{
    private readonly IOracleRoller roller;
    private readonly IEmoteRepository emotes;

    public OracleCommand(IServiceProvider services)
    {
        Oracles = services.GetRequiredService<IOracleRepository>();
        this.roller = services.GetRequiredService<IOracleRoller>();
        this.emotes = services.GetRequiredService<IEmoteRepository>();
    }

    public IOracleRepository Oracles { get; }

    [SlashCommand("oracle", "Roll on an oracle table. To ask a yes/no question, use /ask.")]
    public async Task RollOracle([Autocomplete(typeof(OracleAutocomplete))] string oracle, 
        [Summary(description: "Optional second oracle to roll")]
        [Autocomplete(typeof(OracleAutocomplete))] string secondOracle = null)
    {
        var firstOracle = Oracles.GetOracleById(oracle);
        if (firstOracle == null) throw new ArgumentException($"Unknown oracle: {oracle}");
        var rollResult = roller.GetRollResult(firstOracle);

        OracleRollResult? secondResult = null;
        if (secondOracle != null)
        {
            var secondOracleData = Oracles.GetOracleById(secondOracle);
            if (secondOracleData == null) throw new ArgumentException($"Unknown oracle: {secondOracle}");
            secondResult = roller.GetRollResult(secondOracleData);
        }

        var entityItem = new DiscordOracleBuilder(emotes, rollResult, secondResult);
        await RespondAsync(embeds: entityItem.AsEmbedArray(), ephemeral: entityItem.IsEphemeral, components: await entityItem.AsMessageComponent());
    }
}

public class OracleComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>> 
{
    private readonly IOracleRoller roller;
    private readonly IEmoteRepository emotes;

    public OracleComponents(IServiceProvider services)
    {
        OracleRepo = services.GetRequiredService<IOracleRepository>();
        this.roller = services.GetRequiredService<IOracleRoller>();
        this.emotes = services.GetRequiredService<IEmoteRepository>();
    }

    public IOracleRepository OracleRepo { get; }

    [ComponentInteraction("add-oracle-select")]
    public async Task FollowUp(string[] values)
    {
        var builder = Context.Interaction.Message.Embeds.FirstOrDefault().ToEmbedBuilder();
        foreach (var oracleId in values)
        {
            var oracle = OracleRepo.GetOracleById(oracleId);
            if (oracle == null) throw new ArgumentException($"Unknown oracle: {oracleId}");
            var rollResult = roller.GetRollResult(oracle);

            builder = DiscordOracleBuilder.AddFieldsToBuilder(rollResult, builder);
        }

        var comp = ComponentBuilder.FromMessage(Context.Interaction.Message).RemoveSelectionOptions(values);

        await Context.Interaction.UpdateAsync(msg =>
        {
            msg.Embeds = new Embed[] { builder.Build() };
            msg.Components = comp.Build();

        }).ConfigureAwait(false);
    }

    [ComponentInteraction("roll-oracle:*")]
    public async Task RollOracle(string oracleName)
    {
        var firstOracle = OracleRepo.GetOracleById(oracleName);
        if (firstOracle == null) throw new ArgumentException($"Unknown oracle: {oracleName}");
        var rollResult = roller.GetRollResult(firstOracle);

        var entityItem = new DiscordOracleBuilder(emotes, rollResult);
        await RespondAsync(embeds: entityItem.AsEmbedArray(), ephemeral: entityItem.IsEphemeral, components: await entityItem.AsMessageComponent());
    }
}
