using System.Diagnostics.CodeAnalysis;

namespace TheOracle2.GameObjects;

public class TensionClock : Clock
{
    [SetsRequiredMembers]
    public TensionClock(Embed embed) : base(embed)
    {
    }

    [SetsRequiredMembers]
    public TensionClock(ClockSize segments, int filledSegments, string title, string description = "") : base(segments, filledSegments, title, description)
    {
    }

    public override string EmbedCategory => "Tension Clock";
    public override string ClockFillMessage => "The threat or deadline triggers. This should result in harrowing problems for your character. It may even force you to abandon an expedition, fight, vow, or other challenge.";
}
