using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.Core;
using TheOracle.GameCore;
using TheOracle.GameCore.Oracle;
using TheOracle.GameCore.SettlementGenerator;

namespace TheOracle.StarForged.Settlements
{
    public class StarSettlement : ISettlement
    {
        public Emoji contactEmoji = new Emoji("☎️");

        public Emoji projectEmoji = new Emoji("\uD83D\uDEE0");

        public Emoji troubleEmoji = new Emoji("🔥");
        private bool isGenerated = false;

        public StarSettlement(IServiceProvider services, ulong channelId = 0)
        {
            Services = services;
            ChannelId = channelId;

            var hooks = services.GetRequiredService<HookedEvents>();
            if (!hooks.StarSettlementReactions)
            {
                var reactionService = services.GetRequiredService<ReactionService>();

                ReactionEvent reaction1 = new ReactionEventBuilder().WithEmote(GenericReactions.oneEmoji).WithEvent(SettlementReactionHandler).Build();
                ReactionEvent reaction2 = new ReactionEventBuilder().WithEmote(GenericReactions.twoEmoji).WithEvent(SettlementReactionHandler).Build();
                ReactionEvent reaction3 = new ReactionEventBuilder().WithEmote(GenericReactions.threeEmoji).WithEvent(SettlementReactionHandler).Build();

                ReactionEvent project = new ReactionEventBuilder().WithEmote(projectEmoji).WithEvent(ProjectReactionHandler).Build();
                ReactionEvent contact = new ReactionEventBuilder().WithEmote(contactEmoji).WithEvent(ContactReactionHandler).Build();
                ReactionEvent trouble = new ReactionEventBuilder().WithEmote(troubleEmoji).WithEvent(TroubleReactionHandler).Build();

                reactionService.reactionList.Add(reaction1);
                reactionService.reactionList.Add(reaction2);
                reactionService.reactionList.Add(reaction3);

                reactionService.reactionList.Add(project);
                reactionService.reactionList.Add(contact);
                reactionService.reactionList.Add(trouble);

                hooks.StarSettlementReactions = true;
            }
        }

        public string Authority { get; set; }

        public ulong ChannelId { get; }

        public List<string> FirstLooks { get; set; } = new List<string>();

        public int FirstLooksToReveal { get; set; }

        public string IconUrl { get; private set; }

        public string InitialContact { get; set; }

        public bool InitialContactRevealed { get; private set; }

        public string Location { get; set; }

        public string Name { get; set; }

        public string Population { get; set; }

        public List<string> Projects { get; set; } = new List<string>();

        public int ProjectsRevealed { get; private set; } = 0;

        public SpaceRegion Region { get; set; }

        public IServiceProvider Services { get; }

        public string SettlementTrouble { get; set; }

        public bool SettlementTroubleRevealed { get; private set; }

        public StarSettlement AddProject()
        {
            Projects.AddRandomOracleRow("Settlement Projects", GameName.Starforged, Services, ChannelId);
            ProjectsRevealed++;
            return this;
        }

        public async Task AfterMessageCreated(IUserMessage msg)
        {
            if (msg.Embeds.First().Title == SettlementResources.StarforgedHelper)
            {
                await msg.AddReactionAsync(GenericReactions.oneEmoji);
                await msg.AddReactionAsync(GenericReactions.twoEmoji);
                await msg.AddReactionAsync(GenericReactions.threeEmoji);
                return;
            }

            await msg.AddReactionAsync(projectEmoji);
            await msg.AddReactionAsync(contactEmoji);
            await msg.AddReactionAsync(troubleEmoji);

            return;
        }

        public ISettlement FromEmbed(IEmbed embed)
        {
            if (!embed.Description.Contains(SettlementResources.Settlement)) throw new ArgumentException(SettlementResources.SettlementNotFoundError);

            this.Authority = embed.Fields.FirstOrDefault(fld => fld.Name == SettlementResources.Authority).Value;
            this.FirstLooks = embed.Fields.Where(fld => fld.Name == SettlementResources.FirstLook)?.Select(item => item.Value).ToList() ?? new List<string>();
            this.FirstLooksToReveal = FirstLooks.Count();
            this.InitialContactRevealed = embed.Fields.Any(fld => fld.Name == SettlementResources.InitialContact);
            if (InitialContactRevealed) this.InitialContact = embed.Fields.FirstOrDefault(fld => fld.Name == SettlementResources.InitialContact).Value;
            this.Location = embed.Fields.FirstOrDefault(fld => fld.Name == SettlementResources.Location).Value;
            this.Name = embed.Title.Replace("__", "");
            this.Population = embed.Fields.FirstOrDefault(fld => fld.Name == SettlementResources.Population).Value;
            this.Projects = embed.Fields.Where(fld => fld.Name == SettlementResources.SettlementProjects)?.Select(item => item.Value).ToList() ?? new List<string>();
            this.ProjectsRevealed = embed.Fields.Count(fld => fld.Name.Contains(SettlementResources.SettlementProjects));
            this.Region = StarforgedUtilites.GetAnySpaceRegion(embed.Description);
            this.SettlementTroubleRevealed = embed.Fields.Any(fld => fld.Name == SettlementResources.SettlementTrouble);
            if (SettlementTroubleRevealed) this.SettlementTrouble = embed.Fields.FirstOrDefault(fld => fld.Name == SettlementResources.SettlementTrouble).Value;
            this.IconUrl = embed.Thumbnail?.Url;

            isGenerated = true;
            return this;
        }

