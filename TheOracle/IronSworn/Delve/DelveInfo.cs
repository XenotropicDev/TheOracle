using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOracle.BotCore;
using TheOracle.Core;
using TheOracle.GameCore;
using TheOracle.GameCore.Oracle;
using TheOracle.GameCore.ProgressTracker;

namespace TheOracle.IronSworn.Delve
{
    public class DelveInfo : ProgressTrackerInfo
    {
        public DelveInfo() : base()
        {

        }

        public override string DifficultyFieldTitle => DelveResources.RankField;
        public List<Domain> Domains { get; set; } = new List<Domain>();
        public string SiteName { get; set; }
        public string SiteObjective { get; set; }
        public List<Theme> Themes { get; set; } = new List<Theme>();

        public static DelveInfo FromInput(DelveService delveService, OracleService oracles, string themeInput, string domainInput, string siteNameInput, string siteObjective, string siteRankInput)
        {
            var delveInfo = new DelveInfo();
            var themeItems = themeInput.Split(',');
            var domainItems = domainInput.Split(',');
            var randomAliases = DelveResources.RandomAliases.Split(',').ToList();
            int randomThemes = 0;
            int randomDomains = 0;

            delveInfo.SiteObjective = siteObjective;

            ChallengeRankHelper.TryParse(siteRankInput, out ChallengeRank cr);
            if (cr == ChallengeRank.None && int.TryParse(siteRankInput, out int rankNumber))
            {
                if (rankNumber == 1) cr = ChallengeRank.Troublesome;
                if (rankNumber == 2) cr = ChallengeRank.Dangerous;
                if (rankNumber == 3) cr = ChallengeRank.Formidable;
                if (rankNumber == 4) cr = ChallengeRank.Extreme;
                if (rankNumber == 5) cr = ChallengeRank.Epic;
            }
            delveInfo.Rank = cr;

            for (int i = 0; i < themeItems.Length; i++)
            {
                themeItems[i] = themeItems[i].Trim();
                if (randomAliases.Any(alias => alias.Equals(themeItems[i], StringComparison.OrdinalIgnoreCase)))
                {
                    randomThemes++;
                    continue;
                }

                var matchingTheme = delveService.Themes.Find(t => t.DelveSiteTheme.Equals(themeItems[i], StringComparison.OrdinalIgnoreCase));
                if (matchingTheme != null)
                {
                    delveInfo.Themes.Add(matchingTheme);
                    continue;
                }

                if (int.TryParse(themeItems[i], out int themeValue) && themeValue - 1 < delveService.Themes.Count)
                {
                    delveInfo.Themes.Add(delveService.Themes[themeValue - 1]);
                    continue;
                }

                throw new ArgumentException(String.Format(DelveResources.UnknownThemeError, themeItems[i]));
            }

            for (int i = 0; i < domainItems.Length; i++)
            {
                domainItems[i] = domainItems[i].Trim();
                if (randomAliases.Any(alias => alias.Equals(domainItems[i], StringComparison.OrdinalIgnoreCase)))
                {
                    randomDomains++;
                    continue;
                }

                var matchingDomain = delveService.Domains.Find(d => d.DelveSiteDomain.Equals(domainItems[i], StringComparison.OrdinalIgnoreCase));
                if (matchingDomain != null)
                {
                    delveInfo.Domains.Add(matchingDomain);
                    continue;
                }

                if (int.TryParse(domainItems[i], out int domainValue) && domainValue - 1 < delveService.Domains.Count)
                {
                    delveInfo.Domains.Add(delveService.Domains[domainValue - 1]);
                    continue;
                }

                throw new ArgumentException(String.Format(DelveResources.UnknownDomainError, domainItems[i]));
            }

            for (int i = 0; i < randomThemes; i++) delveInfo.AddRandomTheme(delveService);
            for (int i = 0; i < randomDomains; i++) delveInfo.AddRandomDomain(delveService);

            if (randomAliases.Any(alias => alias.Equals(siteNameInput, StringComparison.OrdinalIgnoreCase)))
            {
                var roller = new OracleRoller(oracles, GameName.Ironsworn);
                roller.BuildRollResults("Site Name Format");
                siteNameInput = roller.RollResultList.First().Result.Description;
                string place = roller.BuildRollResults($"Site Name Place {delveInfo.Domains.First().DelveSiteDomain}").RollResultList.First().Result.Description;
                siteNameInput = siteNameInput.Replace("{Place}", place);
            }
            delveInfo.SiteName = siteNameInput;

            return delveInfo;
        }

