namespace TheOracle2.GameObjects;

/// <summary>
/// Represents a single game die with an arbitrary number of sides and the number it currently shows.
/// </summary>
public class DieRandom : IDie
{
    /// <summary>
    /// Rolls or sets a game die.
    /// </summary>
    /// <param name="sides">The number of sides the die has (minimum 2)</param>
    /// <param name="value">A optional preset value for the die. If this is omitted, the die will be rolled and show a random side.</param>
    /// <exception cref="ArgumentOutOfRangeException">If sides is less than 2; if value is less than 1; if value exceeds sides</exception>
    public DieRandom(Random random, int sides, int? value = null)
    {
        if (sides < 2) { throw new ArgumentOutOfRangeException(nameof(sides), "Die must have at least 2 sides."); }
        if (value != null && (value > sides || value < 1)) { throw new ArgumentOutOfRangeException(nameof(value), "Value must be null, or a positive integer less than the number of sides on the die."); }

        this.random = random;
        Sides = sides;
        Value = value ?? Roll();
    }

    private readonly Random random;

    public static implicit operator int(DieRandom die)
    {
        return die.Value;
    }

    public static implicit operator string(DieRandom die)
    {
        return die.Value.ToString();
    }

    public int Sides { get; }
    public int Value { get; set; }

    private int Roll()
    {
        return random.Next(1, Sides + 1);
    }

    /// <summary>
    /// Re-roll the die to randomize its Value.
    /// </summary>
    public void Reroll()
    {
        Value = Roll();
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override bool Equals(object? obj)
    {
        return obj is IDie die && die.Sides == Sides && die.Value == Value;
    }

    public int CompareTo(IDie? other)
    {
        if (other == null) return 1;
        return this.Value.CompareTo(other.Value);
    }

    public int CompareTo(int other)
    {
        return this.Value.CompareTo(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Sides, Value);
    }
}
