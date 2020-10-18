using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.StarForged;

namespace TheOracle.IronSworn
{
    public class PlanetCommands : ModuleBase<SocketCommandContext>
    {
        public const string oneEmoji = "\u0031\u20E3";
        public const string twoEmoji = "\u0032\u20E3";
        public const string threeEmoji = "\u0033\u20E3";

        public PlanetCommands(DiscordSocketClient client, HookedEvents hooks)
        {
            Client = client;
            if (!hooks.PlanetReactions)
            {
                hooks.PlanetReactions = true;
                Client.ReactionAdded += PlanetReactionHandler;
            }
        }

        public DiscordSocketClient Client { get; }

        [Command("GeneratePlanet", ignoreExtraArgs: true)]
        [Summary("Creates a template post for a new Starforged planet\n🔍 Adds a Closer Look\n\U0001F996 Reveals any life-forms")]
        [Alias("Planet")]
        public async Task PlanetPost([Remainder] string PlanetCommand = "")
        {
            SpaceRegion spaceRegion = StarforgedUtilites.GetAnySpaceRegion(PlanetCommand);

            string PlanetName = PlanetCommand.Replace(spaceRegion.ToString(), "", StringComparison.OrdinalIgnoreCase).Trim();
            if (PlanetName == string.Empty) PlanetName = $"P-{DateTime.Now.Ticks.ToString().Substring(7)}";

            if (spaceRegion == SpaceRegion.None)
            {
                var palceHolderEmbed = new EmbedBuilder()
                    .WithTitle($"__{PlanetName}__")
                    .WithFields(new EmbedFieldBuilder()
                        .WithName("Options:")
                        .WithValue($"{oneEmoji}: Terminus\n{twoEmoji}: Outlands\n{threeEmoji}: Expanse")
                        );

                var msg = await ReplyAsync(embed: palceHolderEmbed.Build());
                await msg.AddReactionAsync(new Emoji(oneEmoji));
                await msg.AddReactionAsync(new Emoji(twoEmoji));
                await msg.AddReactionAsync(new Emoji(threeEmoji));
                return;
            }

            await MakePlanetPost(spaceRegion, PlanetName);
        }

        private async Task MakePlanetPost(SpaceRegion region, string PlanetName, IUserMessage message = null)
        {
            Planet planet = Planet.GeneratePlanet(PlanetName);

            EmbedBuilder embedBuilder = new EmbedBuilder();

            embedBuilder.Title = $"__{PlanetName}__";

            embedBuilder.AddField(planet.PlanetType, planet.Description);
            embedBuilder.AddField("Atmosphere", planet.Atmosphere.Description, true);

            if (region == SpaceRegion.Terminus) embedBuilder.AddField("Terminus Settlements", planet.Settlements.Terminus.Description);
            if (region == SpaceRegion.Outlands) embedBuilder.AddField("Outlands Settlements", planet.Settlements.Outlands.Description);
            if (region == SpaceRegion.Expanse) embedBuilder.AddField("Expanse Settlements", planet.Settlements.Expanse.Description);

            for (int i = 0; i < planet.SpaceObservations.Count; i++)
            {
                embedBuilder.AddField($"Space Observation {i + 1}:", planet.SpaceObservations[i].Description, true);
            }

            embedBuilder.ThumbnailUrl = planet.Thumbnail;

            if (message != null)
            {
                await message.RemoveAllReactionsAsync();
                await message.ModifyAsync(msg => msg.Embed = embedBuilder.Build());
            }
            else
            {
                message = await ReplyAsync("", false, embedBuilder.Build());
            }

            var lookingGlass = new Emoji("🔍"); //TODO make these user settable
            await message.AddReactionAsync(lookingGlass);

            var dino = new Emoji("\U0001F996");
            await message.AddReactionAsync(dino);

            if (planet.NumberOfBiomes > 1)
            {
                var biome = new Emoji("\uD83C\uDF0D");
                await message.AddReactionAsync(biome);
            }
        }

        private int PlanetFieldOrder(string fieldName)
        {
            if (fieldName.Contains("World")) return 1;
            if (fieldName.Contains("Atmosphere")) return 2;
            if (fieldName == "Life") return 3;
            if (fieldName == "Terminus Settlements") return 4;
            if (fieldName == "Outlands Settlements") return 5;
            if (fieldName == "Expanse Settlements") return 6;
            if (fieldName.Contains("Observation 1")) return 7;
            if (fieldName.Contains("Observation 2")) return 8;
            if (fieldName.Contains("Observation 3")) return 9;
            if (fieldName.Contains("Closer Look")) return 10;
            return 100;
        }