        public ISettlement GenerateSettlement()
        {
            if (isGenerated) return this;

            var oracleService = Services.GetRequiredService<OracleService>();
            if (Name == string.Empty)
                Name = oracleService.RandomRow("Settlement Name", GameName.Starforged).Description;

            if (this.Region == SpaceRegion.None) throw new ArgumentException($"Unknown space region for settlement");

            int seed = $"{Name}{Region}".GetDeterministicHashCode();
            Random random = new Random(seed);

            this.Authority = oracleService.RandomRow("Settlement Authority", GameName.Starforged, random).Description;

            this.FirstLooksToReveal = random.Next(1, 4);
            for (int i = 0; i < this.FirstLooksToReveal; i++)
            {
                this.FirstLooks.AddRandomOracleRow("Settlement First Look", GameName.Starforged, Services, ChannelId, random);
            }

            if (string.IsNullOrEmpty(this.Location)) this.Location = oracleService.RandomRow("Settlement Location", GameName.Starforged, random).Description;

            this.Population = oracleService.RandomRow($"Settlement Population {this.Region}", GameName.Starforged, random).Description;

            isGenerated = true;
            return this;
        }

        public EmbedBuilder GetEmbedBuilder()
        {
            if (Region == SpaceRegion.None)
            {
                EmbedBuilder builder = new EmbedBuilder()
                    .WithTitle(SettlementResources.StarforgedHelper)
                    .WithDescription(SettlementResources.PickSpaceRegionMessage);

                if (Name.Length > 0) builder.WithFields(new EmbedFieldBuilder().WithName(SettlementResources.SettlementName).WithValue(Name));

                return builder;
            }

            GenerateSettlement();

            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle($"__{Name}__")
                .WithThumbnailUrl(IconUrl)
                .WithDescription($"{Region} {SettlementResources.Settlement}")
                .AddField(SettlementResources.Location, Location, true)
                .AddField(SettlementResources.Population, Population, true)
                .AddField(SettlementResources.Authority, Authority, true);

            for (int i = 0; i < FirstLooksToReveal; i++) embedBuilder.AddField(SettlementResources.FirstLook, FirstLooks[i], true);

            if (InitialContactRevealed) embedBuilder.AddField(SettlementResources.InitialContact, $"{InitialContact}", true);
            if (SettlementTroubleRevealed) embedBuilder.AddField(SettlementResources.SettlementTrouble, $"{SettlementTrouble}", true);

            for (int i = 0; i < ProjectsRevealed; i++) embedBuilder.AddField(SettlementResources.SettlementProjects, Projects[i], true);

            return embedBuilder;
        }

        public StarSettlement RevealInitialContact()
        {
            var oracleService = Services.GetRequiredService<OracleService>();
            InitialContact = oracleService.RandomOracleResult($"Settlement Initial Contact", Services, GameName.Starforged);
            InitialContactRevealed = true;
            return this;
        }

        public StarSettlement RevealTrouble()
        {
            var oracleService = Services.GetRequiredService<OracleService>();
            SettlementTrouble = oracleService.RandomOracleResult($"Settlement Trouble", Services, GameName.Starforged);
            SettlementTroubleRevealed = true;
            return this;
        }

        public ISettlement SetupFromUserOptions(string options)
        {
            Region = StarforgedUtilites.GetAnySpaceRegion(options);
            Location = StarforgedUtilites.GetAnyPlanetLocation(options) ?? string.Empty;

            var RegionString = (Region != SpaceRegion.None) ? Region.ToString() : string.Empty;

            string match = null;
            if (RegionString.Length + Location.Length > 0)
            {
                match = RegionString.Length > 0 && Location.Length > 0 ? $"({RegionString}|{Location})" : null;
                if (match?.Length == 0 && RegionString.Length > 0) match = RegionString;
                else if (match?.Length == 0 && Location.Length > 0) match = Location;
            }

            if (match != null) Name = Regex.Replace(Regex.Replace(options, match, string.Empty, RegexOptions.IgnoreCase), "  +", " ").Trim();
            else Name = options;
            return this;
        }

