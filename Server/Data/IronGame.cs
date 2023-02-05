using System.Text.RegularExpressions;

namespace TheOracle2;

public enum IronGame
{
    Ironsworn,
    Starforged
}

public static class IronGameExtenstions
{
    public static IronGame? GetIronGameInString(string value)
    {
        foreach(var game in Enum.GetValues<IronGame>())
        {
            if (value.Contains(game.ToString(), StringComparison.OrdinalIgnoreCase)) return game;
        }

        return null;
    }

    public static string RemoveIronGameInString(string value)
    {
        foreach (var game in Enum.GetValues<IronGame>())
        {
            value = Regex.Replace(value, game.ToString() + " ?", "", RegexOptions.IgnoreCase);
        }

        return value.Trim();
    }
}
