using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
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

                services.GetRequiredService<ReactionService>().reactionList.Add(reaction1);
                services.GetRequiredService<ReactionService>().reactionList.Add(reaction2);
                services.GetRequiredService<ReactionService>().reactionList.Add(reaction3);
                services.GetRequiredService<ReactionService>().reactionList.Add(reaction4);
                services.GetRequiredService<ReactionService>().reactionList.Add(reaction5);
                services.GetRequiredService<ReactionService>().reactionList.Add(reaction6);
                hooks.AssetReactions = true;
            }
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

            //

            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed(asset.arguments.ToArray())).ConfigureAwait(false);
        }

        private async Task MultiTrackLeft(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var asset = Asset.FromEmbed(Services, message.Embeds.First());

            //

            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed(asset.arguments.ToArray())).ConfigureAwait(false);
        }

        private async Task NumericTrackDecrease(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var asset = Asset.FromEmbed(Services, message.Embeds.First());

            asset.NumericAssetTrack.ActiveNumber--;

            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed(asset.arguments.ToArray())).ConfigureAwait(false);
        }

        private async Task NumericTrackIncrease(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var asset = Asset.FromEmbed(Services, message.Embeds.First());

            asset.NumericAssetTrack.ActiveNumber++;

            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = asset.GetEmbed(asset.arguments.ToArray())).ConfigureAwait(false);
        }

        public IServiceProvider Services { get; }

        [Command("Asset")]
        [Summary("Creates an interactive post for asset tracking and reference")]
        [Remarks("Use `!Asset <Asset Name> First asset text, second asset text` to populate the asset card with text fields.")]
        public async Task StandardAsset([Remainder] string AssetCommand)
        {
            var assets = Services.GetRequiredService<List<Asset>>();

            var asset = assets.FirstOrDefault(a => AssetCommand.Contains(a.Name, StringComparison.OrdinalIgnoreCase));
            if (asset == default) throw new ArgumentException(AssetResources.UnknownAssetError);

            string additionalInputsRaw = AssetCommand.ReplaceFirst(asset.Name, "", StringComparison.OrdinalIgnoreCase).Replace("  ", " ").Trim();

            string[] seperators = new string[] { " " };
            if (additionalInputsRaw.Contains(",") || additionalInputsRaw.Contains("\n"))
            {
                seperators = new string[] { ",", "\n" };
                additionalInputsRaw.Replace("\"", string.Empty);
            }
            string[] arguments = additionalInputsRaw.Split(seperators, StringSplitOptions.RemoveEmptyEntries);

            var message = await ReplyAsync(embed: asset.GetEmbed(arguments));

            var numericTasks = new List<Task>();
            if (asset.NumericAssetTrack != null)
            {
                numericTasks.Add(message.AddReactionAsync(new Emoji("⬆️")));
                numericTasks.Add(message.AddReactionAsync(new Emoji("⬇️")));
            }

            List<Task> multiFieldTasks = new List<Task>();
            if (asset.MultiFieldAssetTrack != null)
            {
                await Task.WhenAll(numericTasks);
                multiFieldTasks.Add(message.AddReactionAsync(new Emoji("⬅️")));
                multiFieldTasks.Add(message.AddReactionAsync(new Emoji("➡️")));
            }

            if (asset.CountingAssetTrack != null)
            {
                await Task.WhenAll(numericTasks.Union(multiFieldTasks));

                await Task.Run(async () =>
                {
                    await message.AddReactionAsync(new Emoji("➕"));
                    await message.AddReactionAsync(new Emoji("➖"));
                }).ConfigureAwait(false);
            }
        }
    }
}