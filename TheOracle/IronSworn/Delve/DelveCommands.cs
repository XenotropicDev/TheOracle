using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.GameCore.Action;
using TheOracle.GameCore.Oracle;

namespace TheOracle.IronSworn.Delve
{
    public class DelveCommands : InteractiveBase
    {
        public Emoji DangerEmoji = new Emoji("\u26A0");
        public Emoji FeatureEmoji = new Emoji("\uD83C\uDF40");
        public Emoji DecreaseEmoji = new Emoji("\u25C0");
        public Emoji FullEmoji = new Emoji("\u0023\u20E3");
        public Emoji IncreaseEmoji = new Emoji("\u25B6");
        public Emoji RollEmoji = new Emoji("\uD83C\uDFB2");

        public DelveCommands(IServiceProvider services)
        {
            Services = services;
            DelveService = Services.GetRequiredService<DelveService>();
            OracleService = Services.GetRequiredService<OracleService>();

            var hooks = services.GetRequiredService<HookedEvents>();
            if (!hooks.DelveReactions)
            {
                var reactionService = Services.GetRequiredService<ReactionService>();

                ReactionEvent decrease = new ReactionEventBuilder().WithEmote(DecreaseEmoji).WithEvent(ReactionDecreaseEvent).Build();
                ReactionEvent increase = new ReactionEventBuilder().WithEmote(IncreaseEmoji).WithEvent(ReactionIncreaseEvent).Build();
                ReactionEvent fullMark = new ReactionEventBuilder().WithEmote(FullEmoji).WithEvent(ReactionFullMarkEvent).Build();
                ReactionEvent Danger = new ReactionEventBuilder().WithEmote(DangerEmoji).WithEvent(ReactionDangerEvent).Build();
                ReactionEvent Feature = new ReactionEventBuilder().WithEmote(FeatureEmoji).WithEvent(ReactionFeatureEvent).Build();
                ReactionEvent roll = new ReactionEventBuilder().WithEmote(RollEmoji).WithEvent(ReactionLocateObjectiveEvent).Build();

                reactionService.reactionList.Add(decrease);
                reactionService.reactionList.Add(increase);
                reactionService.reactionList.Add(fullMark);
                reactionService.reactionList.Add(roll);
                reactionService.reactionList.Add(Danger);
                reactionService.reactionList.Add(Feature);

                hooks.DelveReactions = true;
            }
        }

        public DelveService DelveService { get; }
        public OracleService OracleService { get; }
        public IServiceProvider Services { get; }

