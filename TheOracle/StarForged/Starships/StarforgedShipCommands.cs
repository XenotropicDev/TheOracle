using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.Core;

namespace TheOracle.StarForged.Starships
{
    public class StarforgedShipCommands : ModuleBase<SocketCommandContext>
    {
        public Emoji projectEmoji = new Emoji("\uD83D\uDEE0");
        public Emoji oneEmoji = new Emoji("\u0031\u20E3");
        public Emoji twoEmoji = new Emoji("\u0032\u20E3");
        public Emoji threeEmoji = new Emoji("\u0033\u20E3");

        public StarforgedShipCommands(ServiceProvider services)
        {
            var hooks = services.GetRequiredService<HookedEvents>();

            if (!hooks.StarShipReactions)
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.ReactionAdded += ShipReactionHandler;
                hooks.StarShipReactions = true;
            }

            Services = services;
        }

        private Task ShipReactionHandler(Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var emojisToProcess = new Emoji[] { projectEmoji, oneEmoji, twoEmoji, threeEmoji };
            if (!reaction.User.IsSpecified || reaction.User.Value.IsBot || !emojisToProcess.Contains(reaction.Emote)) return Task.CompletedTask;

            var message = userMessage.GetOrDownloadAsync().Result;

            var starshipHelperEmbed = message.Embeds.FirstOrDefault(embed => embed?.Title?.Contains(StarShipResources.StarshipTitle) ?? false);

            if (starshipHelperEmbed != null)
            {
                Console.WriteLine($"User {reaction.User} triggered {nameof(starshipHelperEmbed)} with reaction {reaction.Emote.Name}");

                var region = StarforgedUtilites.SpaceRegionFromEmote(reaction.Emote.Name);
                if (region == SpaceRegion.None) return Task.CompletedTask;

                string name = starshipHelperEmbed.Fields.FirstOrDefault(fld => fld.Name == StarShipResources.StarshipName).Value ?? string.Empty;

                Starship newShip = Starship.GenerateShip(Services, region, name);
                Task.WaitAll(message.RemoveAllReactionsAsync());

                var task1 = message.ModifyAsync(msg =>
                {
                    msg.Content = string.Empty;
                    msg.Embed = newShip.GetEmbedBuilder().Build();
                });
                var task2 = message.AddReactionAsync(projectEmoji);
                return Task.WhenAll(task1, task2);
            }

            var shipEmbed = message.Embeds.FirstOrDefault(embed => embed?.Description?.Contains(StarShipResources.StarshipTitle) ?? false);
            if (shipEmbed == null) return Task.CompletedTask;
            
            Console.WriteLine($"User {reaction.User} triggered {nameof(shipEmbed)} with reaction {reaction.Emote.Name}");

            Starship ship = Starship.FromEmbed(Services, shipEmbed);

            //if (reaction.Emote.Name == projectEmoji.Name) ship.ProjectsRevealed++;

            message.ModifyAsync(msg => msg.Embed = ship.GetEmbedBuilder().Build());
            message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);

            return Task.CompletedTask;
        }

        public ServiceProvider Services { get; }

        [Command("GenerateStarship", ignoreExtraArgs: true)]
        [Summary("Creates a template post for a new Starforged settlement")]
        [Alias("Starship", "Spaceship", "Ship")]
        public async Task StarShipPost([Remainder] string StarShipCommand = "")
        {
            var region = StarforgedUtilites.GetAnySpaceRegion(StarShipCommand);

            if (region == SpaceRegion.None)
            {
                EmbedBuilder builder = new EmbedBuilder()
                    .WithTitle(StarShipResources.StarshipTitle)
                    .WithDescription(StarShipResources.PickSpaceRegionMessage);

                if (StarShipCommand.Length > 0) builder.WithFields(new EmbedFieldBuilder().WithName(StarShipResources.StarshipName).WithValue(StarShipCommand));

                var msg = await ReplyAsync(embed: builder.Build());
                await msg.AddReactionAsync(oneEmoji);
                await msg.AddReactionAsync(twoEmoji);
                await msg.AddReactionAsync(threeEmoji);
                return;
            }

            string ShipName = StarShipCommand.Replace(region.ToString(), "").Trim();
            var ship = Starship.GenerateShip(Services, region, ShipName);

            var message = await ReplyAsync("", false, ship.GetEmbedBuilder().Build());

            await message.AddReactionAsync(projectEmoji);
        }
    }
}