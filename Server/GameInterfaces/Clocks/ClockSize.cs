using System.ComponentModel.DataAnnotations;
using Discord.Interactions;

namespace TheOracle2.GameObjects;

public enum ClockSize
{
    [ChoiceDisplay("4 segments")]
    Four = 4,

    [ChoiceDisplay("6 segments")]
    Six = 6,

    [ChoiceDisplay("8 segments")]
    Eight = 8,

    [ChoiceDisplay("10 segments")]
    Ten = 10
}

public enum SceneChallengeClockSize
{
    [ChoiceDisplay("4 segments")]
    Four = 4,

    [ChoiceDisplay("6 segments")]
    Six = 6,

    [ChoiceDisplay("8 segments")]
    Eight = 8,
}
