using Discord.Interactions;

namespace TheOracle2;

public class DieNotation
{
    public DieNotation([MinValue(1)][MaxValue(1000)] int dice, [MinValue(2)] int sides)
    {
        //if (numberOfDice < 1) throw new ArgumentException(nameof(numberOfDice));
        //if (sides < 2) throw new ArgumentException(nameof(sides));

        Number = dice;
        Sides = sides;
    }

    public int Number { get; }
    public int Sides { get; }

    public override string ToString()
    {
        return $"{Number}d{Sides}";
    }
}
