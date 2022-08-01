namespace TheOracle2.GameObjects;

public interface IDie : IComparable<IDie>, IComparable<int>
{
    int Sides { get; }
    int Value { get; set; }
    bool Equals(object? obj);
    void Reroll();
    string? ToString();
}

public class DieStatic : IDie
{
    public DieStatic(int sides, int value)
    {
        Sides = sides;
        Value = value;
    }

    public int Sides { get; }

    public int Value { get; set; }

    public int CompareTo(IDie? other)
    {
        if (other == null) return 1;
        return this.Value.CompareTo(other.Value);
    }

    public int CompareTo(int other)
    {
        return this.Value.CompareTo(other);
    }

    public void Reroll()
    {
        return;
    }
}
