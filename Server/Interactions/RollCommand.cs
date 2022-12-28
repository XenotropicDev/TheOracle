using Discord.Interactions;
using TheOracle2.GameObjects;

namespace TheOracle2;

public class RollCommand : InteractionModuleBase
{
    public RollCommand(Random random)
    {
        Random = random;
    }

    public Random Random { get; }

    [SlashCommand("roll-dice", "Rolls dice")]
    public async Task RollDie([ComplexParameter]DieNotation first, [ComplexParameter] DieNotation second = null)
    {
        List<int> firstRollResults = GenerateRollList(first);
        List<int> secondRollResults = GenerateRollList(second);

        var outputString = $"{first.Number}d{first.Sides}: {string.Join(", ", firstRollResults)}";
        if (second != null) outputString += $"\n\n{second.Number}d{second.Sides}: {string.Join(", ", secondRollResults)}";
        await RespondAsync(outputString).ConfigureAwait(false);
    }

    private List<int> GenerateRollList(DieNotation die)
    {
        var rollList = new List<int>();
        if (die != null)
        {
            for (var i = 0; i < die.Number; i++)
            {
                var roll = Random.Next(1, die.Sides + 1);
                rollList.Add(roll);
            }
        }
        return rollList;
    }
}

public class DieNotation
{
    public DieNotation([MinValue(1)]int number, [MinValue(2)]int sides)
    {
        if (number < 1) throw new ArgumentException(nameof(number));
        if (sides < 2) throw new ArgumentException(nameof(sides));

        Number = number;
        Sides = sides;
    }

    public int Number { get; }
    public int Sides { get; }
}
