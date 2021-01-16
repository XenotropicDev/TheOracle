using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.GameCore.Oracle;

namespace TheOracle.StarForged.PlayerShips
{
    public class PlayerShipCommands : InteractiveBase
    {
        public Emoji DecreaseIntegrityEmoji = new Emoji("🟦");
        public Emoji IncreaseIntegrityEmoji = new Emoji("☑️");
        public Emoji twoEmoji = new Emoji("\u0032\u20E3");
        public Emoji threeEmoji = new Emoji("\u0033\u20E3");
        public Emoji SupplyDownEmoji = new Emoji("🟩");
        public Emoji SupplyUpEmoji = new Emoji("✅");

        public PlayerShipCommands(IServiceProvider services)
        {
            Services = services;
            OracleService = Services.GetRequiredService<OracleService>();

            var hooks = services.GetRequiredService<HookedEvents>();
            if (!hooks.PlayerShipReactions)
            {
                var reactionService = Services.GetRequiredService<ReactionService>();

                ReactionEvent DecreaseIntegrityEvent = new ReactionEventBuilder().WithEmote(DecreaseIntegrityEmoji).WithEvent(DecreaseIntegrity).Build();
                ReactionEvent IncreaseIntegrityEvent = new ReactionEventBuilder().WithEmote(IncreaseIntegrityEmoji).WithEvent(IncreaseIntegrity).Build();
                ReactionEvent SupplyUpEvent = new ReactionEventBuilder().WithEmote(SupplyUpEmoji).WithEvent(SupplyUp).Build();
                ReactionEvent SupplyDownEvent = new ReactionEventBuilder().WithEmote(SupplyDownEmoji).WithEvent(SupplyDown).Build();
                ReactionEvent AssetTwoEvent = new ReactionEventBuilder().WithEmote(twoEmoji).WithEvent(EnableAssetTwo).Build();
                ReactionEvent AssetThreeEvent = new ReactionEventBuilder().WithEmote(threeEmoji).WithEvent(EnableAssetThree).Build();

                reactionService.reactionList.Add(DecreaseIntegrityEvent);
                reactionService.reactionList.Add(IncreaseIntegrityEvent);
                reactionService.reactionList.Add(SupplyUpEvent);
                reactionService.reactionList.Add(SupplyDownEvent);
                reactionService.reactionList.Add(AssetTwoEvent);
                reactionService.reactionList.Add(AssetThreeEvent);

                hooks.PlayerShipReactions = true;
            }
        }

        private async Task EnableAssetThree(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            await channel.SendMessageAsync("This feature isn't ready, but the reaction is in place so that you don't have to recreate the card in the future.");
        }

        private async Task EnableAssetTwo(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            await channel.SendMessageAsync("This feature isn't ready, but the reaction is in place so that you don't have to recreate the card in the future.");
        }

        private async Task SupplyDown(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var ship = new PlayerShip(Services).PopulateFromEmbed(message.Embeds.FirstOrDefault());
            if (ship == null) return;
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);

            ship.Supply--;
            await message.ModifyAsync(msg => msg.Embed = ship.GetEmbed()).ConfigureAwait(false);
        }

        private async Task SupplyUp(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var ship = new PlayerShip(Services).PopulateFromEmbed(message.Embeds.FirstOrDefault());
            if (ship == null) return;
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);

