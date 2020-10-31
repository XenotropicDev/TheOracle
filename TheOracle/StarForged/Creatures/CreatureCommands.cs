using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.Core;

namespace TheOracle.StarForged.Creatures
{
    public class StarfrogedCreatureCommands : ModuleBase<SocketCommandContext>
    {
        public Emoji revealAspectEmoji = new Emoji("🦋");
        public Emoji oneEmoji = new Emoji("\u0031\u20E3");
        public Emoji twoEmoji = new Emoji("\u0032\u20E3");
        public Emoji threeEmoji = new Emoji("\u0033\u20E3");
        public Emoji fourEmoji = new Emoji("\u0034\u20E3");
        public Emoji fiveEmoji = new Emoji("\u0035\u20E3");
        public Emoji randomEmoji = new Emoji("🎲");

        public StarfrogedCreatureCommands(ServiceProvider services)
        {
            var hooks = services.GetRequiredService<HookedEvents>();

            if (!hooks.StarCreatureReactions)
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                var reactionService = services.GetRequiredService<ReactionService>();

                ReactionEvent reaction1 = new ReactionEventBuilder().WithEmote(oneEmoji).WithEvent(CreatureReactionHandler).Build();
                ReactionEvent reaction2 = new ReactionEventBuilder().WithEmote(twoEmoji).WithEvent(CreatureReactionHandler).Build();
                ReactionEvent reaction3 = new ReactionEventBuilder().WithEmote(threeEmoji).WithEvent(CreatureReactionHandler).Build();
                ReactionEvent reaction4 = new ReactionEventBuilder().WithEmote(fourEmoji).WithEvent(CreatureReactionHandler).Build();
                ReactionEvent reaction5 = new ReactionEventBuilder().WithEmote(fiveEmoji).WithEvent(CreatureReactionHandler).Build();
                ReactionEvent randomRoll = new ReactionEventBuilder().WithEmote(randomEmoji).WithEvent(CreatureReactionHandler).Build();

                ReactionEvent reveal = new ReactionEventBuilder().WithEmote(revealAspectEmoji).WithEvent(CreatureReactionHandler).Build();

                reactionService.reactionList.Add(reaction1);
                reactionService.reactionList.Add(reaction2);
                reactionService.reactionList.Add(reaction3);
                reactionService.reactionList.Add(reaction4);
                reactionService.reactionList.Add(reaction5);
                reactionService.reactionList.Add(randomRoll);
                reactionService.reactionList.Add(reveal);

                hooks.StarCreatureReactions = true;
            }

            Services = services;
        }

        private async Task CreatureReactionHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var CreatureHelperEmbed = message.Embeds.FirstOrDefault(embed => embed?.Title?.Contains(CreatureResources.CreatureHelper) ?? false);

            if (CreatureHelperEmbed != null)
            {
                CreatureEnvironment environment = StarforgedUtilites.CreatureEnvironmentFromEmote(reaction.Emote.Name);
                if (reaction.Emote.Name == randomEmoji.Name)
                {
                    string lookupValue = Services.GetRequiredService<OracleService>().RandomRow("Creature Environment").Description;
                    environment = StarforgedUtilites.GetAnyEnvironment(lookupValue);
                }
                if (environment == CreatureEnvironment.None) return;

                var newCreature = Creature.GenerateCreature(Services, channel.Id, environment);
                Task.WaitAll(message.RemoveAllReactionsAsync());

                await message.ModifyAsync(msg =>
                {
                    msg.Content = string.Empty;
                    msg.Embed = newCreature.GetEmbedBuilder().Build();
                }).ConfigureAwait(false);

                await message.AddReactionAsync(revealAspectEmoji).ConfigureAwait(false);
                return;
            }

            var creatureEmbed = message.Embeds.FirstOrDefault(embed => embed?.Title?.Contains(CreatureResources.CreatureTitle) ?? false);
            if (creatureEmbed == null) return;

            var creature = Creature.FromEmbed(creatureEmbed, Services, channel.Id);

            if (reaction.Emote.Name == revealAspectEmoji.Name) creature.RevealedAspectsToShow++;

            await message.ModifyAsync(msg => msg.Embed = creature.GetEmbedBuilder().Build()).ConfigureAwait(false);
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);

            return;
        }

        public OracleService oracleService { get; set; }
        public ServiceProvider Services { get; }

        [Command("GenerateCreature", ignoreExtraArgs: true)]
        [Summary("Creates a template post for a new Starforged Creature")]
        [Alias("Creature")]
        public async Task CreaturePost([Remainder] string CreatureCommand = "")
        {
            CreatureEnvironment environment = StarforgedUtilites.GetAnyEnvironment(CreatureCommand);

            if (environment == CreatureEnvironment.None)
            {
                EmbedBuilder builder = new EmbedBuilder()
                    .WithTitle(CreatureResources.CreatureHelper)
                    .WithDescription(CreatureResources.PickCreatureEnvironmentMessage);

                var msg = await ReplyAsync(embed: builder.Build());
                _ = Task.Run(async () =>
                {
                    await msg.AddReactionAsync(oneEmoji);
                    await msg.AddReactionAsync(twoEmoji);
                    await msg.AddReactionAsync(threeEmoji);
                    await msg.AddReactionAsync(fourEmoji);
                    await msg.AddReactionAsync(fiveEmoji);
                    await msg.AddReactionAsync(randomEmoji);
                }).ConfigureAwait(false);
                
                return;
            }

            string CreatureName = CreatureCommand.Replace(environment.ToString(), "").Trim();
            var creature = Creature.GenerateCreature(Services, Context.Channel.Id, environment);

            var message = await ReplyAsync("", false, creature.GetEmbedBuilder().Build());

            await message.AddReactionAsync(revealAspectEmoji);
        }
    }
}