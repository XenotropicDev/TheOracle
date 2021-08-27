using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheOracle.BotCore;

namespace TheOracle.GameCore.Assets
{
    public class AssetCommands : ModuleBase<SocketCommandContext>
    {
        public AssetCommands(IServiceProvider services)
        {
            Services = services;

            var hooks = Services.GetRequiredService<HookedEvents>();
            if (!hooks.AssetReactions)
            {
                ReactionEvent reaction1 = new ReactionEventBuilder().WithEmoji("⬆️").WithEvent(MeterIncrease).Build();
                ReactionEvent reaction2 = new ReactionEventBuilder().WithEmoji("⬇️").WithEvent(MeterDecrease).Build();
                ReactionEvent reaction3 = new ReactionEventBuilder().WithEmoji("⬅️").WithEvent(RadioLeft).Build();
                ReactionEvent reaction4 = new ReactionEventBuilder().WithEmoji("➡️").WithEvent(RadioRight).Build();
                ReactionEvent reaction5 = new ReactionEventBuilder().WithEmoji("➕").WithEvent(CounterUp).Build();
                ReactionEvent reaction6 = new ReactionEventBuilder().WithEmoji("➖").WithEvent(CounterDown).Build();

                ReactionEvent reaction7 = new ReactionEventBuilder().WithEmote(GenericReactions.oneEmoji).WithEvent(AssetAbilityEventAdd).Build();
                ReactionEvent reaction8 = new ReactionEventBuilder().WithEmote(GenericReactions.twoEmoji).WithEvent(AssetAbilityEventAdd).Build();
                ReactionEvent reaction9 = new ReactionEventBuilder().WithEmote(GenericReactions.threeEmoji).WithEvent(AssetAbilityEventAdd).Build();
                ReactionEvent reaction10 = new ReactionEventBuilder().WithEmote(GenericReactions.fourEmoji).WithEvent(AssetAbilityEventAdd).Build();

                ReactionEvent reaction7rem = new ReactionEventBuilder().WithEmote(GenericReactions.oneEmoji).WithRemoveEvent(AssetAbilityEventRem).Build();
                ReactionEvent reaction8rem = new ReactionEventBuilder().WithEmote(GenericReactions.twoEmoji).WithRemoveEvent(AssetAbilityEventRem).Build();
                ReactionEvent reaction9rem = new ReactionEventBuilder().WithEmote(GenericReactions.threeEmoji).WithRemoveEvent(AssetAbilityEventRem).Build();
                ReactionEvent reaction10rem = new ReactionEventBuilder().WithEmote(GenericReactions.fourEmoji).WithRemoveEvent(AssetAbilityEventRem).Build();

                services.GetRequiredService<ReactionService>().reactionList.Add(reaction1);
                services.GetRequiredService<ReactionService>().reactionList.Add(reaction2);
                services.GetRequiredService<ReactionService>().reactionList.Add(reaction3);
                services.GetRequiredService<ReactionService>().reactionList.Add(reaction4);
                services.GetRequiredService<ReactionService>().reactionList.Add(reaction5);
                services.GetRequiredService<ReactionService>().reactionList.Add(reaction6);

                services.GetRequiredService<ReactionService>().reactionList.Add(reaction7);
                services.GetRequiredService<ReactionService>().reactionList.Add(reaction8);
                services.GetRequiredService<ReactionService>().reactionList.Add(reaction9);
                services.GetRequiredService<ReactionService>().reactionList.Add(reaction10);

                services.GetRequiredService<ReactionService>().reactionRemovedList.Add(reaction7rem);
                services.GetRequiredService<ReactionService>().reactionRemovedList.Add(reaction8rem);
                services.GetRequiredService<ReactionService>().reactionRemovedList.Add(reaction9rem);
                services.GetRequiredService<ReactionService>().reactionRemovedList.Add(reaction10rem);

                hooks.AssetReactions = true;
            }
        }

        private async Task AssetAbilityEventRem(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!Asset.IsAssetMessage(message, Services)) return;

            var asset = Asset.FromEmbed(Services, message.Embeds.First());

            if (reaction.Emote.IsSameAs(GenericReactions.oneEmoji)) asset.AssetAbilities[0].Enabled = false;
            if (reaction.Emote.IsSameAs(GenericReactions.twoEmoji)) asset.AssetAbilities[1].Enabled = false;
            if (reaction.Emote.IsSameAs(GenericReactions.threeEmoji)) asset.AssetAbilities[2].Enabled = false;
            if (reaction.Emote.IsSameAs(GenericReactions.fourEmoji)) asset.AssetAbilities[3].Enabled = false;

