namespace TheOracle2.GameObjects;

/// <summary>
/// Interface for game objects where challenge dice matches are relevant.
/// </summary>
public interface IMatchable
{
    bool IsMatch { get; }
}
