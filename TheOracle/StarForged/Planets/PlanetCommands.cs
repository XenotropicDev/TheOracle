﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.StarForged;

namespace TheOracle.StarForged.Planets
{
    public class PlanetCommands : ModuleBase<SocketCommandContext>
    {
        public Emoji lookEmoji = new Emoji("🔍");
        public Emoji lifeEmoji = new Emoji("\U0001F996");
        public Emoji biomeEmoji = new Emoji("\uD83C\uDF0D");

        public PlanetCommands(ServiceProvider services)
        {
            Client = services.GetRequiredService<DiscordSocketClient>();
            var hooks = services.GetRequiredService<HookedEvents>();
            if (!hooks.PlanetReactions)
            {
                hooks.PlanetReactions = true;
                var reactionService = services.GetRequiredService<ReactionService>();

                ReactionEvent reaction1 = new ReactionEventBuilder().WithEmote(GenericReactions.oneEmoji).WithEvent(PlanetReactionHandler).Build();
                ReactionEvent reaction2 = new ReactionEventBuilder().WithEmote(GenericReactions.twoEmoji).WithEvent(PlanetReactionHandler).Build();
                ReactionEvent reaction3 = new ReactionEventBuilder().WithEmote(GenericReactions.threeEmoji).WithEvent(PlanetReactionHandler).Build();

                ReactionEvent look = new ReactionEventBuilder().WithEmote(lookEmoji).WithEvent(PlanetReactionHandler).Build();
                ReactionEvent Life = new ReactionEventBuilder().WithEmote(lifeEmoji).WithEvent(PlanetReactionHandler).Build();
                ReactionEvent biome = new ReactionEventBuilder().WithEmote(biomeEmoji).WithEvent(PlanetReactionHandler).Build();

                reactionService.reactionList.Add(reaction1);
                reactionService.reactionList.Add(reaction2);
                reactionService.reactionList.Add(reaction3);
                reactionService.reactionList.Add(look);
                reactionService.reactionList.Add(Life);
                reactionService.reactionList.Add(biome);
            }
            Services = services;
        }

        public DiscordSocketClient Client { get; }
        public ServiceProvider Services { get; }

        [Command("GeneratePlanet", ignoreExtraArgs: true)]
        [Summary("Creates an interactive post for a new Starforged planet")]
        [Remarks("PlannetCommand Options: World Type, Planet Name, Space Region\n\nReactions:\n🔍 Adds a Closer Look\n\U0001F996 Reveals any life-forms\n\uD83C\uDF0D Adds a biome (vital worlds only)")]
        [Alias("Planet")]
        public async Task PlanetPost([Remainder] string PlanetCommand = "")
        {
            SpaceRegion spaceRegion = StarforgedUtilites.GetAnySpaceRegion(PlanetCommand);
            
            var planetTypes = PlanetTemplate.GetPlanetTemplates().Select(t => t.PlanetType.Replace(PlanetResources.World, string.Empty).Trim());
            string planetType = planetTypes.FirstOrDefault(p => PlanetCommand.Contains(p, StringComparison.OrdinalIgnoreCase)) ?? string.Empty;
            if (planetType?.Length > 0) PlanetCommand = PlanetCommand.Replace(planetType, string.Empty, StringComparison.OrdinalIgnoreCase).Replace(PlanetResources.World, string.Empty, StringComparison.OrdinalIgnoreCase);

            string PlanetName = PlanetCommand.Replace(spaceRegion.ToString(), string.Empty, StringComparison.OrdinalIgnoreCase).Trim();
            if (PlanetName == string.Empty) PlanetName = $"P-{DateTime.Now.Ticks.ToString().Substring(7)}";
            else PlanetName = Regex.Replace(PlanetName, @"  +", " ");

            if (spaceRegion == SpaceRegion.None)
            {
                var palceHolderEmbed = new EmbedBuilder()
                    .WithTitle(PlanetResources.PlanetHelperTitle)
                    .WithDescription(PlanetName)
                    .WithFields(new EmbedFieldBuilder()
                        .WithName(PlanetResources.HelperOptions)
                        .WithValue(PlanetResources.HelperText)
                        )
                    .WithFooter(planetType);

                var msg = await ReplyAsync(embed: palceHolderEmbed.Build());
                await msg.AddReactionAsync(GenericReactions.oneEmoji);
                await msg.AddReactionAsync(GenericReactions.twoEmoji);
                await msg.AddReactionAsync(GenericReactions.threeEmoji);
                return;
            }

            await MakePlanetPost(spaceRegion, PlanetName, Context.Channel.Id, PlanetType: planetType);
        }

