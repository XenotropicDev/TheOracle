using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.Core;
using TheOracle.StarForged;

namespace TheOracle.IronSworn
{
    public class PlanetCommands : ModuleBase<SocketCommandContext>
    {
        [Command("GeneratePlanet", ignoreExtraArgs: true)]
        [Summary("Creates a template post for a new Starforged planet\n🔍 Adds a Closer Look\n\U0001F996 Reveals any life-forms")]
        [Alias("Planet")]
        public async Task PlanetPost(string PlanetName = "")
        {
            if (PlanetName == string.Empty) PlanetName = $"P-{DateTime.Now.Ticks}";

            Planet planet = Planet.GeneratePlanet(PlanetName);

            EmbedBuilder embedBuilder = new EmbedBuilder();

            embedBuilder.Title = $"__{PlanetName}__";

            embedBuilder.AddField(planet.PlanetType, planet.Description);
            embedBuilder.AddField("Atmosphere", planet.Atmosphere.Description);

            embedBuilder.AddField("Terminus Settlements", planet.Settlements.Terminus.Description, true);
            embedBuilder.AddField("Outlands Settlements", planet.Settlements.Outlands.Description, true);
            embedBuilder.AddField("Expanse Settlements", planet.Settlements.Expanse.Description, true);

            for (int i = 0; i < planet.SpaceObservations.Count; i++)
            {
                embedBuilder.AddField($"Space Observation {i + 1}:", planet.SpaceObservations[i].Description, true);
            }

            embedBuilder.ThumbnailUrl = planet.Thumbnail;
            var message = await ReplyAsync("", false, embedBuilder.Build());

            var lookingGlass = new Emoji("🔍"); //TODO make these user settable
            await message.AddReactionAsync(lookingGlass);

            var dino = new Emoji("\U0001F996");
            await message.AddReactionAsync(dino);
        }

        internal static void CloserLook(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction)
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

        private static int PlanetFieldOrder(string fieldName)
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

        internal static void Life(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction)
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
    }
}