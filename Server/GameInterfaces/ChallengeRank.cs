namespace Server.GameInterfaces;

public enum ChallengeRank
{
    Troublesome = 1,
    Dangerous = 2,
    Formidable = 3,
    Extreme = 4,
    Epic = 5
}

public static class ChallengeRankExtenstions
{
    public static int GetStandardTickAmount(this ChallengeRank cr)
    {
        return cr switch
        {
            ChallengeRank.Troublesome => 12,
            ChallengeRank.Dangerous => 8,
            ChallengeRank.Formidable => 4,
            ChallengeRank.Extreme => 2,
            ChallengeRank.Epic => 1,
            _ => 0,
        };
    }
}
