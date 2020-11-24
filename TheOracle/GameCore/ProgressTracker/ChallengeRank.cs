namespace TheOracle.GameCore.ProgressTracker
{
    public enum ChallengeRank
    {
        None,
        Troublesome,
        Dangerous,
        Formidable,
        Extreme,
        Epic
    }

    public static class ChallengeRankHelper
    {
        public static bool TryParse(string s, out ChallengeRank cr)
        {
            cr = ChallengeRank.None;
            if (s.Contains(ProgressResources.Troublesome, System.StringComparison.OrdinalIgnoreCase)) cr = ChallengeRank.Troublesome;
            if (s.Contains(ProgressResources.Dangerous, System.StringComparison.OrdinalIgnoreCase)) cr = ChallengeRank.Dangerous;
            if (s.Contains(ProgressResources.Formidable, System.StringComparison.OrdinalIgnoreCase)) cr = ChallengeRank.Formidable;
            if (s.Contains(ProgressResources.Extreme, System.StringComparison.OrdinalIgnoreCase)) cr = ChallengeRank.Extreme;
            if (s.Contains(ProgressResources.Epic, System.StringComparison.OrdinalIgnoreCase)) cr = ChallengeRank.Epic;

            return cr != ChallengeRank.None;
        }
    }
}