using System.Diagnostics.CodeAnalysis;

namespace TheOracle2.GameObjects;

public class CampaignClock : Clock
{
    [SetsRequiredMembers]
    public CampaignClock(Embed embed) : base(embed)
    {
    }

    [SetsRequiredMembers]
    public CampaignClock(ClockSize segments, int filledSegments, string title, string description = "") : base(segments, filledSegments, title, description)
    {
    }

    public override string EmbedCategory => "Campaign Clock";
    public override string ClockFillMessage => "The event is triggered or the project is complete. Envision the outcome and the impact on your setting.";

    public override ComponentBuilder MakeComponents()
    {
        SelectMenuBuilder menu = new SelectMenuBuilder()
            .WithCustomId($"clock-menu")
            .WithMinValues(0);

        if (!IsFull)
        {
            menu.AddOption(AdvanceOption());
            foreach (AskOption odds in Enum.GetValues(typeof(AskOption)))
            {
                menu.AddOption(AdvanceAskOption(odds));
            }
        }

        if (Filled != 0)
        {
            menu.AddOption(ResetOption());
        }

        return new ComponentBuilder()
            .WithSelectMenu(menu);
    }
}
