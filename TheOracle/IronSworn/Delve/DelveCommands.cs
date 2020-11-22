using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TheOracle.IronSworn.Delve
{
    public class DelveCommands : InteractiveBase
    {
        public DelveCommands(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }

        [Command("DelveSite", RunMode = RunMode.Async)]
        public async Task Test_NextMessageAsync()
        {
            var delveService = Services.GetRequiredService<DelveService>();
            string themeHelperText = string.Empty;
            string domainHelperText = string.Empty;

            for (int i = 0; i < delveService.Themes.Count; i++)
            {
                themeHelperText += String.Format(DelveResources.HelperTextFormat, i + 1, delveService.Themes[i].DelveSiteTheme) + "\n";
            }
            themeHelperText += "\n" + String.Format(DelveResources.HelperTextFormat, DelveResources.RandomAliases.Split(',')[0], DelveResources.RandomAliases.Split(',')[1]);

            for (int i = 0; i < delveService.Domains.Count; i++)
            {
                domainHelperText += String.Format(DelveResources.HelperTextFormat, i + 1, delveService.Domains[i].DelveSiteDomain) + "\n";
            }
            domainHelperText += "\n" + String.Format(DelveResources.HelperTextFormat, DelveResources.RandomAliases.Split(',')[0], DelveResources.RandomAliases.Split(',')[1]);

            var ogMessage = await ReplyAsync(embed: new EmbedBuilder()
                .WithTitle(DelveResources.ThemeHelperTitle)
                .WithDescription(themeHelperText)
                .WithFooter(DelveResources.HelperFooter)
                .Build());
            var themeInput = string.Empty;
            var response = await NextMessageAsync(timeout: TimeSpan.FromMinutes(2));
            if (response != null)
            {
                themeInput = response.Content;
                await ogMessage.ModifyAsync(msg => msg.Embed = new EmbedBuilder()
                .WithTitle(DelveResources.DomainHelperTitle)
                .WithDescription(domainHelperText)
                .WithFooter(DelveResources.HelperFooter)
                .Build());
                response = await NextMessageAsync(timeout: TimeSpan.FromMinutes(2));

                if (response != null)
                {
                    DelveInfo delve = DelveInfo.FromInput(delveService, themeInput, response.Content);
                    
                    await ogMessage.ModifyAsync(msg => { msg.Content = $"{delve}"; msg.Embed = null; });
                    return;
                }
            }
            await ogMessage.ModifyAsync(msg => msg.Embed = ogMessage.Embeds.First().ToEmbedBuilder().WithDescription("You did not reply before the timeout, please start again.").Build());
        }

        private List<Theme> ParseTheme(string content)
        {
            var match = Regex.Match(content, @"(\w+),? ?(\w+)?");
            return new List<Theme>();
        }
    }
}