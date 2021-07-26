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
using TheOracle.GameCore.NpcGenerator;
using TheOracle.GameCore.Oracle;

namespace TheOracle.StarForged.NPC
{
    public class StarforgedNPC : INpcGenerator
    {
        private Emoji aspectEmoji = new Emoji("🔍");
        private Emoji dispositionEmoji = new Emoji("👋");
        private Emoji firstLookEmoji = new Emoji("👀");
        private Emoji goalEmoji = new Emoji("❗");
        private Emoji roleEmoji = new Emoji("🎭");

        public StarforgedNPC(IServiceProvider services)
        {
            Services = services;
            ReactionsToAdd = new IEmote[] { aspectEmoji, dispositionEmoji, firstLookEmoji, goalEmoji, roleEmoji };

            var hooks = services.GetRequiredService<HookedEvents>();

            if (!hooks.StarNPCReactions)
            {
                var reactionService = services.GetRequiredService<ReactionService>();

                ReactionEvent aspectReaction = new ReactionEventBuilder().WithEmote(aspectEmoji).WithEvent(AspectHandler).Build();
                ReactionEvent dispositionReaction = new ReactionEventBuilder().WithEmote(dispositionEmoji).WithEvent(DispositionHandler).Build();
                ReactionEvent firstlookReaction = new ReactionEventBuilder().WithEmote(firstLookEmoji).WithEvent(FirstLookHandler).Build();
                ReactionEvent goalReaction = new ReactionEventBuilder().WithEmote(goalEmoji).WithEvent(GoalHandler).Build();
                ReactionEvent roleReaction = new ReactionEventBuilder().WithEmote(roleEmoji).WithEvent(RoleHandler).Build();

                reactionService.reactionList.Add(aspectReaction);
                reactionService.reactionList.Add(dispositionReaction);
                reactionService.reactionList.Add(firstlookReaction);
                reactionService.reactionList.Add(goalReaction);
                reactionService.reactionList.Add(roleReaction);

                hooks.StarNPCReactions = true;
            }
        }

        public List<string> Aspects { get; set; } = new List<string>();

        public List<string> Dispositions { get; set; } = new List<string>();

        public List<string> FirstLooks { get; set; } = new List<string>();

        public List<string> Goals { get; set; } = new List<string>();

        public string Name { get; set; }
        public string IconUrl { get; private set; }
        public string EmbedDesc { get; private set; }
        public IEmote[] ReactionsToAdd { get; set; }

        public List<string> Roles { get; set; } = new List<string>();

        private IServiceProvider Services { get; }

