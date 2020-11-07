using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TheOracle.Core;
using TheOracle.GameCore.NpcGenerator;
using TheOracle.IronSworn;

namespace TheOracle.StarForged.NPC
{
    public class StarforgedNPC : INpcGenerator
    {
        public StarforgedNPC(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string[] Aspects { get; set; }
        public string[] Dispositions { get; set; }
        public string[] FirstLooks { get; set; }
        public string[] Goals { get; set; }
        public string Name { get; set; }
        public string[] Roles { get; set; }
        private IServiceProvider _serviceProvider { get; }
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
                Aspects = AspectsRegex.Groups[1].Value.Split(',');
                NPCCreationOptions = NPCCreationOptions.Replace(AspectsRegex.Groups[0].Value, "");
            }

            if (DispositionRegex.Success)
            {
                Dispositions = DispositionRegex.Groups[1].Value.Split(',');
                NPCCreationOptions = NPCCreationOptions.Replace(DispositionRegex.Groups[0].Value, "");
            }

            if (FirstLookRegex.Success)
            {
                FirstLooks = FirstLookRegex.Groups[1].Value.Split(',');
                NPCCreationOptions = NPCCreationOptions.Replace(FirstLookRegex.Groups[0].Value, "");
            }

            if (GoalsRegex.Success)
            {
                Goals = GoalsRegex.Groups[1].Value.Split(',');
                NPCCreationOptions = NPCCreationOptions.Replace(GoalsRegex.Groups[0].Value, "");
            }

            if (RolesRegex.Success)
            {
                Roles = RolesRegex.Groups[1].Value.Split(',');
                NPCCreationOptions = NPCCreationOptions.Replace(RolesRegex.Groups[0].Value, "");
            }

            var oracles = _serviceProvider.GetRequiredService<OracleService>();
            Aspects ??= new string[] { oracles.RandomRow("Character Aspects", GameName.Starforged).Description };
            Dispositions ??= new string[] { oracles.RandomRow("Disposition", GameName.Starforged).Description };
            FirstLooks ??= new string[] { oracles.RandomRow("Character First Look", GameName.Starforged).Description };
            Goals ??= new string[] { oracles.RandomRow("Character Goal", GameName.Starforged).Description };
            Roles ??= new string[] { oracles.RandomRow("Character Role", GameName.Starforged).Description };

            Name = (NPCCreationOptions.Length > 0) ? NPCCreationOptions : oracles.RandomRow("Ironlander Names", GameName.Ironsworn).Description; //TODO make this starforged name when available

            return this;
        }

        public void BuildNPCFromEmbed(Embed embed)
        {
            throw new System.NotImplementedException();
        }

        public Embed GetEmbed()
        {
            List<EmbedFieldBuilder> AspectFields = new List<EmbedFieldBuilder>();
            for (int i = 0; i < Aspects.Length; i++) AspectFields.Add(new EmbedFieldBuilder().WithIsInline(true).WithName(StarforgedNPCResources.Aspect).WithValue(Aspects[i]));

            List<EmbedFieldBuilder> DispositionsFields = new List<EmbedFieldBuilder>();
            for (int i = 0; i < Dispositions.Length; i++) DispositionsFields.Add(new EmbedFieldBuilder().WithIsInline(true).WithName(StarforgedNPCResources.Disposition).WithValue(Dispositions[i]));

            List<EmbedFieldBuilder> FirstLookFields = new List<EmbedFieldBuilder>();
            for (int i = 0; i < Dispositions.Length; i++) FirstLookFields.Add(new EmbedFieldBuilder().WithIsInline(true).WithName(StarforgedNPCResources.FirstLook).WithValue(FirstLooks[i]));

            List<EmbedFieldBuilder> rolesField = new List<EmbedFieldBuilder>();
            for (int i = 0; i < Roles.Length; i++) rolesField.Add(new EmbedFieldBuilder().WithIsInline(true).WithName(NPCResources.Role).WithValue(Roles[i]));

            List<EmbedFieldBuilder> goalFields = new List<EmbedFieldBuilder>();
            for (int i = 0; i < Goals.Length; i++) goalFields.Add(new EmbedFieldBuilder().WithIsInline(true).WithName(NPCResources.Goal).WithValue(Goals[i]));

            return new EmbedBuilder()
                .WithTitle($"__{NPCResources.NPCTitle}__")                     
                .WithFields(new EmbedFieldBuilder().WithName(NPCResources.Name).WithValue(Name).WithIsInline(false))
                .WithFields(FirstLookFields)
                .WithFields(DispositionsFields)
                .WithFields(rolesField)
                .WithFields(goalFields)
                .WithFields(AspectFields)
                .Build();
        }
    }
}
