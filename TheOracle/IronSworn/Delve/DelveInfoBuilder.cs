using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TheOracle.Core;
using TheOracle.GameCore;
using TheOracle.GameCore.Oracle;
using TheOracle.GameCore.ProgressTracker;

namespace TheOracle.IronSworn.Delve
{
    public class DelveInfoBuilder
    {
        private List<string> randomAliases = DelveResources.RandomAliases.Split(',').ToList();

        public DelveInfoBuilder(DelveService delveService, OracleService oracles)
        {
            DelveService = delveService;
            Oracles = oracles;
        }

        public DelveService DelveService { get; }
        public string DomainInput { get; private set; }
        public string NameInput { get; private set; }
        public string Objective { get; set; }
        public OracleService Oracles { get; }
        public string RankInput { get; private set; }
        public string ThemeInput { get; private set; }
        private List<Domain> Domains { get; set; } = new List<Domain>();
        private string Name { get; set; }
        private ChallengeRank Rank { get; set; }
        private List<Theme> Themes { get; set; } = new List<Theme>();

        public DelveInfo Build()
        {
            var delveInfo = new DelveInfo();
            delveInfo.SiteObjective = Objective;
            delveInfo.Description = Objective;
            delveInfo.Rank = Rank;
            delveInfo.Domains = Domains;
            delveInfo.Themes = Themes;

            //Just to be safe if things aren't added in the right order parse the name again
            if (randomAliases.Any(alias => alias.Equals(Name, StringComparison.OrdinalIgnoreCase)))
            {
                var roller = new OracleRoller(Oracles, GameName.Ironsworn);
                roller.BuildRollResults("Site Name Format");
                Name = roller.RollResultList.First().Result.Description;
                string place = roller.BuildRollResults($"Site Name Place {delveInfo.Domains.First().DelveSiteDomain}").RollResultList.First().Result.Description;
                Name = Name.Replace("{Place}", place);
            }
            delveInfo.SiteName = Name;

            return delveInfo;
        }

        public override string ToString()
        {
            string message = string.Empty;

            if (Themes.Count > 0) message += string.Format(DelveResources.Theme, String.Join(DelveResources.ListSeperator, Themes.Select(t => t.DelveSiteTheme))) + "\n";
            if (Domains.Count > 0) message += string.Format(DelveResources.Domain, String.Join(DelveResources.ListSeperator, Domains.Select(d => d.DelveSiteDomain))) + "\n";
            message += (Name?.Length > 0) ? String.Format(DelveResources.SiteName, Name) + "\n" : string.Empty;
            message += (Objective?.Length > 0) ? String.Format(DelveResources.Objective, Objective) + "\n" : string.Empty;
            message += (Rank != ChallengeRank.None) ? String.Format(DelveResources.Rank, Rank) + "\n" : string.Empty;

            return message;
        }

        public DelveInfoBuilder WithDomains(string domainInput)
        {
            int randomDomains = 0;
            DomainInput = domainInput;

            string[] seperators = new string[] { DelveResources.ListSeperator, DelveResources.ListSeperator.Trim() };
            var domains = domainInput.Split(seperators, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < domains.Length; i++)
            {
                domains[i] = domains[i].Trim();
                if (randomAliases.Any(alias => alias.Equals(domains[i], StringComparison.OrdinalIgnoreCase)))
                {
                    randomDomains++;
                    continue;
                }

                var matchingDomain = DelveService.Domains.Find(d => d.DelveSiteDomain.Equals(domains[i], StringComparison.OrdinalIgnoreCase));
                if (matchingDomain != null)
                {
                    Domains.Add(matchingDomain);
                    continue;
                }

                if (int.TryParse(domains[i], out int domainValue) && domainValue - 1 < DelveService.Domains.Count)
                {
                    Domains.Add(DelveService.Domains[domainValue - 1]);
                    continue;
                }

                throw new ArgumentException(String.Format(DelveResources.UnknownDomainError, domains[i]));
            }

            for (int i = 0; i < randomDomains; i++)
            {
                Domain toAdd = null;
                while (toAdd == null)
                {
                    int value = BotRandom.Instance.Next(0, DelveService.Domains.Count);
                    var check = DelveService.Domains[value];
                    if (!this.Domains.Contains(check)) toAdd = check;
                }

                Domains.Add(toAdd);
            }

            return this;
        }

        public DelveInfoBuilder WithName(string name)
        {
            NameInput = name;

            if (randomAliases.Any(alias => alias.Equals(NameInput, StringComparison.OrdinalIgnoreCase)) && Domains.Count > 0)
            {
                var roller = new OracleRoller(Oracles, GameName.Ironsworn);
                roller.BuildRollResults("Site Name Format");
                Name = roller.RollResultList.First().Result.Description;
                string place = roller.BuildRollResults($"Site Name Place {Domains.First().DelveSiteDomain}").RollResultList.First().Result.Description;
                Name = Name.Replace("{Place}", place);
            }
            else
            {
                Name = name;
            }

            return this;
        }

        public DelveInfoBuilder WithObjective(string objective)
        {
            Objective = objective;
            return this;
        }

        public DelveInfoBuilder WithRank(string rank)
        {
            RankInput = rank;

            ChallengeRankHelper.TryParse(RankInput, out ChallengeRank cr);
            if (cr == ChallengeRank.None && int.TryParse(RankInput, out int rankNumber))
            {
                if (rankNumber == 1) cr = ChallengeRank.Troublesome;
                if (rankNumber == 2) cr = ChallengeRank.Dangerous;
                if (rankNumber == 3) cr = ChallengeRank.Formidable;
                if (rankNumber == 4) cr = ChallengeRank.Extreme;
                if (rankNumber == 5) cr = ChallengeRank.Epic;
            }
            Rank = cr;

            return this;
        }

        public DelveInfoBuilder WithThemes(string themeInput)
        {
            ThemeInput = themeInput;
            int randomThemes = 0;

            string[] seperators = new string[] { DelveResources.ListSeperator, DelveResources.ListSeperator.Trim() };
            var themes = themeInput.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < themes.Length; i++)
            {
                themes[i] = themes[i].Trim();
                if (randomAliases.Any(alias => alias.Equals(themes[i], StringComparison.OrdinalIgnoreCase)))
                {
                    randomThemes++;
                    continue;
                }

                var matchingTheme = DelveService.Themes.Find(t => t.DelveSiteTheme.Equals(themes[i], StringComparison.OrdinalIgnoreCase));
                if (matchingTheme != null)
                {
                    Themes.Add(matchingTheme);
                    continue;
                }

                if (int.TryParse(themes[i], out int themeValue) && themeValue - 1 < DelveService.Themes.Count)
                {
                    Themes.Add(DelveService.Themes[themeValue - 1]);
                    continue;
                }

                throw new ArgumentException(String.Format(DelveResources.UnknownThemeError, themes[i]));
            }

            for (int i = 0; i < randomThemes; i++)
            {
                Theme toAdd = null;
                while (toAdd == null)
                {
                    int value = BotRandom.Instance.Next(0, DelveService.Themes.Count);
                    var check = DelveService.Themes[value];
                    if (!this.Themes.Contains(check)) toAdd = check;
                }

                Themes.Add(toAdd);
            }

            return this;
        }
    }
}