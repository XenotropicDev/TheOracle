using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOracle.Core;

namespace TheOracle.IronSworn.Delve
{
    public class DelveInfo
    {
        public List<Domain> Domains { get; set; } = new List<Domain>();
        public List<Theme> Themes { get; set; } = new List<Theme>();

        public IEmbed GetEmbed()
        {
            var builder = new EmbedBuilder();

            return builder.Build();
        }

        public IEmbed GetHelper()
        {
            var builder = new EmbedBuilder();

            return builder.Build();
        }

        public static DelveInfo BuildFromEmbed(IEmbed embed)
        {
            return new DelveInfo();
        }

        public override string ToString()
        {
            string info = "Themes: ";
            info += string.Join(',', Themes.Select(t => $"**{t.DelveSiteTheme}**"));

            info += $"\nDomains: ";
            info += string.Join(',', Domains.Select(d => $"**{d.DelveSiteDomain}**"));

            return info;
        }

        internal static DelveInfo FromInput(DelveService delveService, string themeInput, string domainInput)
        {
            var delveInfo = new DelveInfo();
            var themeItems = themeInput.Split(',');
            var domainItems = domainInput.Split(',');
            var randomAliases = DelveResources.RandomAliases.Split(',').ToList();
            int randomThemes = 0;
            int randomDomains = 0;

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

            return delveInfo;
        }

        private void AddRandomDomain(DelveService delveService)
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

        private void AddRandomTheme(DelveService delveService)
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
    }
}