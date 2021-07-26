using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.GameCore.Oracle;

namespace TheOracle.GameCore.NpcGenerator
{
    public class IronNPC : INpcGenerator
    {
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> Goals { get; set; } = new List<string>();
        public List<string> Descriptors { get; set; } = new List<string>();
        public List<string> Dispositions { get; set; } = new List<string>();
        public List<string> Activities { get; set; } = new List<string>();
        public string Name { get; set; }
        private IServiceProvider Services { get; }
        public IEmote[] ReactionsToAdd { get; set; } = null;
        public string IconUrl { get; private set; }
        public string EmbedDesc { get; private set; }

        private Emoji descEmoji = new Emoji("🔍");
        private Emoji dispositionEmoji = new Emoji("👋");
        private Emoji activityEmoji = new Emoji("💃");
        private Emoji goalEmoji = new Emoji("❗");
        private Emoji roleEmoji = new Emoji("🎭");

        public IronNPC(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
            ReactionsToAdd = new IEmote[] { descEmoji, dispositionEmoji, activityEmoji, goalEmoji, roleEmoji };

            var hooks = serviceProvider.GetRequiredService<HookedEvents>();
            if (!hooks.IronNpcReactions)
            {
                var reactionService = serviceProvider.GetRequiredService<ReactionService>();

                ReactionEvent descReaction = new ReactionEventBuilder().WithEmote(descEmoji).WithEvent(DescHandler).Build();
                ReactionEvent dispositionReaction = new ReactionEventBuilder().WithEmote(dispositionEmoji).WithEvent(DispositionHandler).Build();
                ReactionEvent activityReaction = new ReactionEventBuilder().WithEmote(activityEmoji).WithEvent(ActivityHandler).Build();
                ReactionEvent goalReaction = new ReactionEventBuilder().WithEmote(goalEmoji).WithEvent(GoalHandler).Build();
                ReactionEvent roleReaction = new ReactionEventBuilder().WithEmote(roleEmoji).WithEvent(RoleHandler).Build();

                reactionService.reactionList.Add(descReaction);
                reactionService.reactionList.Add(dispositionReaction);
                reactionService.reactionList.Add(activityReaction);
                reactionService.reactionList.Add(goalReaction);
                reactionService.reactionList.Add(roleReaction);

                hooks.IronNpcReactions = true;
            }
        }

        public Embed GetEmbed()
        {
            return new EmbedBuilder()
                .WithTitle($"__{NPCResources.NPCTitle}__")
                .WithThumbnailUrl(IconUrl)
                .WithFields(new EmbedFieldBuilder().WithName(NPCResources.Name).WithValue(Name).WithIsInline(false))
                .WithFields(Roles.EmbedFieldBuilderFromList(NPCResources.Role, true))
                .WithFields(Goals.EmbedFieldBuilderFromList(NPCResources.Goal, true))
                .WithFields(Descriptors.EmbedFieldBuilderFromList(NPCResources.Descriptor, true))
                .WithFields(Activities.EmbedFieldBuilderFromList(NPCResources.Activity, true))
                .WithFields(Dispositions.EmbedFieldBuilderFromList(NPCResources.Disposition, true))
                .WithFooter(GameName.Ironsworn.ToString())
                .WithDescription(EmbedDesc)
                .Build();
        }

        public void BuildNPCFromEmbed(Embed embed)
        {
            foreach (var activityField in embed.Fields.Where(fld => fld.Name == NPCResources.Activity)) Activities.Add(activityField.Value);
            foreach (var dispositionField in embed.Fields.Where(fld => fld.Name == NPCResources.Disposition)) Dispositions.Add(dispositionField.Value);
            foreach (var goal in embed.Fields.Where(fld => fld.Name == NPCResources.Goal)) Goals.Add(goal.Value);
            foreach (var role in embed.Fields.Where(fld => fld.Name == NPCResources.Role)) Roles.Add(role.Value);
            foreach (var desc in embed.Fields.Where(fld => fld.Name == NPCResources.Descriptor)) Descriptors.Add(desc.Value);
            Name = embed.Fields.FirstOrDefault(fld => fld.Name == NPCResources.Name).Value;
            IconUrl = embed.Thumbnail.HasValue ? embed.Thumbnail.Value.Url : null;
            EmbedDesc = embed.Description;
        }