        public INpcGenerator Build(string NPCCreationOptions)
        {
            //TODO fix the regex so it can support Role: role1 Role: role2 syntax
            Match AspectsRegex = Regex.Match(NPCCreationOptions, $"{StarforgedNPCResources.Aspect}[{NPCResources.ItemSeperators}]([\\w ,]*)");
            Match DispositionRegex = Regex.Match(NPCCreationOptions, $"{StarforgedNPCResources.Disposition}[{NPCResources.ItemSeperators}]([\\w ,]*)");
            Match FirstLookRegex = Regex.Match(NPCCreationOptions, $"{StarforgedNPCResources.FirstLook}[{NPCResources.ItemSeperators}]([\\w ,]*)");
            Match RolesRegex = Regex.Match(NPCCreationOptions, $"{NPCResources.Role}[{NPCResources.ItemSeperators}]([\\w ,]*)");
            Match GoalsRegex = Regex.Match(NPCCreationOptions, $"{NPCResources.Goal}[{NPCResources.ItemSeperators}]([\\w ,]*)");

            if (AspectsRegex.Success)
            {
                Aspects = AspectsRegex.Groups[1].Value.Split(',').ToList();
                NPCCreationOptions = NPCCreationOptions.Replace(AspectsRegex.Groups[0].Value, "");
            }

            if (DispositionRegex.Success)
            {
                Dispositions = DispositionRegex.Groups[1].Value.Split(',').ToList();
                NPCCreationOptions = NPCCreationOptions.Replace(DispositionRegex.Groups[0].Value, "");
            }

            if (FirstLookRegex.Success)
            {
                FirstLooks = FirstLookRegex.Groups[1].Value.Split(',').ToList();
                NPCCreationOptions = NPCCreationOptions.Replace(FirstLookRegex.Groups[0].Value, "");
            }

            if (GoalsRegex.Success)
            {
                Goals = GoalsRegex.Groups[1].Value.Split(',').ToList();
                NPCCreationOptions = NPCCreationOptions.Replace(GoalsRegex.Groups[0].Value, "");
            }

            if (RolesRegex.Success)
            {
                Roles = RolesRegex.Groups[1].Value.Split(',').ToList();
                NPCCreationOptions = NPCCreationOptions.Replace(RolesRegex.Groups[0].Value, "");
            }

            var oracles = Services.GetRequiredService<OracleService>();

            if (FirstLooks.Count == 0)
            {
                FirstLooks.Add(oracles.RandomOracleResult("Character First Look", Services, GameName.Starforged));
                FirstLooks.Add(oracles.RandomOracleResult("Character First Look", Services, GameName.Starforged));
            }

            Name = NPCCreationOptions.Trim();
            if (Name.Length == 0)
            {
                var givenName = oracles.RandomRow("Character Given Name", GameName.Starforged).GetOracleResult(Services, GameName.Starforged);
                var familyName = oracles.RandomRow("Character Family Name", GameName.Starforged).GetOracleResult(Services, GameName.Starforged);
                var callSign = oracles.RandomRow("Character Family Name", GameName.Starforged).GetOracleResult(Services, GameName.Starforged);

                var randomDouble = BotRandom.Instance.NextDouble();
                if (randomDouble > .98d) Name = string.Format(StarforgedNPCResources.NameFormat, givenName, familyName, String.Format(StarforgedNPCResources.CallSignFormat, callSign));
                else if (randomDouble > .95d) Name = string.Format(String.Format(StarforgedNPCResources.CallSignFormat, callSign));
                else if (randomDouble > .80d) Name = string.Format(StarforgedNPCResources.NameFormat, givenName, familyName, string.Empty);
                else Name = givenName;
            }

            return this;
        }

        public void BuildNPCFromEmbed(Embed embed)
        {
            foreach (var aspectField in embed.Fields.Where(fld => fld.Name == StarforgedNPCResources.Aspect)) Aspects.Add(aspectField.Value);
            foreach (var dispositionField in embed.Fields.Where(fld => fld.Name == StarforgedNPCResources.Disposition)) Dispositions.Add(dispositionField.Value);
            foreach (var firstLook in embed.Fields.Where(fld => fld.Name == StarforgedNPCResources.FirstLook)) FirstLooks.Add(firstLook.Value);
            foreach (var goal in embed.Fields.Where(fld => fld.Name == NPCResources.Goal)) Goals.Add(goal.Value);
            foreach (var role in embed.Fields.Where(fld => fld.Name == NPCResources.Role)) Roles.Add(role.Value);
            Name = embed.Fields.FirstOrDefault(fld => fld.Name == NPCResources.Name).Value;
            IconUrl = embed.Thumbnail.HasValue ? embed.Thumbnail.Value.Url : null;
            EmbedDesc = embed.Description;
        }

