using Server.GameInterfaces;

namespace Server.Interactions
{
    public class DelveInfo
    {
        public DelveInfo(ChallengeRank rank, DelveThemeOption theme, DelveDomainOption domain, string siteName, string siteObjective)
        {
            Rank = rank;
            Theme = theme;
            Domain = domain;
            SiteName = siteName;
            SiteObjective = siteObjective;
        }

        public int Id { get; set; }

        public ChallengeRank Rank { get; set; }
        public DelveThemeOption Theme { get; set; }
        public DelveDomainOption Domain { get; set; }
        public string SiteName { get; set; }
        public string SiteObjective { get; set; }
        public DenizenMatrix Denizens { get; set; } = new();
    }
}