        public override IEmbed BuildEmbed()
        {
            var builder = base.BuildEmbed().ToEmbedBuilder();
            builder.WithAuthor(String.Format(DelveResources.CardThemeDomainTitleFormat,
                string.Join(DelveResources.ListSeperator, Themes.Select(t => t.DelveSiteTheme)),
                string.Join(DelveResources.ListSeperator, Domains.Select(d => d.DelveSiteDomain))));
            builder.WithTitle(String.Format(DelveResources.CardSiteNameFormat, SiteName));
            builder.WithDescription(SiteObjective);

            string riskText = string.Empty;
            if (Ticks <= 12) riskText = DelveResources.RiskZoneLow;
            else if (Ticks <= 28) riskText = DelveResources.RiskZoneMedium;
            else riskText = DelveResources.RiskZoneHigh;
            builder.AddField(DelveResources.RiskZoneField, riskText);

            return builder.Build();
        }

        public override string ToString()
        {
            string info = $"Name: {SiteName}";
            info += $"\nObjective: {SiteObjective}";
            info += $"\nRank: {Rank}";
            info += $"\nThemes: ";
            info += string.Join(',', Themes.Select(t => $"**{t.DelveSiteTheme}**"));

            info += $"\nDomains: ";
            info += string.Join(',', Domains.Select(d => $"**{d.DelveSiteDomain}**"));

            return info;
        }

        internal DelveInfo FromMessage(DelveService delveService, IUserMessage message)
        {
            var embed = message.Embeds.First();
            base.PopulateFromMessage(message);

            if (!Enum.TryParse(embed.Fields.FirstOrDefault(f => f.Name == DifficultyFieldTitle).Value, out ChallengeRank challengeRank))
                throw new ArgumentException("Unknown delve post format, unable to parse difficulty");

            Rank = challengeRank;

            if (embed.Footer.HasValue)
            {
                Ticks = (Int32.TryParse(embed.Footer.Value.Text.Replace(ProgressResources.Ticks, "").Replace(":", ""), out int temp)) ? temp : 0;
            }
            Description = embed.Description;
            SiteObjective = embed.Description;
            if (!Utilities.UndoFormatString(embed.Title, DelveResources.CardSiteNameFormat, out string[] titleValues)) titleValues = new string[] { "Delve Site Title Error" };
            SiteName = titleValues[0];

            if (!Utilities.UndoFormatString(embed.Author.Value.Name, DelveResources.CardThemeDomainTitleFormat, out string[] themeDomainArgs))
                throw new ArgumentException("Unknown delve post format, unable to parse themes and domains");

            var themes = themeDomainArgs[0].Split(DelveResources.ListSeperator).Select(s => s.Trim());
            var domains = themeDomainArgs[1].Split(DelveResources.ListSeperator).Select(s => s.Trim());

            Themes.AddRange(delveService.Themes.Where(t1 => themes.Any(t2 => t2.Equals(t1.DelveSiteTheme, StringComparison.OrdinalIgnoreCase))).Take(2));
            Domains.AddRange(delveService.Domains.Where(d1 => domains.Any(d2 => d2.Equals(d1.DelveSiteDomain, StringComparison.OrdinalIgnoreCase))).Take(2));

            return this;
        }

        public void AddRandomDomain(DelveService delveService)
        {
            Domain toAdd = null;
            while (toAdd == null)
            {
                int value = BotRandom.Instance.Next(0, delveService.Domains.Count);
                var check = delveService.Domains[value];
                if (!this.Domains.Contains(check)) toAdd = check;
            }

            this.Domains.Add(toAdd);
        }

