using System.Numerics;
using Server.Data;
using Server.DiscordServer;
using Server.GameInterfaces;
using TheOracle2;
using TheOracle2.GameObjects;

namespace Server.DiceRoller;

public interface IActionRoll : IMatchable, IDiscordEntity
{
}

public class ActionRollRandom : IActionRoll, IBurnable
{
    private List<int> ActionAdds;
    private string description;
    private readonly int? characterId;
    private readonly string? jumpURL;
    private readonly IEmoteRepository emotes;
    private readonly PlayerDataFactory dataFactory;
    private readonly ApplicationContext? db;
    private readonly ulong playerId;
    private int? momentum;

    public ActionRollRandom(Random random, IEmoteRepository emotes, PlayerDataFactory pdf, ApplicationContext? db, ulong PlayerId, int stat, int adds, int? momentum = null, string description = "", int? actionDie = null, int? challengeDie1 = null, int? challengeDie2 = null, int? characterId = null, string? jumpURL = null, string? statName = null) : base()
    {
        this.emotes = emotes;
        this.dataFactory = pdf;
        this.db = db;
        playerId = PlayerId;
        this.momentum = momentum;
        this.description = description;
        this.characterId = characterId;
        this.jumpURL = jumpURL;
        StatName = statName;
        Name = "Action Roll";

        Action = new DieRandom(random, 6, actionDie);
        Challenge1 = new DieRandom(random, 10, challengeDie1);
        Challenge2 = new DieRandom(random, 10, challengeDie2);
        ActionAdds = new List<int>() { stat, adds };
        this.momentum = momentum;
    }

    public IDie Action { get; set; }
    public int ActionScore { get => Math.Min(Action.Value + ActionAdds.Sum(), 10); }
    public IDie Challenge1 { get; set; }
    public IDie Challenge2 { get; set; }
    public bool IsEphemeral { get; set; } = false;
    public bool IsMatch => Challenge1 != null && Challenge2 != null && Challenge1.CompareTo(Challenge2) == 0;
    public string Name { get; set; }
    public string? DiscordMessage { get; set; } = null;
    public string? StatName { get; }

    private string burnMessage = string.Empty;

    public IronswornRollOutcome GetBurnResult()
    {
        if (momentum == null) return IronswornRollOutcome.Miss;
        if (momentum > Math.Max(Challenge1.Value, Challenge2.Value)) return IronswornRollOutcome.StrongHit;
        if (momentum > Math.Min(Challenge1.Value, Challenge2.Value)) return IronswornRollOutcome.WeakHit;
        return IronswornRollOutcome.Miss;
    }

    public bool CanBurn()
    {
        if (momentum == null) return false;
        
        return GetBurnResult() > GetOutcome();
    }

    public async Task<ComponentBuilder?> GetComponentsAsync()
    {
        var builder = new ComponentBuilder();
        if (GetOutcome() == IronswornRollOutcome.Miss)
        {
            var playerOracles = await dataFactory.GetPlayerOracles(playerId);
            string ptpId = playerOracles?.FirstOrDefault(a => a.JsonId.Contains("Oracles/Moves/Pay_the_Price"))?.JsonId ?? "Ironsworn/Oracles/Moves/Pay_the_Price";
            builder.WithButton("Pay the Price", $"roll-oracle:{ptpId}", emote: emotes.Roll);
        }

        if (CanBurn() && characterId != null)
        {
            builder.WithButton("Burn Momentum", $"burn-roll:{momentum},{characterId}", emote: emotes.BurnMomentum);
        }

        return builder;
    }

