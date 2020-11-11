using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.Core;

namespace TheOracle.StarForged.Starships
{
    public class StarforgedShipCommands : ModuleBase<SocketCommandContext>
    {
        public Emoji missionEmoji = new Emoji("❗");
        public Emoji oneEmoji = new Emoji("\u0031\u20E3");
        public Emoji twoEmoji = new Emoji("\u0032\u20E3");
        public Emoji threeEmoji = new Emoji("\u0033\u20E3");

        public StarforgedShipCommands(ServiceProvider services)
        {
            var hooks = services.GetRequiredService<HookedEvents>();

            if (!hooks.StarShipReactions)
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                var reactionService = services.GetRequiredService<ReactionService>();

                ReactionEvent reaction1 = new ReactionEventBuilder().WithEmote(oneEmoji).WithEvent(ShipReactionHandler).Build();
                ReactionEvent reaction2 = new ReactionEventBuilder().WithEmote(twoEmoji).WithEvent(ShipReactionHandler).Build();
                ReactionEvent reaction3 = new ReactionEventBuilder().WithEmote(threeEmoji).WithEvent(ShipReactionHandler).Build();

                ReactionEvent misison = new ReactionEventBuilder().WithEmote(missionEmoji).WithEvent(ShipReactionHandler).Build();

                reactionService.reactionList.Add(reaction1);
                reactionService.reactionList.Add(reaction2);
                reactionService.reactionList.Add(reaction3);
                reactionService.reactionList.Add(misison);

                hooks.StarShipReactions = true;
            }

            Services = services;
        }

        private async Task ShipReactionHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var starshipHelperEmbed = message.Embeds.FirstOrDefault(embed => embed?.Title?.Contains(StarShipResources.StarshipHelperTitle) ?? false);

            if (starshipHelperEmbed != null)
            {
                var region = StarforgedUtilites.SpaceRegionFromEmote(reaction.Emote.Name);
                if (region == SpaceRegion.None) return;

                string name = starshipHelperEmbed.Fields.FirstOrDefault(fld => fld.Name == StarShipResources.StarshipName).Value ?? string.Empty;

                Starship newShip = Starship.GenerateShip(Services, region, name, channel.Id);
                Task.WaitAll(message.RemoveAllReactionsAsync());

                await message.ModifyAsync(msg =>
                {
                    msg.Content = string.Empty;
                    msg.Embed = newShip.GetEmbedBuilder().Build();
                }).ConfigureAwait(false);
                await message.AddReactionAsync(missionEmoji).ConfigureAwait(false);
                return;
            }

            var shipEmbed = message.Embeds.FirstOrDefault(embed => embed?.Description?.Contains(StarShipResources.Starship, StringComparison.OrdinalIgnoreCase) ?? false);
            if (shipEmbed == null) return;
            
            Starship ship = new Starship(Services, channel.Id).FromEmbed(shipEmbed);

            if (reaction.Emote.Name == missionEmoji.Name)
            {
                ship.AddMission();
                await message.RemoveReactionAsync(reaction.Emote, message.Author).ConfigureAwait(false);
            }

            await message.ModifyAsync(msg => msg.Embed = ship.GetEmbedBuilder().Build()).ConfigureAwait(false);
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);

            return;
        }

        public ServiceProvider Services { get; }

        [Command("GenerateStarship", ignoreExtraArgs: true)]
        [Summary("Creates an interactive post for a new Starforged starship")]
        [Remarks("❗ - Adds a starship mission")]
        [Alias("Starship", "Spaceship", "Ship")]
        public async Task StarShipPost([Remainder] string StarShipCommand = "")
        {
            var region = StarforgedUtilites.GetAnySpaceRegion(StarShipCommand);

            if (region == SpaceRegion.None)
            {
                EmbedBuilder builder = new EmbedBuilder()
                    .WithTitle(StarShipResources.StarshipHelperTitle)
                    .WithDescription(StarShipResources.PickSpaceRegionMessage);

                if (StarShipCommand.Length > 0) builder.WithFields(new EmbedFieldBuilder().WithName(StarShipResources.StarshipName).WithValue(StarShipCommand));

                var msg = await ReplyAsync(embed: builder.Build());
                await msg.AddReactionAsync(oneEmoji);
                await msg.AddReactionAsync(twoEmoji);
                await msg.AddReactionAsync(threeEmoji);
                return;
            }

            string ShipName = Regex.Replace(StarShipCommand, region.ToString(), "", RegexOptions.IgnoreCase).Trim();
            var ship = Starship.GenerateShip(Services, region, ShipName, Context.Channel.Id);

            var message = await ReplyAsync("", false, ship.GetEmbedBuilder().Build());

            await message.AddReactionAsync(missionEmoji);
        }
    }
}