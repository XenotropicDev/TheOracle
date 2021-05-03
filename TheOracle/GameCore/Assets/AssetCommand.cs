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
                ReactionEvent reaction1 = new ReactionEventBuilder().WithEmoji("⬆️").WithEvent(NumericTrackIncrease).Build();
                ReactionEvent reaction2 = new ReactionEventBuilder().WithEmoji("⬇️").WithEvent(NumericTrackDecrease).Build();
                ReactionEvent reaction3 = new ReactionEventBuilder().WithEmoji("⬅️").WithEvent(MultiTrackLeft).Build();
                ReactionEvent reaction4 = new ReactionEventBuilder().WithEmoji("➡️").WithEvent(MultiTrackRight).Build();
                ReactionEvent reaction5 = new ReactionEventBuilder().WithEmoji("➕").WithEvent(CountingTrackUp).Build();
                ReactionEvent reaction6 = new ReactionEventBuilder().WithEmoji("➖").WithEvent(CountingTrackDown).Build();

                ReactionEvent reaction7 = new ReactionEventBuilder().WithEmote(GenericReactions.oneEmoji).WithEvent(AssetFieldEventAdd).Build();
                ReactionEvent reaction8 = new ReactionEventBuilder().WithEmote(GenericReactions.twoEmoji).WithEvent(AssetFieldEventAdd).Build();
                ReactionEvent reaction9 = new ReactionEventBuilder().WithEmote(GenericReactions.threeEmoji).WithEvent(AssetFieldEventAdd).Build();
                ReactionEvent reaction10 = new ReactionEventBuilder().WithEmote(GenericReactions.fourEmoji).WithEvent(AssetFieldEventAdd).Build();

                ReactionEvent reaction7rem = new ReactionEventBuilder().WithEmote(GenericReactions.oneEmoji).WithRemoveEvent(AssetFieldEventRem).Build();
                ReactionEvent reaction8rem = new ReactionEventBuilder().WithEmote(GenericReactions.twoEmoji).WithRemoveEvent(AssetFieldEventRem).Build();
                ReactionEvent reaction9rem = new ReactionEventBuilder().WithEmote(GenericReactions.threeEmoji).WithRemoveEvent(AssetFieldEventRem).Build();
                ReactionEvent reaction10rem = new ReactionEventBuilder().WithEmote(GenericReactions.fourEmoji).WithRemoveEvent(AssetFieldEventRem).Build();

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

        private async Task AssetFieldEventRem(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!Asset.IsAssetMessage(message, Services)) return;

            var asset = Asset.FromEmbed(Services, message.Embeds.First());

            if (reaction.Emote.IsSameAs(GenericReactions.oneEmoji)) asset.AssetFields[0].Enabled = false;
            if (reaction.Emote.IsSameAs(GenericReactions.twoEmoji)) asset.AssetFields[1].Enabled = false;
            if (reaction.Emote.IsSameAs(GenericReactions.threeEmoji)) asset.AssetFields[2].Enabled = false;
            if (reaction.Emote.IsSameAs(GenericReactions.fourEmoji)) asset.AssetFields[3].Enabled = false;

            await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed(asset.arguments.ToArray())).ConfigureAwait(false);
        }

        private async Task AssetFieldEventAdd(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!Asset.IsAssetMessage(message, Services)) return;

            var asset = Asset.FromEmbed(Services, message.Embeds.First());

            if (reaction.Emote.IsSameAs(GenericReactions.oneEmoji)) asset.AssetFields[0].Enabled = true;
            if (reaction.Emote.IsSameAs(GenericReactions.twoEmoji)) asset.AssetFields[1].Enabled = true;
            if (reaction.Emote.IsSameAs(GenericReactions.threeEmoji)) asset.AssetFields[2].Enabled = true;
            if (reaction.Emote.IsSameAs(GenericReactions.fourEmoji)) asset.AssetFields[3].Enabled = true;

            await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed(asset.arguments.ToArray())).ConfigureAwait(false);
        }

        private async Task CountingTrackDown(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var asset = Asset.FromEmbed(Services, message.Embeds.First());

            asset.CountingAssetTrack.StartingValue--;

            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed(asset.arguments.ToArray())).ConfigureAwait(false);
        }

        private async Task CountingTrackUp(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var asset = Asset.FromEmbed(Services, message.Embeds.First());

            asset.CountingAssetTrack.StartingValue++;

            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed(asset.arguments.ToArray())).ConfigureAwait(false);
        }

        private async Task MultiTrackRight(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var asset = Asset.FromEmbed(Services, message.Embeds.First());

            //TODO

            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed(asset.arguments.ToArray())).ConfigureAwait(false);
        }

        private async Task MultiTrackLeft(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var asset = Asset.FromEmbed(Services, message.Embeds.First());

            //TODO

            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed(asset.arguments.ToArray())).ConfigureAwait(false);
        }

        private async Task NumericTrackDecrease(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var asset = Asset.FromEmbed(Services, message.Embeds.First());

            if (asset.NumericAssetTrack.ActiveNumber - 1 >= asset.NumericAssetTrack.Min)
            {
                asset.NumericAssetTrack.ActiveNumber--;
                await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed(asset.arguments.ToArray())).ConfigureAwait(false);
            }

            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
        }

        private async Task NumericTrackIncrease(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var asset = Asset.FromEmbed(Services, message.Embeds.First());

            if (asset.NumericAssetTrack.ActiveNumber + 1 <= asset.NumericAssetTrack.Max)
            {
                asset.NumericAssetTrack.ActiveNumber++;
                await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed(asset.arguments.ToArray())).ConfigureAwait(false);
            }

            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
        }

        public IServiceProvider Services { get; }

        [Command("Asset")]
        [Summary("Creates an interactive post for asset tracking and reference")]
        [Remarks("Use `!Asset <Asset Name> First asset text, second asset text` to populate the asset card with text fields.")]
        public async Task StandardAsset([Remainder] string AssetCommand)
        {
            var assets = Services.GetRequiredService<List<Asset>>();

            ChannelSettings channelSettings = await ChannelSettings.GetChannelSettingsAsync(Context.Channel.Id);
            var game = Utilities.GetGameContainedInString(AssetCommand);
            if (game != GameName.None) AssetCommand = Utilities.RemoveGameNamesFromString(AssetCommand);
            if (game == GameName.None && channelSettings != null) game = channelSettings.DefaultGame;

            Asset asset = FindMatchingAsset(AssetCommand, assets, game);

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

            var message = await ReplyAsync(embed: asset.GetEmbed(arguments));

            await Task.Run(async () =>
            {
                if (asset.NumericAssetTrack != null)
                {
                    await message.AddReactionAsync(new Emoji("⬆️"));
                    await message.AddReactionAsync(new Emoji("⬇️"));
                }

                if (asset.MultiFieldAssetTrack != null)
                {
                    await message.AddReactionAsync(new Emoji("⬅️"));
                    await message.AddReactionAsync(new Emoji("➡️"));
                }

                if (asset.CountingAssetTrack != null)
                {
                    await message.AddReactionAsync(new Emoji("➖"));
                    await message.AddReactionAsync(new Emoji("➕"));
                }

                for (int i = 0; i < asset.AssetFields.Count; i++)
                {
                    if (asset.AssetFields[i].Enabled) continue;
                    if (i == 0) await message.AddReactionAsync(GenericReactions.oneEmoji);
                    if (i == 1) await message.AddReactionAsync(GenericReactions.twoEmoji);
                    if (i == 2) await message.AddReactionAsync(GenericReactions.threeEmoji);
                    if (i == 3) await message.AddReactionAsync(GenericReactions.fourEmoji);
                }
            }).ConfigureAwait(false);
        }

        public Asset FindMatchingAsset(string AssetCommand, List<Asset> assets, GameName game)
        {
            var asset = assets.Where(a => new Regex(@"(\W|\b)" + a.Name + @"(\W|\b)", RegexOptions.IgnoreCase).IsMatch(AssetCommand) && (game == GameName.None || game == a.Game)); //Strong match
            if (asset.Count() == 0) asset = assets.Where(a => new Regex(@"(\W|\b)" + a.Name, RegexOptions.IgnoreCase).IsMatch(AssetCommand) && (game == GameName.None || game == a.Game));
            if (asset.Count() == 0) asset = assets.Where(a => AssetCommand.Contains(a.Name, StringComparison.OrdinalIgnoreCase) && (game == GameName.None || game == a.Game)); //Weakest match - This is mostly for languages that don't have spaces between words

            if (asset.Count() == 0)
            {
                if (game == GameName.None) return default;
                return FindMatchingAsset(AssetCommand, assets, GameName.None);
            }

            if (asset.Count() > 1) throw new ArgumentException(string.Format(AssetResources.TooManyAssetsError, AssetCommand));

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

            var sourceList = Services.GetRequiredService<List<Asset>>().Where(a => a.Game == game || game == GameName.None).ToList();
            var assetList = new List<Asset>();

            if (AssetListOptions.Length > 0)
            {
                assetList.AddRange(sourceList.Where(a => a.AssetType.Contains(AssetListOptions, StringComparison.OrdinalIgnoreCase)));
                assetList.AddRange(sourceList.Where(a => a.Name.Contains(AssetListOptions, StringComparison.OrdinalIgnoreCase)));
            }
            else
            {
                assetList = new List<Asset>(sourceList);
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