        public void AddRandomTheme(DelveService delveService)
        {
            Theme toAdd = null;
            while (toAdd == null)
            {
                int value = BotRandom.Instance.Next(0, delveService.Themes.Count);
                var check = delveService.Themes[value];
                if (!this.Themes.Contains(check)) toAdd = check;
            }

            this.Themes.Add(toAdd);
        }

        public OracleRoller RevealDangerRoller(OracleService oracleService)
        {
            var roller = new OracleRoller(oracleService, GameName.Ironsworn);
            roller.BuildRollResults("Reveal a Danger");

            int roll = roller.RollResultList.First().Roll;

            //Theme & domain pusdo logic:
            //Two themes or domains: odd = first, even = second
            //1-30 theme
            //31-45 domain
            //46+ table
            if (roll <= 30)
            {
                if (Themes.Count == 2 && roll % 2 == 0) //Even roll result
                {
                    roller.RollResultList.First().Result = Themes[1].Dangers.OrderBy(d => d.Chance).First(d => d.Chance > roll).AsStandardOracle();
                }
                else
                {
                    roller.RollResultList.First().Result = Themes[0].Dangers.OrderBy(d => d.Chance).First(d => d.Chance > roll).AsStandardOracle();
                }
            }
            else if (roll <= 45)
            {
                if (Domains.Count == 2 && roll % 2 == 0) //Even roll result
                {
                    roller.RollResultList.First().Result = Domains[1].Dangers.OrderBy(d => d.Chance).First(d => d.Chance >= roll).AsStandardOracle();
                }
                else
                {
                    roller.RollResultList.First().Result = Domains[0].Dangers.OrderBy(d => d.Chance).First(d => d.Chance >= roll).AsStandardOracle();
                }
            }

            return roller;
        }

        public OracleRoller RevealFeatureRoller()
        {
            var roller = new OracleRoller(this.GetFeaturesOracleService(), GameName.Ironsworn).BuildRollResults("Features");

            int roll = roller.RollResultList.First().Roll;

            if ((Themes.Count == 2 || Domains.Count == 2) && roll % 2 == 0)
            {
                roller.RollResultList.First().Result = this.GetFeaturesOracleService().OracleList.First(ol => ol.MatchTableAlias("Features Even")).Oracles.First(o => o.Chance >= roll);
            }

            return roller;
        }

        public OracleService GetFeaturesOracleService()
        {
            var oraclesService = new OracleService();

            var oracleTable = new OracleTable();
            oracleTable.Name = "Features";
            oracleTable.Aliases = new string[] { "Feature" };
            oracleTable.d = 100;
            oracleTable.Oracles = new List<StandardOracle>();
            oracleTable.Game = GameName.Ironsworn;

            oracleTable.Oracles.AddRange(Themes[0].Features.Select(f => f.AsStandardOracle()));
            oracleTable.Oracles.AddRange(Domains[0].Features.Select(f => f.AsStandardOracle()));

            oracleTable.Oracles = oracleTable.Oracles.OrderBy(oracle => oracle.Chance).ToList();

            oraclesService.OracleList.Add(oracleTable);

            if (Themes.Count == 2 || Domains.Count == 2)
            {
                var oracleTableEven = new OracleTable();
                oracleTableEven.Name = "Features Even";
                oracleTableEven.Aliases = new string[] { "Feature Even" };
                oracleTableEven.d = 100;
                oracleTableEven.Oracles = new List<StandardOracle>();

                if (Themes.Count == 2) oracleTableEven.Oracles.AddRange(Themes[1].Features.Select(f => f.AsStandardOracle()));
                else oracleTableEven.Oracles.AddRange(Themes[0].Features.Select(f => f.AsStandardOracle()));

                if (Domains.Count == 2) oracleTableEven.Oracles.AddRange(Domains[1].Features.Select(f => f.AsStandardOracle()));
                else oracleTableEven.Oracles.AddRange(Domains[0].Features.Select(f => f.AsStandardOracle()));

                oracleTableEven.Oracles = oracleTableEven.Oracles.OrderBy(oracle => oracle.Chance).ToList();

                oraclesService.OracleList.Add(oracleTableEven);
            }

            return oraclesService;
        }
    }
}