        public StarSettlement WithLocation(string location)
        {
            this.Location = location;
            return this;
        }

        public StarSettlement WithName(string name)
        {
            this.Name = name;
            return this;
        }

        public StarSettlement WithRegion(SpaceRegion region)
        {
            this.Region = region;
            return this;
        }

        private async Task ContactReactionHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsStarSettlement(message)) return;

            var settlmentEmbed = message.Embeds.FirstOrDefault(embed => embed?.Description?.Contains(SettlementResources.Settlement) ?? false);
            if (settlmentEmbed == null) return;

            var settlement = new StarSettlement(Services, channel.Id).FromEmbed(settlmentEmbed) as StarSettlement;

            settlement.RevealInitialContact();

            await message.ModifyAsync(msg => msg.Embed = settlement.GetEmbedBuilder().Build()).ConfigureAwait(false);
            await message.RemoveReactionAsync(reaction.Emote, message.Author).ConfigureAwait(false);
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
        }

        private async Task ProjectReactionHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsStarSettlement(message)) return;

            var settlmentEmbed = message.Embeds.FirstOrDefault(embed => embed?.Description?.Contains(SettlementResources.Settlement) ?? false);
            if (settlmentEmbed == null) return;

            var settlement = new StarSettlement(Services, channel.Id).FromEmbed(settlmentEmbed) as StarSettlement;
            settlement.AddProject();

            await message.ModifyAsync(msg => msg.Embed = settlement.GetEmbedBuilder().Build()).ConfigureAwait(false);
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);

            return;
        }

        private async Task SettlementReactionHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsStarSettlement(message)) return;

            var settlementHelperEmbed = message.Embeds.FirstOrDefault(embed => embed?.Title?.Contains(SettlementResources.StarforgedHelper) ?? false);
            if (settlementHelperEmbed == null) return;

            var region = StarforgedUtilites.SpaceRegionFromEmote(reaction.Emote.Name);
            if (region == SpaceRegion.None) return;

            string command = settlementHelperEmbed.Fields.FirstOrDefault(fld => fld.Name == SettlementResources.SettlementName).Value ?? string.Empty;
            string location = StarforgedUtilites.ExtractAnySettlementLocation(ref command);

            var newSettlement = new StarSettlement(Services, channel.Id)
                .WithName(command)
                .WithLocation(location)
                .WithRegion(region)
                .GenerateSettlement();
            Task.WaitAll(message.RemoveAllReactionsAsync());

            await message.ModifyAsync(msg =>
            {
                msg.Content = string.Empty;
                msg.Embed = newSettlement.GetEmbedBuilder().Build();
            }).ConfigureAwait(false);

            await Task.Run(async () =>
            {
                await message.AddReactionAsync(projectEmoji);
                await message.AddReactionAsync(contactEmoji);
                await message.AddReactionAsync(troubleEmoji);
            }).ConfigureAwait(false);

            return;
        }

        private bool IsStarSettlement(IUserMessage message)
        {
            var embed = message?.Embeds?.FirstOrDefault();
            if (embed == default) return false;

            if (embed.Title.Contains(SettlementResources.StarforgedHelper)) return true;

            if (embed.Footer.HasValue && embed.Footer.Value.Text.Contains(GameName.Ironsworn.ToString())) return false;
            if (!embed.Description?.Contains(SettlementResources.Settlement) ?? false == false) return false;

            if (embed.Fields.Any(fld => fld.Name == SettlementResources.Authority)) return true;

            return false;
        }

        private async Task TroubleReactionHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsStarSettlement(message)) return;

            var settlmentEmbed = message.Embeds.FirstOrDefault(embed => embed?.Description?.Contains(SettlementResources.Settlement) ?? false);
            if (settlmentEmbed == null) return;

            var settlement = new StarSettlement(Services, channel.Id).FromEmbed(settlmentEmbed) as StarSettlement;

            settlement.RevealTrouble();

            await message.ModifyAsync(msg => msg.Embed = settlement.GetEmbedBuilder().Build()).ConfigureAwait(false);
            await message.RemoveReactionAsync(reaction.Emote, message.Author).ConfigureAwait(false);
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);

            return;
        }
    }
}