            await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed()).ConfigureAwait(false);
        }

        private async Task AssetAbilityEventAdd(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!Asset.IsAssetMessage(message, Services)) return;

            var asset = Asset.FromEmbed(Services, message.Embeds.First());

            if (reaction.Emote.IsSameAs(GenericReactions.oneEmoji)) asset.AssetAbilities[0].Enabled = true;
            if (reaction.Emote.IsSameAs(GenericReactions.twoEmoji)) asset.AssetAbilities[1].Enabled = true;
            if (reaction.Emote.IsSameAs(GenericReactions.threeEmoji)) asset.AssetAbilities[2].Enabled = true;
            if (reaction.Emote.IsSameAs(GenericReactions.fourEmoji)) asset.AssetAbilities[3].Enabled = true;

            await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed()).ConfigureAwait(false);
        }

        private async Task CounterDown(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var asset = Asset.FromEmbed(Services, message.Embeds.First());

            asset.AssetCounter.StartingValue--;

            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed()).ConfigureAwait(false);
        }

        private async Task CounterUp(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var asset = Asset.FromEmbed(Services, message.Embeds.First());

            asset.AssetCounter.StartingValue++;

            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed()).ConfigureAwait(false);
        }

        private async Task RadioRight(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var asset = Asset.FromEmbed(Services, message.Embeds.First());

            //TODO

            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed()).ConfigureAwait(false);
        }

        private async Task RadioLeft(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var asset = Asset.FromEmbed(Services, message.Embeds.First());

            //TODO

            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed()).ConfigureAwait(false);
        }

        private async Task MeterDecrease(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var asset = Asset.FromEmbed(Services, message.Embeds.First());

            if (asset.AssetConditionMeter.ActiveNumber - 1 >= asset.AssetConditionMeter.Min)
            {
                asset.AssetConditionMeter.ActiveNumber--;
                await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed()).ConfigureAwait(false);
            }

            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
        }

        private async Task MeterIncrease(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var asset = Asset.FromEmbed(Services, message.Embeds.First());

            if (asset.AssetConditionMeter.ActiveNumber + 1 <= asset.AssetConditionMeter.Max)
            {
                asset.AssetConditionMeter.ActiveNumber++;
                await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed()).ConfigureAwait(false);
            }

            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
        }

        public IServiceProvider Services { get; }

        [Command("Asset")]
        [Summary("Creates an interactive post for asset tracking and reference")]
        [Remarks("Use `!Asset <Asset Name> First asset text, second asset text` to populate the asset card with text fields.")]
        public async Task StandardAsset([Remainder] string AssetCommand)
        {
            var assets = Services.GetRequiredService<List<IAsset>>();

            ChannelSettings channelSettings = await ChannelSettings.GetChannelSettingsAsync(Context.Channel.Id);
            var game = Utilities.GetGameContainedInString(AssetCommand);
            if (game != GameName.None) AssetCommand = Utilities.RemoveGameNamesFromString(AssetCommand);
            if (game == GameName.None && channelSettings != null) game = channelSettings.DefaultGame;

            IAsset asset = FindMatchingAsset(AssetCommand, assets, game);

            if (asset == default)
            {
                var dict = Services.GetRequiredService<WeCantSpell.Hunspell.WordList>();
                var suggestions = dict.Suggest(AssetCommand);
                string suggestion = (suggestions?.Count() > 0) ? string.Format(AssetResources.DidYouMean, suggestions.FirstOrDefault()) : string.Empty;
                await ReplyAsync(string.Format(AssetResources.UnknownAssetError, suggestion));
                return;
            }

            string additionalInputsRaw = AssetCommand.ReplaceFirst(asset.Name, "", StringComparison.OrdinalIgnoreCase).Replace("  ", " ").Trim();

            string[] seperators = new string[] { ",", "\n" };
            if (additionalInputsRaw.Contains(",") || additionalInputsRaw.Contains("\n")) additionalInputsRaw.Replace("\"", string.Empty);

            string[] arguments = additionalInputsRaw.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
            asset.Arguments = arguments;

            var message = await ReplyAsync(embed: asset.GetEmbed());

            await Task.Run(async () =>
            {
                if (asset.AssetConditionMeter != null)
                {
                    await message.AddReactionAsync(new Emoji("⬆️"));
                    await message.AddReactionAsync(new Emoji("⬇️"));
                }

                if (asset.AssetCounter != null)
                {
                    await message.AddReactionAsync(new Emoji("➖"));
                    await message.AddReactionAsync(new Emoji("➕"));
                }

                for (int i = 0; i < asset.AssetAbilities.Count; i++)
                {
                    if (asset.AssetAbilities[i].Enabled) continue;
                    if (i == 0) await message.AddReactionAsync(GenericReactions.oneEmoji);
                    if (i == 1) await message.AddReactionAsync(GenericReactions.twoEmoji);
                    if (i == 2) await message.AddReactionAsync(GenericReactions.threeEmoji);
                    if (i == 3) await message.AddReactionAsync(GenericReactions.fourEmoji);
                }

                await message.AddReactionAsync(new Emoji(GenericReactions.recreatePostEmoji));
            }).ConfigureAwait(false);
        }

        public IAsset FindMatchingAsset(string AssetCommand, List<IAsset> assets, GameName game)
        {
            var asset = assets.Where(a => new Regex(@"(\W|\b)" + a.Name + @"(\W|\b)", RegexOptions.IgnoreCase).IsMatch(AssetCommand) && (game == GameName.None || game == a.Game)); //Strong match
            if (!asset.Any()) asset = assets.Where(a => new Regex(@"(\W|\b)" + a.Name, RegexOptions.IgnoreCase).IsMatch(AssetCommand) && (game == GameName.None || game == a.Game));
            if (!asset.Any()) asset = assets.Where(a => AssetCommand.Contains(a.Name, StringComparison.OrdinalIgnoreCase) && (game == GameName.None || game == a.Game)); //Weakest match - This is mostly for languages that don't have spaces between words

            if (!asset.Any())
            {
                if (game == GameName.None) return default;
                return FindMatchingAsset(AssetCommand, assets, GameName.None);
            }

            if (asset.Count() > 1)
            {
                if (asset.Count(a => a.Name.Equals(AssetCommand, StringComparison.OrdinalIgnoreCase)) == 1)
                    return asset.Single(a => a.Name.Equals(AssetCommand, StringComparison.OrdinalIgnoreCase));

                throw new ArgumentException(string.Format(AssetResources.TooManyAssetsError, AssetCommand));
            }

            return asset.FirstOrDefault();
        }

        [Command("AssetList")]
        [Summary("Posts a list of assets")]
        [Remarks("Usage: `!AssetList` to show all assets `!AssetList [Options]` to specify specific assets.\nOptions: Game name, asset category, search string")]
        public async Task AssetList([Remainder] string AssetListOptions = "")
        {
            var game = Utilities.GetGameContainedInString(AssetListOptions);
            AssetListOptions = Utilities.RemoveGameNamesFromString(AssetListOptions).Trim();
            if (game == GameName.None)
            {
                var settings = await ChannelSettings.GetChannelSettingsAsync(Context.Channel.Id);
                game = settings?.DefaultGame ?? GameName.None;
            }

            var sourceList = Services.GetRequiredService<List<IAsset>>().Where(a => a.Game == game || game == GameName.None).ToList();
            var assetList = new List<IAsset>();

            if (AssetListOptions.Length > 0)
            {
                assetList.AddRange(sourceList.Where(a => a.Category.Contains(AssetListOptions, StringComparison.OrdinalIgnoreCase)));
                assetList.AddRange(sourceList.Where(a => a.Name.Contains(AssetListOptions, StringComparison.OrdinalIgnoreCase)));
            }
            else
            {
                assetList = new List<IAsset>(sourceList);
            }

            if (assetList.Count > 0)
            {
                string replyMessage = string.Join(", ", assetList.Select(a => a.Name));
                await ReplyAsync(replyMessage).ConfigureAwait(false);
            }
            else
            {
                var dict = Services.GetRequiredService<WeCantSpell.Hunspell.WordList>();
                var suggestions = dict.Suggest(AssetListOptions);
                string suggestion = (suggestions?.Count() > 0) ? string.Format(AssetResources.DidYouMean, suggestions.FirstOrDefault()) : string.Empty;
                await ReplyAsync(string.Format(AssetResources.UnknownAssetError, suggestion));
                return;
            }
        }
    }
}