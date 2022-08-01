using System.ComponentModel.DataAnnotations;
using Discord.Interactions;
using TheOracle2.GameObjects;

namespace TheOracle2;

public class AskTheOracleCommand : InteractionModuleBase
{
    private readonly Random random;

    public AskTheOracleCommand(Random random)
    {
        this.random = random;
    }

    [SlashCommand("ask", "Ask the Oracle a yes/no question (p. 225). To roll on a specific oracle table, use /oracle.")]
    public async Task AskTheOracle(
        [Summary(description: "The question to ask the oracle.")]
        string question,
        [Summary(description: "The odds of receiving a 'yes' answer.")]
        [Choice("Small chance (10 or less)", 10),
        Choice("Unlikely (25 or less)", 25),
        Choice("50/50 (50 or less)", 50),
        Choice("Likely (75 or less)", 75),
        Choice("Sure thing (90 or less)", 90)]
        int odds
    )
    {
        /// TODO: once discord display string attributes are available for enums, this can use the AskOption enum directly
        await RespondAsync(embed: new OracleAnswer(random, (AskOption)odds, question).ToEmbed().Build()).ConfigureAwait(false);
    }
}

public enum AskOption
{
    [Display(Name = "Sure thing")]
    SureThing = 90,

    Likely = 75,

    [Display(Name = "Fifty-fifty")]
    FiftyFifty = 50,

    Unlikely = 25,

    [Display(Name = "Small chance")]
    SmallChance = 10
}

public class OracleAnswer : DieRandom, IMatchable
{
    public OracleAnswer(Random random, AskOption odds, string question) : base(random, 100)
    {
        Question = question;
        Odds = odds;
    }

    public string Question { get; set; }
    public AskOption Odds { get; }
    public bool IsYes => Value <= (int)Odds;
    public bool IsMatch => Value == 100 || Value % 11 == 0;

    public override string ToString()
    {
        return $"Rolled {Value} vs. {(int)Odds}: **{AnswerString}**";
    }

    public Color OutcomeColor() => IsYes ? new Color(0x47AEDD) : new Color(0xC50933);

    private string MatchMessage => (IsMatch) ? "You rolled a match! Envision an extreme result or twist." : String.Empty;

    private string AnswerString
    {
        get
        {
            string str = IsYes ? "Yes" : "No";
            if (IsMatch)
            {
                str = $"hell {str}!".ToUpperInvariant();
            }
            return str;
        }
    }

    public static Dictionary<AskOption, string> OddsString => new()
    {
        { AskOption.SmallChance, "Small chance" },
        { AskOption.Unlikely, "Unlikely" },
        { AskOption.FiftyFifty, "50/50" },
        { AskOption.Likely, "Likely" },
        { AskOption.SureThing, "Sure thing" }
    };

    public EmbedBuilder ToEmbed()
    {
        string authorString = $"Ask the Oracle: {OddsString[Odds]}";
        string footerString = MatchMessage;
        return new EmbedBuilder()
        .WithAuthor(authorString)
        .WithDescription(ToString())
        .WithTitle(Question.Length > 0 ? Question : "Ask the Oracle")
        .WithFooter(footerString)
        .WithColor(OutcomeColor())
        ;
    }
}