        public Embed GetEmbed()
        {
            List<EmbedFieldBuilder> AspectFields = new List<EmbedFieldBuilder>();
            foreach (var aspect in Aspects) AspectFields.Add(new EmbedFieldBuilder().WithIsInline(true).WithName(StarforgedNPCResources.Aspect).WithValue(aspect));

            List<EmbedFieldBuilder> DispositionsFields = new List<EmbedFieldBuilder>();
            foreach (var disposition in Dispositions) DispositionsFields.Add(new EmbedFieldBuilder().WithIsInline(true).WithName(StarforgedNPCResources.Disposition).WithValue(disposition));

            List<EmbedFieldBuilder> FirstLookFields = new List<EmbedFieldBuilder>();
            foreach (var firstLook in FirstLooks) FirstLookFields.Add(new EmbedFieldBuilder().WithIsInline(true).WithName(StarforgedNPCResources.FirstLook).WithValue(firstLook));

            List<EmbedFieldBuilder> rolesField = new List<EmbedFieldBuilder>();
            foreach (var role in Roles) rolesField.Add(new EmbedFieldBuilder().WithIsInline(true).WithName(NPCResources.Role).WithValue(role));

            List<EmbedFieldBuilder> goalFields = new List<EmbedFieldBuilder>();
            foreach (var goal in Goals) goalFields.Add(new EmbedFieldBuilder().WithIsInline(true).WithName(NPCResources.Goal).WithValue(goal));

            return new EmbedBuilder()
                .WithTitle($"__{NPCResources.NPCTitle}__")
                .WithThumbnailUrl(IconUrl)
                .WithFields(new EmbedFieldBuilder().WithName(NPCResources.Name).WithValue(Name).WithIsInline(false))
                .WithFields(FirstLookFields)
                .WithFields(DispositionsFields)
                .WithFields(rolesField)
                .WithFields(goalFields)
                .WithFields(AspectFields)
                .WithFooter(GameName.Starforged.ToString())
                .WithDescription(EmbedDesc)
                .Build();
        }

        public bool IsStarNPC(IUserMessage message)
        {
            var embed = message?.Embeds?.FirstOrDefault();
            if (embed == null) return false;

            return (embed.Title?.Contains(NPCResources.NPCTitle) == true && embed.Footer.HasValue && embed.Footer.Value.Text.Contains(GameName.Starforged.ToString(), StringComparison.OrdinalIgnoreCase));
        }

        private async Task AspectHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsStarNPC(message)) return;
            var npc = new StarforgedNPC(Services);
            npc.BuildNPCFromEmbed(message.Embeds.First() as Embed);

            var oracles = Services.GetRequiredService<OracleService>();
            npc.Aspects.Add(oracles.RandomOracleResult("Revealed Character Aspect", Services, GameName.Starforged));
            await message.ModifyAsync(msg => msg.Embed = npc.GetEmbed());
            await message.RemoveReactionAsync(reaction.Emote, user);
        }

        private async Task DispositionHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsStarNPC(message)) return;
            var npc = new StarforgedNPC(Services);
            npc.BuildNPCFromEmbed(message.Embeds.First() as Embed);

            var oracles = Services.GetRequiredService<OracleService>();
            npc.Dispositions.Add(oracles.RandomOracleResult("Disposition", Services, GameName.Starforged));
            await message.ModifyAsync(msg => msg.Embed = npc.GetEmbed());
            await message.RemoveReactionAsync(reaction.Emote, user);
        }

        private async Task FirstLookHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsStarNPC(message)) return;
            var npc = new StarforgedNPC(Services);
            npc.BuildNPCFromEmbed(message.Embeds.First() as Embed);

            var oracles = Services.GetRequiredService<OracleService>();
            npc.FirstLooks.Add(oracles.RandomOracleResult("Character First Look", Services, GameName.Starforged));
            await message.ModifyAsync(msg => msg.Embed = npc.GetEmbed());
            await message.RemoveReactionAsync(reaction.Emote, user);
        }

        private async Task GoalHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsStarNPC(message)) return;
            var npc = new StarforgedNPC(Services);
            npc.BuildNPCFromEmbed(message.Embeds.First() as Embed);

            var oracles = Services.GetRequiredService<OracleService>();
            npc.Goals.Add(oracles.RandomOracleResult("Character Goal", Services, GameName.Starforged));
            await message.ModifyAsync(msg => msg.Embed = npc.GetEmbed());
            await message.RemoveReactionAsync(reaction.Emote, user);
        }

        private async Task RoleHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsStarNPC(message)) return;
            var npc = new StarforgedNPC(Services);
            npc.BuildNPCFromEmbed(message.Embeds.First() as Embed);

            var oracles = Services.GetRequiredService<OracleService>();
            npc.Roles.Add(oracles.RandomOracleResult("Character Role", Services, GameName.Starforged));
            await message.ModifyAsync(msg => msg.Embed = npc.GetEmbed());
            await message.RemoveReactionAsync(reaction.Emote, user);
        }
    }
}