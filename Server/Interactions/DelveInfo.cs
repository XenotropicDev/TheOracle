using Server.GameInterfaces;
using TheOracle2;
using TheOracle2.GameObjects;

namespace Server.Interactions
{
    public class DelveInfo : IDiscordEntity
    {
        public DelveInfo(ChallengeRank rank, DelveThemeOption theme, DelveDomainOption domain, string siteName, string siteObjective)
        {
            Rank = rank;
            Theme = theme;
            Domain = domain;
            SiteName = siteName;
            SiteObjective = siteObjective;
            TrackData = new TrackData();
        }

        public int Id { get; set; }

        public ChallengeRank Rank { get; set; }
        public DelveThemeOption Theme { get; set; }
        public DelveDomainOption Domain { get; set; }
        public string SiteName { get; set; }
        public string SiteObjective { get; set; }
        public TrackData TrackData { get; private set; }
        public DenizenMatrix Denizens { get; set; } = new();
        public bool IsEphemeral { get; set; } = false;
        public string? DiscordMessage { get; set; }

        public Task<ComponentBuilder?> GetComponentsAsync()
        {
            throw new NotImplementedException();
        }

        public EmbedBuilder? GetEmbed()
        {
            //Todo: finish populating this
            var builder = new EmbedBuilder();

            builder.WithAuthor("Delve Site");
            builder.WithTitle(SiteName);
            builder.WithDescription(SiteObjective);

            return builder;
        }
    }
}
