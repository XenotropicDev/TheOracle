using System.IO;
using Discord.Interactions;
using Server.GameInterfaces;
using Server.Interactions.Helpers;

namespace Server.Interactions
{
    [DontAutoRegister]
    public class DelveCommand : InteractionModuleBase
    {
        public Random Random { get; set; }

        public DelveCommand(Random random)
        {
            Random = random;
        }

        [SlashCommand("delve", "Creates a delve site")]
        public async Task DiscoverSite(ChallengeRank rank, DelveThemeOption theme, DelveDomainOption domain)
        {
            if (theme == DelveThemeOption.Random) 
            {
                theme = (DelveThemeOption)Random.Next(1, Enum.GetValues<DelveThemeOption>().Length);
            }

            if (domain == DelveDomainOption.Random)
            {
                domain = (DelveDomainOption)Random.Next(1, Enum.GetValues<DelveDomainOption>().Length);
            }

            //todo site name and objective modal?

            var sitename = "Delve Site";
            var siteObjective = "";

            var delveInfo = new DelveInfo(rank, theme, domain, sitename, siteObjective);

            await delveInfo.EntityAsResponse(RespondAsync).ConfigureAwait(false);
        }
    }

    public enum DelveThemeOption { Random, Ancient, Corrupted, Fortified, Hallowed, Haunted, Infested, Ravaged, Wild}
    public enum DelveDomainOption {Random, Barrow, Cavern, FrozenCavern, Icereach, Mine, Pass, Ruin, SeaCave, Shadowfen, Stronghold, Tanglewood, Underkeep }
}
