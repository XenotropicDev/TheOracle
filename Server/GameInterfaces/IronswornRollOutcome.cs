using Server.GameInterfaces;

namespace TheOracle2;

public enum IronswornRollOutcome
{
    Miss = 0,
    WeakHit = 1,
    StrongHit = 2
}

public static class IronswornRollOutcomeExtensions
{
    public static string ToOutcomeString(this IronswornRollOutcome outcome, bool isMatch = false)
    {
        if (isMatch)
        {
            return outcome switch
            {
                IronswornRollOutcome.Miss => IronswornRollResources.MissWithAMatch,
                IronswornRollOutcome.WeakHit => IronswornRollResources.WeakHitWithAMatch,
                IronswornRollOutcome.StrongHit => IronswornRollResources.StrongHitWithAMatch,
                _ => "ERROR",
            };
        }

        return outcome switch
        {
            IronswornRollOutcome.Miss => IronswornRollResources.Miss,
            IronswornRollOutcome.WeakHit => IronswornRollResources.Weak_Hit,
            IronswornRollOutcome.StrongHit => IronswornRollResources.Strong_Hit,
            _ => "ERROR",
        };
    }

    public static Color OutcomeColor(this IronswornRollOutcome outcome) => outcome switch
    {
        IronswornRollOutcome.Miss => new Color(0xC50933),
        IronswornRollOutcome.WeakHit => new Color(0x842A8C),
        IronswornRollOutcome.StrongHit => new Color(0x47AEDD),
        _ => new Color(0x842A8C),
    };

    public static string OutcomeIcon(this IronswornRollOutcome outcome) => outcome switch
    {
        IronswornRollOutcome.Miss => IronswornRollResources.MissImageURL,
        IronswornRollOutcome.WeakHit => IronswornRollResources.WeakHitImageURL,
        IronswornRollOutcome.StrongHit => IronswornRollResources.StrongHitImageURL,
        _ => IronswornRollResources.MissImageURL,
    };
}