        private void Biome(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var oldEmbed = message.Embeds.FirstOrDefault();

            var planet = Planet.GeneratePlanet(oldEmbed.Title);

            int currentBiomes = oldEmbed.Fields.Count(field => field.Name == "Biome");
            if (currentBiomes >= planet.NumberOfBiomes)
            {
                var userReaction = message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                var botReaction = message.RemoveReactionAsync(reaction.Emote, message.Author);
                Task.WaitAll(userReaction, botReaction);
                return;
            }

            var embedBuilder = oldEmbed.ToEmbedBuilder();

            EmbedFieldBuilder fieldBuilder = new EmbedFieldBuilder().WithName("Biome").WithValue(planet.Biomes[currentBiomes].Description).WithIsInline(true);
            embedBuilder.AddField(fieldBuilder);
            embedBuilder.Fields = embedBuilder.Fields.OrderBy(field => PlanetFieldOrder(field.Name)).ToList();

            message.ModifyAsync(msg =>
            {
                msg.Content = string.Empty;
                msg.Embed = embedBuilder.Build();
            });
            currentBiomes++;
            message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            if (currentBiomes >= planet.NumberOfBiomes) message.RemoveReactionAsync(reaction.Emote, message.Author);
        }

        private void CloserLook(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var oldEmbed = message.Embeds.FirstOrDefault();

            int currentLooks = oldEmbed.Fields.Count(field => field.Name == "Closer Look");
            if (currentLooks >= 3) //TODO move this to a property of the planet?
            {
                var userReaction = message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                var botReaction = message.RemoveReactionAsync(reaction.Emote, message.Author);
                Task.WaitAll(userReaction, botReaction);
                return;
            }

            var planet = Planet.GeneratePlanet(oldEmbed.Title);

            var embedBuilder = oldEmbed.ToEmbedBuilder();

            EmbedFieldBuilder fieldBuilder = new EmbedFieldBuilder().WithName("Closer Look").WithValue(planet.CloserLooks[currentLooks].Description).WithIsInline(true);
            embedBuilder.AddField(fieldBuilder);
            embedBuilder.Fields = embedBuilder.Fields.OrderBy(field => PlanetFieldOrder(field.Name)).ToList();

            message.ModifyAsync(msg =>
            {
                msg.Content = string.Empty;
                msg.Embed = embedBuilder.Build();
            });
            currentLooks++;
            message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            if (currentLooks >= 3) message.RemoveReactionAsync(reaction.Emote, message.Author);
        }

        private void Life(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var oldEmbed = message.Embeds.FirstOrDefault();

            if (oldEmbed.Fields.Any(field => field.Name == "Life"))
            {
                message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                return;
            }

            var embedBuilder = oldEmbed.ToEmbedBuilder();
            var planet = Planet.GeneratePlanet(oldEmbed.Title);

            EmbedFieldBuilder builder = new EmbedFieldBuilder().WithName("Life").WithValue(planet.Life.Description);
            embedBuilder.AddField(builder);
            embedBuilder.Fields = embedBuilder.Fields.OrderBy(field => PlanetFieldOrder(field.Name)).ToList();

            message.ModifyAsync(msg =>
            {
                msg.Content = string.Empty;
                msg.Embed = embedBuilder.Build();
            });
            message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            message.RemoveReactionAsync(reaction.Emote, message.Author);
        }

        private async Task PlanetReactionHandler(Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (!reaction.User.IsSpecified || reaction.User.Value.IsBot) return;

            var message = userMessage.GetOrDownloadAsync().Result;

            if (reaction.Emote.Name == "🔍") CloserLook(message, channel, reaction);
            if (reaction.Emote.Name == "\U0001F996") Life(message, channel, reaction);
            if (reaction.Emote.Name == "\uD83C\uDF0D") Biome(message, channel, reaction);

            if (!message.Embeds.First()?.Fields.FirstOrDefault().Name.Contains("Options") ?? true) return;

            string PlanetName = message.Embeds.First().Title.Replace("__", "");

            if (reaction.Emote.Name == oneEmoji) await MakePlanetPost(SpaceRegion.Terminus, PlanetName, message);
            if (reaction.Emote.Name == twoEmoji) await MakePlanetPost(SpaceRegion.Outlands, PlanetName, message);
            if (reaction.Emote.Name == threeEmoji) await MakePlanetPost(SpaceRegion.Expanse, PlanetName, message);

            return;
        }
    }
}