        [Command("DelveSite", RunMode = RunMode.Async)]
        [Alias("Delve")]
        [Summary("Creates an delve site tracking post")]
        [Remarks("\u25C0 - Decreases the progress track by the difficulty amount." +
            "\n\u25B6 - Increases the progress track by the difficulty amount." +
            "\n\u0023\u20E3 - Increases the progress track by a single full box (four ticks)." +
            "\n\uD83C\uDFB2 - Rolls the action and challenge die for the Locate your Objective move" +
            "\n\uD83C\uDF40 - Rolls a Feature for the delve site" +
            "\n\u26A0 - Rolls the Reveal a Danger table for the delve site")]
        public async Task DelveSite()
        {
            var delveService = Services.GetRequiredService<DelveService>();
            string themeHelperText = string.Empty;
            string domainHelperText = string.Empty;

            var builder = new DelveInfoBuilder(delveService, OracleService);

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

            var helperMessage = await ReplyAsync(embed: new EmbedBuilder()
                .WithTitle(DelveResources.ThemeHelperTitle)
                .WithDescription(themeHelperText)
                .WithFooter(DelveResources.HelperFooterThemeDomain)
                .Build());

            var themeResponse = await NextMessageAsync(timeout: TimeSpan.FromMinutes(2));
            if (themeResponse != null)
            {
                builder.WithThemes(themeResponse.Content);

                await themeResponse.DeleteAsync().ConfigureAwait(false);

                await helperMessage.ModifyAsync(msg => msg.Embed = new EmbedBuilder()
                .WithTitle(DelveResources.DomainHelperTitle)
                .WithDescription(domainHelperText)
                .AddField(DelveResources.HelperSelectionsTitle, builder)
                .WithFooter(DelveResources.HelperFooterThemeDomain)
                .Build());
            }
            else
            {
                await helperMessage.ModifyAsync(msg => msg.Embed = helperMessage.Embeds.First().ToEmbedBuilder().WithDescription(DelveResources.UserInputTimeoutError).Build());
                return;
            }

            var domainResponse = await NextMessageAsync(timeout: TimeSpan.FromMinutes(2));
            if (domainResponse != null)
            {
                builder.WithDomains(domainResponse.Content);
                await domainResponse.DeleteAsync().ConfigureAwait(false);
                await helperMessage.ModifyAsync(msg => msg.Embed = new EmbedBuilder()
                    .WithTitle(DelveResources.HelperSiteNameTitle)
                    .WithDescription(DelveResources.HelperSiteNameText)
                    .AddField(DelveResources.HelperSelectionsTitle, builder)
                    .Build());
            }
            else
            {
                await helperMessage.ModifyAsync(msg => msg.Embed = helperMessage.Embeds.First().ToEmbedBuilder().WithDescription(DelveResources.UserInputTimeoutError).Build());
                return;
            }

            var siteName = await NextMessageAsync(timeout: TimeSpan.FromMinutes(2));
            if (siteName != null)
            {
                builder.WithName(siteName.Content);
                await siteName.DeleteAsync().ConfigureAwait(false);
                await helperMessage.ModifyAsync(msg => msg.Embed = new EmbedBuilder()
                    .WithTitle(DelveResources.HelperSiteObjectiveTitle)
                    .WithDescription(DelveResources.HelperSiteObjectiveText)
                    .AddField(DelveResources.HelperSelectionsTitle, builder)
                    .Build());
            }
            else
            {
                await helperMessage.ModifyAsync(msg => msg.Embed = helperMessage.Embeds.First().ToEmbedBuilder().WithDescription(DelveResources.UserInputTimeoutError).Build());
                return;
            }

            var siteObjective = await NextMessageAsync(timeout: TimeSpan.FromMinutes(2));
            if (siteObjective != null)
            {
                builder.WithObjective(siteObjective.Content);
                await siteObjective.DeleteAsync().ConfigureAwait(false);
                await helperMessage.ModifyAsync(msg => msg.Embed = new EmbedBuilder()
                    .WithTitle(DelveResources.HelperSiteRankTitle)
                    .WithDescription(DelveResources.HelperSiteRankText)
                    .AddField(DelveResources.HelperSelectionsTitle, builder)
                    .Build());
            }
            else
            {
                await helperMessage.ModifyAsync(msg => msg.Embed = helperMessage.Embeds.First().ToEmbedBuilder().WithDescription(DelveResources.UserInputTimeoutError).Build());
                return;
            }

            var siteRank = await NextMessageAsync(timeout: TimeSpan.FromMinutes(2));
            if (siteRank == null)
            {
                await siteRank.DeleteAsync().ConfigureAwait(false);
                await helperMessage.ModifyAsync(msg => msg.Embed = helperMessage.Embeds.First().ToEmbedBuilder().WithDescription(DelveResources.UserInputTimeoutError).Build());
                return;
            }

            builder.WithRank(siteRank.Content);
            await siteRank.DeleteAsync().ConfigureAwait(false);
            DelveInfo delve = builder.Build();
            await helperMessage.ModifyAsync(msg => { msg.Content = null; msg.Embed = delve.BuildEmbed() as Embed; });

            await helperMessage.AddReactionAsync(DecreaseEmoji);
            await helperMessage.AddReactionAsync(IncreaseEmoji);
            await helperMessage.AddReactionAsync(FullEmoji);
            await helperMessage.AddReactionAsync(FeatureEmoji);
            await helperMessage.AddReactionAsync(DangerEmoji);
            await helperMessage.AddReactionAsync(RollEmoji);
        }

        private bool IsDelveMessage(IUserMessage message)
        {
            if (!message.Embeds.First().Author.HasValue) return false;
            return Utilities.UndoFormatString(message.Embeds.First().Author.Value.Name, DelveResources.CardThemeDomainTitleFormat, out _, true);
        }

        private async Task ReactionDangerEvent(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsDelveMessage(message)) return;
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            var delve = new DelveInfo().FromMessage(DelveService, message);

            var oracles = Services.GetRequiredService<OracleService>();
            await channel.SendMessageAsync(String.Format(DelveResources.RevealDangerRoll, delve.SiteName), false, delve.RevealDangerRoller(oracles).GetEmbed()).ConfigureAwait(false);
        }

        private async Task ReactionFeatureEvent(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsDelveMessage(message)) return;
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            var delve = new DelveInfo().FromMessage(DelveService, message);

            await channel.SendMessageAsync(String.Format(DelveResources.RevealFeatureRoll, delve.SiteName), false, delve.RevealFeatureRoller().GetEmbed()).ConfigureAwait(false);
        }

        private async Task ReactionDecreaseEvent(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsDelveMessage(message)) return;
            DelveInfo delve = new DelveInfo().FromMessage(DelveService, message);
            delve.Ticks -= delve.TicksPerProgress;
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = delve.BuildEmbed() as Embed);
        }

        private async Task ReactionFullMarkEvent(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsDelveMessage(message)) return;
            DelveInfo delve = new DelveInfo().FromMessage(DelveService, message);
            delve.Ticks += 4;
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = delve.BuildEmbed() as Embed);
        }

        private async Task ReactionIncreaseEvent(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsDelveMessage(message)) return;
            DelveInfo delve = new DelveInfo().FromMessage(DelveService, message);
            delve.Ticks += delve.TicksPerProgress;
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = delve.BuildEmbed() as Embed);
        }

        private async Task ReactionLocateObjectiveEvent(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsDelveMessage(message)) return;
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);

            DelveInfo delve = new DelveInfo().FromMessage(DelveService, message);
            var roll = new ActionRoll(0, delve.ActionDie, String.Format(DelveResources.LocateObjectiveRoll, delve.SiteName));
            await channel.SendMessageAsync(roll.ToString()).ConfigureAwait(false);
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
        }
    }
}