        public INpcGenerator Build(string NPCCreationOptions)
        {
            //TODO fix the regex so it can support Role: role1 Role: role2 syntax
            Match RolesRegex = Regex.Match(NPCCreationOptions, $"{NPCResources.Role}[{NPCResources.ItemSeperators}]([\\w ,]*)", RegexOptions.IgnoreCase);
            Match AnyGoals = Regex.Match(NPCCreationOptions, $"{NPCResources.Goal}[{NPCResources.ItemSeperators}]([\\w ,]*)", RegexOptions.IgnoreCase);
            Match AnyDescriptors = Regex.Match(NPCCreationOptions, $"{NPCResources.Descriptor}[{NPCResources.ItemSeperators}]([\\w ,]*)", RegexOptions.IgnoreCase);

            if (RolesRegex.Success)
            {
                Roles = RolesRegex.Groups[1].Value.Split(',').ToList();
                NPCCreationOptions = NPCCreationOptions.Replace(RolesRegex.Groups[0].Value, "");
            }

            if (AnyGoals.Success)
            {
                Goals = RolesRegex.Groups[1].Value.Split(',').ToList();
                NPCCreationOptions = NPCCreationOptions.Replace(RolesRegex.Groups[0].Value, "");
            }

            if (AnyDescriptors.Success)
            {
                Descriptors = RolesRegex.Groups[1].Value.Split(',').ToList();
                NPCCreationOptions = NPCCreationOptions.Replace(RolesRegex.Groups[0].Value, "");
            }

            var oracles = Services.GetRequiredService<OracleService>();
            if (Descriptors.Count == 0) Descriptors = new List<string> { oracles.RandomRow("NPC Descriptors", GameName.Ironsworn).Description };

            if (Regex.IsMatch(NPCCreationOptions, @"\bElf\b", RegexOptions.IgnoreCase)) Name = oracles.RandomRow("Elf Names", GameName.Ironsworn).Description;
            else if (Regex.IsMatch(NPCCreationOptions, @"\bGiant\b", RegexOptions.IgnoreCase)) Name = oracles.RandomRow("Giant Names", GameName.Ironsworn).Description;
            else if (Regex.IsMatch(NPCCreationOptions, @"\bVarou\b", RegexOptions.IgnoreCase)) Name = oracles.RandomRow("Varou Names", GameName.Ironsworn).Description;
            else if (NPCCreationOptions.Length > 0) Name = NPCCreationOptions;
            else Name = oracles.RandomRow("Ironlander Names", GameName.Ironsworn).Description;

            return this;
        }

        public bool IsIronNPC(IUserMessage message)
        {
            var embed = message?.Embeds?.FirstOrDefault();
            if (embed == null) return false;

            return (embed.Title?.Contains(NPCResources.NPCTitle) == true && embed.Footer.HasValue && embed.Footer.Value.Text.Contains(GameName.Ironsworn.ToString(), StringComparison.OrdinalIgnoreCase));
        }

        private async Task DescHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsIronNPC(message)) return;
            var npc = new IronNPC(Services);
            npc.BuildNPCFromEmbed(message.Embeds.First() as Embed);

            var oracles = Services.GetRequiredService<OracleService>();
            npc.Descriptors.Add(oracles.RandomOracleResult("Character Descriptor", Services, GameName.Ironsworn));
            await message.ModifyAsync(msg => msg.Embed = npc.GetEmbed());
            await message.RemoveReactionAsync(reaction.Emote, user);
        }

        private async Task DispositionHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsIronNPC(message)) return;
            var npc = new IronNPC(Services);
            npc.BuildNPCFromEmbed(message.Embeds.First() as Embed);

            var oracles = Services.GetRequiredService<OracleService>();
            npc.Dispositions.Add(oracles.RandomOracleResult("Character Disposition", Services, GameName.Ironsworn));
            await message.ModifyAsync(msg => msg.Embed = npc.GetEmbed());
            await message.RemoveReactionAsync(reaction.Emote, user);
        }

        private async Task ActivityHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsIronNPC(message)) return;
            var npc = new IronNPC(Services);
            npc.BuildNPCFromEmbed(message.Embeds.First() as Embed);

            var oracles = Services.GetRequiredService<OracleService>();
            npc.Activities.Add(oracles.RandomOracleResult("Character Activity", Services, GameName.Ironsworn));
            await message.ModifyAsync(msg => msg.Embed = npc.GetEmbed());
            await message.RemoveReactionAsync(reaction.Emote, user);
        }

        private async Task GoalHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsIronNPC(message)) return;
            var npc = new IronNPC(Services);
            npc.BuildNPCFromEmbed(message.Embeds.First() as Embed);

            var oracles = Services.GetRequiredService<OracleService>();
            npc.Goals.Add(oracles.RandomOracleResult("Character Goal", Services, GameName.Ironsworn));
            await message.ModifyAsync(msg => msg.Embed = npc.GetEmbed());
            await message.RemoveReactionAsync(reaction.Emote, user);
        }

        private async Task RoleHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsIronNPC(message)) return;
            var npc = new IronNPC(Services);
            npc.BuildNPCFromEmbed(message.Embeds.First() as Embed);

            var oracles = Services.GetRequiredService<OracleService>();
            npc.Roles.Add(oracles.RandomOracleResult("Character Role", Services, GameName.Ironsworn));
            await message.ModifyAsync(msg => msg.Embed = npc.GetEmbed());
            await message.RemoveReactionAsync(reaction.Emote, user);
        }
    }
}