        private async Task MakePlanetPost(SpaceRegion region, string PlanetName, ulong channelId, IUserMessage message = null, string PlanetType = "")
        {
            Planet planet = Planet.GeneratePlanet(PlanetName, region, Services, channelId, PlanetType);

            if (message != null)
            {
                await message.RemoveAllReactionsAsync();
                await message.ModifyAsync(msg => msg.Embed = planet.GetEmbedBuilder().Build());
            }
            else
            {
                message = await ReplyAsync(embed: planet.GetEmbedBuilder().Build());
            }

            _ = Task.Run(async () =>
            {
                await message.AddReactionAsync(lookEmoji);
                await message.AddReactionAsync(lifeEmoji);

                if (planet.NumberOfBiomes > 1)
                {
                    await message.AddReactionAsync(biomeEmoji);
                }
            }).ConfigureAwait(false);
        }

        private async Task Biome(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var oldEmbed = message.Embeds.FirstOrDefault();
            var planet = Planet.GeneratePlanetFromEmbed(oldEmbed, Services, channel.Id);

            if (planet.RevealedBiomes >= planet.NumberOfBiomes)
            {
                await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
                return;
            }

            planet.RevealBiome();

            await message.ModifyAsync(msg =>
            {
                msg.Content = string.Empty;
                msg.Embed = planet.GetEmbedBuilder().Build();
            }).ConfigureAwait(false);

            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            if (planet.RevealedBiomes >= planet.NumberOfBiomes) await message.RemoveReactionAsync(reaction.Emote, message.Author).ConfigureAwait(false);
        }

        private async Task CloserLook(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var oldEmbed = message.Embeds.FirstOrDefault();
            var planet = Planet.GeneratePlanetFromEmbed(oldEmbed, Services, channel.Id);

            if (planet.RevealedLooks >= 3)
            {
                await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
                return;
            }

            planet.RevealCloserLook();

            await message.ModifyAsync(msg =>
            {
                msg.Content = string.Empty;
                msg.Embed = planet.GetEmbedBuilder().Build();
            }).ConfigureAwait(false);

            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            if (planet.RevealedLooks >= 3) await message.RemoveReactionAsync(reaction.Emote, message.Author).ConfigureAwait(false);
        }

        private async Task Life(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var oldEmbed = message.Embeds.FirstOrDefault();
            var planet = Planet.GeneratePlanetFromEmbed(oldEmbed, Services, channel.Id);

            planet.RevealLife();

            await Task.Run(async () =>
            {
                await message.ModifyAsync(msg =>
                {
                    msg.Content = string.Empty;
                    msg.Embed = planet.GetEmbedBuilder().Build();
                });
                await message.RemoveReactionAsync(reaction.Emote, user);
                await message.RemoveReactionAsync(reaction.Emote, message.Author);
            }).ConfigureAwait(false);
        }

        private async Task PlanetReactionHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsPlanetMessage(message)) return;
            if (reaction.Emote.IsSameAs(lookEmoji)) await CloserLook(message, channel, reaction, user).ConfigureAwait(false);
            if (reaction.Emote.IsSameAs(lifeEmoji)) await Life(message, channel, reaction, user).ConfigureAwait(false);
            if (reaction.Emote.IsSameAs(biomeEmoji)) await Biome(message, channel, reaction, user).ConfigureAwait(false);

            if (message.Embeds.FirstOrDefault()?.Title.Contains(PlanetResources.PlanetHelperTitle) ?? false)
            {
                string PlanetName = message.Embeds.First().Description;
                string PlanetType = message.Embeds.First().Footer.Value.Text ?? string.Empty;

                if (reaction.Emote.IsSameAs(GenericReactions.oneEmoji)) await MakePlanetPost(SpaceRegion.Terminus, PlanetName, channel.Id, message: message, PlanetType: PlanetType).ConfigureAwait(false);
                if (reaction.Emote.IsSameAs(GenericReactions.twoEmoji)) await MakePlanetPost(SpaceRegion.Outlands, PlanetName, channel.Id, message: message, PlanetType: PlanetType).ConfigureAwait(false);
                if (reaction.Emote.IsSameAs(GenericReactions.threeEmoji)) await MakePlanetPost(SpaceRegion.Expanse, PlanetName, channel.Id, message: message, PlanetType: PlanetType).ConfigureAwait(false);
            }

            return;
        }

        public bool IsPlanetMessage(IUserMessage message)
        {
            var embed = message.Embeds.FirstOrDefault();
            if (embed == null) return false;

            if (embed.Title.Contains(PlanetResources.PlanetHelperTitle, StringComparison.OrdinalIgnoreCase)) return true;

            return (embed.Description != null && embed.Description.Contains(String.Format(PlanetResources.PlanetPostDescription, string.Empty).Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}