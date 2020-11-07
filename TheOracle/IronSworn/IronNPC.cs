using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TheOracle.Core;
using TheOracle.IronSworn;

namespace TheOracle.GameCore.NpcGenerator
{
    public class IronNPC : INpcGenerator
    {
        public string[] Roles { get; set; }
        public string[] Goals { get; set; }
        public string[] Descriptors { get; set; }
        public string Name { get; set; }
        private IServiceProvider _serviceProvider { get; }

        public IronNPC(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Embed GetEmbed()
        {
            bool firstFieldInline = Roles.Count() + Descriptors.Count() + Goals.Count() <= 3;
            List<EmbedFieldBuilder> rolesField = new List<EmbedFieldBuilder>();
            for (int i = 0; i < Roles.Length; i++)
            {
                if (i == 0) rolesField.Add(new EmbedFieldBuilder().WithIsInline(firstFieldInline).WithName(NPCResources.Role).WithValue(Roles[i]));
                else rolesField.Add(new EmbedFieldBuilder().WithIsInline(true).WithName(NPCResources.Role).WithValue(Roles[i]));
            }

            List<EmbedFieldBuilder> goalFields = new List<EmbedFieldBuilder>();
            for (int i = 0; i < Goals.Length; i++)
            {
                if (i == 0) goalFields.Add(new EmbedFieldBuilder().WithIsInline(firstFieldInline).WithName(NPCResources.Goal).WithValue(Goals[i]));
                else goalFields.Add(new EmbedFieldBuilder().WithIsInline(true).WithName(NPCResources.Goal).WithValue(Goals[i]));
            }

            List<EmbedFieldBuilder> descFields = new List<EmbedFieldBuilder>();
            for (int i = 0; i < Descriptors.Length; i++)
            {
                if (i == 0) descFields.Add(new EmbedFieldBuilder().WithIsInline(firstFieldInline).WithName(NPCResources.Descriptor).WithValue(Descriptors[i]));
                else descFields.Add(new EmbedFieldBuilder().WithIsInline(true).WithName(NPCResources.Descriptor).WithValue(Descriptors[i]));
            }

            return new EmbedBuilder()
                .WithTitle($"__{NPCResources.NPCTitle}__")
                .WithFields(new EmbedFieldBuilder().WithName(NPCResources.Name).WithValue(Name).WithIsInline(false))
                .WithFields(rolesField)
                .WithFields(goalFields)
                .WithFields(descFields)
                .Build();
        }

        public void BuildNPCFromEmbed(Embed embed)
        {
            throw new System.NotImplementedException();
        }

        public INpcGenerator Build(string NPCCreationOptions)
        {
            //TODO fix the regex so it can support Role: role1 Role: role2 syntax
            Match RolesRegex = Regex.Match(NPCCreationOptions, $"{NPCResources.Role}[{NPCResources.ItemSeperators}]([\\w ,]*)");
            Match AnyGoals = Regex.Match(NPCCreationOptions, $"{NPCResources.Goal}[{NPCResources.ItemSeperators}]([\\w ,]*)");
            Match AnyDescriptors = Regex.Match(NPCCreationOptions, $"{NPCResources.Descriptor}[{NPCResources.ItemSeperators}]([\\w ,]*)");

            if (RolesRegex.Success)
            {
                Roles = RolesRegex.Groups[1].Value.Split(',');
                NPCCreationOptions = NPCCreationOptions.Replace(RolesRegex.Groups[0].Value, "");
            }

            if (AnyGoals.Success)
            {
                Goals = RolesRegex.Groups[1].Value.Split(',');
                NPCCreationOptions = NPCCreationOptions.Replace(RolesRegex.Groups[0].Value, "");
            }

            if (AnyDescriptors.Success)
            {
                Descriptors = RolesRegex.Groups[1].Value.Split(',');
                NPCCreationOptions = NPCCreationOptions.Replace(RolesRegex.Groups[0].Value, "");
            }

            var oracles = _serviceProvider.GetRequiredService<OracleService>();
            Roles ??= new string[] { oracles.RandomRow("NPC Role", GameName.Ironsworn).Description };
            Goals ??= new string[] { oracles.RandomRow("Goals", GameName.Ironsworn).Description };
            Descriptors ??= new string[] { oracles.RandomRow("NPC Descriptors", GameName.Ironsworn).Description };

            Name = (NPCCreationOptions.Length > 0) ? NPCCreationOptions : oracles.RandomRow("Ironlander Names", GameName.Ironsworn).Description;

            return this;
        }
    }
}