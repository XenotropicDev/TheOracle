﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.Core;
using TheOracle.GameCore.Oracle;

namespace TheOracle.StarForged.Creatures
{
    public class StarforgedCreatureCommands : ModuleBase<SocketCommandContext>
    {
        public Emoji revealAspectEmoji = new Emoji("🦋");
        public Emoji randomEmoji = new Emoji("🎲");

        public StarforgedCreatureCommands(ServiceProvider services)
        {
            var hooks = services.GetRequiredService<HookedEvents>();

            if (!hooks.StarCreatureReactions)
            {
                var reactionService = services.GetRequiredService<ReactionService>();

                ReactionEvent reaction1 = new ReactionEventBuilder().WithEmote(GenericReactions.oneEmoji).WithEvent(CreatureReactionHandler).Build();
                ReactionEvent reaction2 = new ReactionEventBuilder().WithEmote(GenericReactions.twoEmoji).WithEvent(CreatureReactionHandler).Build();
                ReactionEvent reaction3 = new ReactionEventBuilder().WithEmote(GenericReactions.threeEmoji).WithEvent(CreatureReactionHandler).Build();
                ReactionEvent reaction4 = new ReactionEventBuilder().WithEmote(GenericReactions.fourEmoji).WithEvent(CreatureReactionHandler).Build();
                ReactionEvent reaction5 = new ReactionEventBuilder().WithEmote(GenericReactions.fiveEmoji).WithEvent(CreatureReactionHandler).Build();
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
                if (reaction.Emote.IsSameAs(randomEmoji))
                {
                    string lookupValue = Services.GetRequiredService<OracleService>().RandomRow("Creature Environment").Description;
                    environment = StarforgedUtilites.GetAnyEnvironment(lookupValue);
                }
                if (environment == CreatureEnvironment.None) return;

                var newCreature = Creature.GenerateNewCreature(Services, channel.Id, environment);
                Task.WaitAll(message.RemoveAllReactionsAsync());

                await message.ModifyAsync(msg =>
                {
                    msg.Content = string.Empty;
                    msg.Embed = newCreature.GetEmbedBuilder().Build();
                }).ConfigureAwait(false);

                await Task.Run(async () =>
                {
                    if (message.Reactions.Count > 0)
                    {
                        await Task.Delay(1500); //wait just in case we are still adding more reactions. Impatient users deserve to wait!!!
                        await message.RemoveAllReactionsAsync();
                    }

                    await message.AddReactionAsync(revealAspectEmoji).ConfigureAwait(false);
                }).ConfigureAwait(false);
                return;
            }

            var creatureEmbed = message.Embeds.FirstOrDefault(embed => embed?.Title?.Contains(CreatureResources.CreatureTitle) ?? false);
            if (creatureEmbed == null) return;

            var creature = Creature.FromEmbed(creatureEmbed, Services, channel.Id);

            if (reaction.Emote.IsSameAs(revealAspectEmoji)) creature.AddRandomAspect();

            await message.ModifyAsync(msg => msg.Embed = creature.GetEmbedBuilder().Build()).ConfigureAwait(false);
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);

            return;
        }

        public OracleService oracleService { get; set; }
        public ServiceProvider Services { get; }

        [Command("GenerateCreature", ignoreExtraArgs: true)]
        [Summary("Creates an interactive post for a new Starforged creature")]
        [Remarks("🦋 - Adds a creature revealed aspect")]
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
                    await msg.AddReactionAsync(GenericReactions.oneEmoji);
                    await msg.AddReactionAsync(GenericReactions.twoEmoji);
                    await msg.AddReactionAsync(GenericReactions.threeEmoji);
                    await msg.AddReactionAsync(GenericReactions.fourEmoji);
                    await msg.AddReactionAsync(GenericReactions.fiveEmoji);
                    await msg.AddReactionAsync(randomEmoji);
                }).ConfigureAwait(false);

                return;
            }

            var creature = Creature.GenerateNewCreature(Services, Context.Channel.Id, environment);

            var message = await ReplyAsync("", false, creature.GetEmbedBuilder().Build());

            await message.AddReactionAsync(revealAspectEmoji);
        }
    }
}