    public EmbedBuilder GetEmbed()
    {
        var embed = new EmbedBuilder()
            .WithAuthor(Name)
            .WithTitle(GetOutcome().ToOutcomeString(IsMatch))
            .WithColor(GetOutcome().OutcomeColor())
            .WithThumbnailUrl(GetOutcome().OutcomeIcon());

        if (!string.IsNullOrWhiteSpace(description)) { embed.WithDescription(description); }

        if (CanBurn()) embed.WithFooter($"You may burn +{momentum} momentum for a {GetBurnResult().ToOutcomeString()} (see p. 32).");
        if (Action.Value + ActionAdds.Sum() > 10) embed.WithFooter(IronswornRollResources.OverMaxMessage);
        if (!string.IsNullOrWhiteSpace(burnMessage)) embed.WithFooter(burnMessage);

        string actionScoreDisplay = ActionAdds.Count == 0 ? Action.Value.ToString() : $"{Action.Value} + {String.Join(" + ", ActionAdds)} = {ActionScore}";
        embed.AddField("Action Score", actionScoreDisplay)
            .AddField("Challenge Dice", $"{Challenge1.Value}, {Challenge2.Value}");

        if (jumpURL != null) embed.Author.WithUrl(jumpURL);
        else if (characterId > 0 && db != null)
        {
            var player = db.PlayerCharacters.Find(characterId);
            if (player != null) 
            {
                embed.WithAuthor($"{player.Name} rolls");
                embed.Author.WithUrl(player.GetAssumedJumpUrl());
            }
        }

        if (StatName != null)
        {
            embed.Author.Name += $" +{StatName}";
        }

        return embed;
    }

    public IronswornRollOutcome GetOutcome()
    {
        if (ActionScore > Math.Max(Challenge1.Value, Challenge2.Value)) return IronswornRollOutcome.StrongHit;
        if (ActionScore > Math.Min(Challenge1.Value, Challenge2.Value)) return IronswornRollOutcome.WeakHit;
        return IronswornRollOutcome.Miss;
    }

    public void Burn(int? momentum, int? reset = null)
    {
        if (!momentum.HasValue) return;

        burnMessage = $"Momentum was burned to change this result from {ActionScore} to {momentum.Value}";

        Action = new DieStatic(10, momentum.Value);
        ActionAdds = new List<int>();

        this.momentum = reset;
    }
}

public class ProgressRollRandom : IActionRoll
{
    private readonly string description;

    public ProgressRollRandom(Random random, int progressAmount, string description, int? challengeDie1 = null, int? challengeDie2 = null) : base()
    {
        Action = new DieStatic(10, progressAmount);
        Challenge1 = new DieRandom(random, 10, challengeDie1);
        Challenge2 = new DieRandom(random, 10, challengeDie2);

        Name = "Progress Roll";
        this.description = description;
    }

    public IDie Action { get; set; }
    public IDie Challenge1 { get; set; }
    public IDie Challenge2 { get; set; }
    public bool IsEphemeral { get; set; } = false;
    public bool IsMatch => Challenge1 != null && Challenge2 != null && Challenge1.CompareTo(Challenge2) == 0;
    public string Name { get; set; }
    public string? DiscordMessage { get; set; } = null;

    public Task<ComponentBuilder?> GetComponentsAsync()
    {
        return Task.FromResult<ComponentBuilder?>(null);
    }

    public EmbedBuilder GetEmbed()
    {
        var embed = new EmbedBuilder()
            .WithAuthor(Name)
            .WithTitle(GetOutcome().ToOutcomeString())
            .WithColor(GetOutcome().OutcomeColor())
            .WithThumbnailUrl(GetOutcome().OutcomeIcon());

        if (!String.IsNullOrWhiteSpace(description)) embed.WithDescription(description);

        embed.AddField("Action Score", $"{Action.Value}")
            .AddField("Challenge Dice", $"{Challenge1.Value}, {Challenge2.Value}");

        return embed;
    }

    public IronswornRollOutcome GetOutcome()
    {
        if (Action.Value > Math.Max(Challenge1.Value, Challenge2.Value)) return IronswornRollOutcome.StrongHit;
        if (Action.Value > Math.Min(Challenge1.Value, Challenge2.Value)) return IronswornRollOutcome.WeakHit;
        return IronswornRollOutcome.Miss;
    }

    public void SetActionScore(int value)
    {
        Action = new DieStatic(10, value);
    }
}