            ship.Supply++;
            await message.ModifyAsync(msg => msg.Embed = ship.GetEmbed()).ConfigureAwait(false);
        }

        private async Task IncreaseIntegrity(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var ship = new PlayerShip(Services).PopulateFromEmbed(message.Embeds.FirstOrDefault());
            if (ship == null) return;
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);

            ship.Integrity++;
            await message.ModifyAsync(msg => msg.Embed = ship.GetEmbed()).ConfigureAwait(false);
        }

        private async Task DecreaseIntegrity(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var ship = new PlayerShip(Services).PopulateFromEmbed(message.Embeds.FirstOrDefault());
            if (ship == null) return;
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);

            ship.Integrity--;
            await message.ModifyAsync(msg => msg.Embed = ship.GetEmbed()).ConfigureAwait(false);
        }

        public IServiceProvider Services { get; }
        public OracleService OracleService { get; }

        [Summary("Creates a embed to track the main starship")]
        [Command("PlayerShip", RunMode = RunMode.Async)]
        [Alias("CommandShip")]
        [Remarks("🟦 - Decrease Integrity stat\n☑️ - Increase Integrity\n🟩 - Decrease Supply\n✅ - Increase Supply\n\u0032\u20E3 - Not implemented yet\n\u0033\u20E3 - Not implemented yet")]
        public async Task CreatePlayerShip([Remainder] string ShipNameInput = "")
        {
            string[] randomWords = PlayerShipResources.RandomWords.Split(',');
            for (int i = 0; i < randomWords.Length; i++) randomWords[i] = randomWords[i].Trim();

            string[] skipWords = PlayerShipResources.SkipWords.Split(',');
            for (int i = 0; i < skipWords.Length; i++) skipWords[i] = skipWords[i].Trim();

            string[] yesWords = PlayerShipResources.YesWords.Split(',');
            for (int i = 0; i < yesWords.Length; i++) yesWords[i] = yesWords[i].Trim();

            var helperEmbed = new EmbedBuilder()
                .WithTitle(PlayerShipResources.HelperTitle)
                .WithDescription(PlayerShipResources.HelperSetShipName)
                .WithFooter(PlayerShipResources.HelperFooter);

            IUserMessage post = null;
            if (ShipNameInput.Length == 0)
            {
                post = await ReplyAsync(embed: helperEmbed.Build()).ConfigureAwait(false);
                var shipNameMsg = await NextMessageAsync(timeout: TimeSpan.FromMinutes(2));
                if (shipNameMsg == null) { await ShipCreationFailed(post); return; }
                ShipNameInput = shipNameMsg.Content;
                await shipNameMsg.DeleteAsync();
            }

            var valuesSoFarField = new EmbedFieldBuilder().WithName(PlayerShipResources.SelectedItems).WithValue(ShipNameInput);
            helperEmbed.WithDescription(PlayerShipResources.HelperSetHistory)
                .AddField(valuesSoFarField);

            if (post != null) await post.ModifyAsync(msg => msg.Embed = helperEmbed.Build()).ConfigureAwait(false);
            else post = await ReplyAsync(embed: helperEmbed.Build()).ConfigureAwait(false);

            var shipHistoryMessage = await NextMessageAsync(timeout: TimeSpan.FromMinutes(2));
            if (shipHistoryMessage == null) { await ShipCreationFailed(post); return; }
            string shipHistory = shipHistoryMessage.Content;
            if (skipWords.Any(skipWord => shipHistory.Equals(skipWord, StringComparison.OrdinalIgnoreCase))) shipHistory = string.Empty;
            if (randomWords.Any(randWord => shipHistory.Equals(randWord, StringComparison.OrdinalIgnoreCase))) shipHistory = OracleService.RandomOracleResult("Starship History", Services, GameCore.GameName.Starforged);
            await shipHistoryMessage.DeleteAsync().ConfigureAwait(false);

            valuesSoFarField.WithValue($"{valuesSoFarField.Value}\n{shipHistory}".Trim('\n'));
            await post.ModifyAsync(msg => msg.Embed = helperEmbed
                .WithDescription(PlayerShipResources.HelperSetLooks).Build()).ConfigureAwait(false);
            var shipLooksMessage = await NextMessageAsync(timeout: TimeSpan.FromMinutes(2));
            if (shipLooksMessage == null) { await ShipCreationFailed(post); return; }
            string shipLooks = shipLooksMessage.Content;
            if (skipWords.Any(skipWord => shipLooks.Equals(skipWord, StringComparison.OrdinalIgnoreCase))) shipLooks = string.Empty;
            if (randomWords.Any(randWord => shipLooks.Equals(randWord, StringComparison.OrdinalIgnoreCase))) shipLooks = OracleService.RandomOracleResult("Starship Quirks", Services, GameCore.GameName.Starforged);
            await shipLooksMessage.DeleteAsync().ConfigureAwait(false);

            valuesSoFarField.WithValue($"{valuesSoFarField.Value}\n{shipLooks}".Trim('\n'));
            await post.ModifyAsync(msg => msg.Embed = helperEmbed.WithDescription(PlayerShipResources.HelperIncludeSupply).Build()).ConfigureAwait(false);
            var useSupplyMsg = await NextMessageAsync(timeout: TimeSpan.FromMinutes(2));
            if (useSupplyMsg == null) { await ShipCreationFailed(post); return; }
            await useSupplyMsg.DeleteAsync().ConfigureAwait(false);

            var ship = new PlayerShip(Services);
            ship.Name = ShipNameInput;
            ship.UseSupply = yesWords.Any(yesWord => useSupplyMsg.Content.Contains(yesWord, StringComparison.OrdinalIgnoreCase));

            ship.Description = shipHistory.Length > 0 ? string.Format(PlayerShipResources.ShipHistory, shipHistory) : string.Empty;
            if (ship.Description.Length > 0 && shipLooks.Length > 0) ship.Description += "\n";
            ship.Description += shipLooks.Length > 0 ? string.Format(PlayerShipResources.ShipLooks, shipLooks) : string.Empty;

            await post.ModifyAsync(msg => msg.Embed = ship.GetEmbed()).ConfigureAwait(false);
            await Task.Run(async () =>
            {
                await post.RemoveAllReactionsAsync();
                await post.AddReactionAsync(DecreaseIntegrityEmoji);
                await post.AddReactionAsync(IncreaseIntegrityEmoji);
                if (ship.UseSupply) await post.AddReactionAsync(SupplyDownEmoji);
                if (ship.UseSupply) await post.AddReactionAsync(SupplyUpEmoji);
                await post.AddReactionAsync(twoEmoji);
                await post.AddReactionAsync(threeEmoji);
            }).ConfigureAwait(false);
        }

        private async Task ShipCreationFailed(IUserMessage post)
        {
            await post.ModifyAsync(msg => msg.Content = "I can't wait around all day ;-)\n\n Please restart your ship creation");
        }
    }
}