using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TheOracle.IronSworn.Delve
{
    public class DelveCommands : InteractiveBase
    {
        [Command("DelveSite", RunMode = RunMode.Async)]
        public async Task Test_NextMessageAsync()
        {
            var ogMessage = await ReplyAsync(embed: new EmbedBuilder()
                .WithTitle(DelveResources.ThemeHelperTitle)
                .WithDescription(DelveResources.ThemeHelperText)
                .WithFooter(DelveResources.HelperFooter)
                .Build());
            var themeInput = string.Empty;
            var response = await NextMessageAsync();
            if (response != null)
            {
                themeInput = response.Content;
                await ogMessage.ModifyAsync(msg => msg.Embed = new EmbedBuilder()
                .WithTitle(DelveResources.DomainHelperTitle)
                .WithDescription(DelveResources.DomainHelperText)
                .WithFooter(DelveResources.HelperFooter)
                .Build());
                response = await NextMessageAsync();

                if (response != null)
                {
                    await ogMessage.ModifyAsync(msg => { msg.Content = $"You replied: {themeInput} & {response.Content}"; msg.Embed = null; });
                }
            }
            else await ReplyAsync("You did not reply before the timeout");
        }

        private List<Theme> ParseTheme(string content)
        {
            var match = Regex.Match(content, @"(\w+),? ?(\w+)?");
            return new List<Theme>();
        }
    }
}
