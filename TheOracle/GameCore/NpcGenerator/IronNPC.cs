using Discord;
using System.Collections.Generic;
using System.Linq;
using TheOracle.Core;
using TheOracle.IronSworn;

namespace TheOracle.GameCore.NpcGenerator
{
    public class IronNPC : NPCGenerator
    {
        public string[] Roles { get; set; }
        public string[] Goals { get; set; }
        public string[] Descriptors { get; set; }
        public string Name { get; set; }

        public IronNPC(OracleService oracles, string name = default, string[] roles = null, string[] goals = null, string[] descriptors = null)
        {
            Roles = roles ?? new string[] { oracles.RandomRow("NPC Role", GameName.Ironsworn).Description };
            Goals = goals ?? new string[] { oracles.RandomRow("Goals", GameName.Ironsworn).Description };
            Descriptors = descriptors ?? new string[] { oracles.RandomRow("NPC Descriptors", GameName.Ironsworn).Description };
            Name = (name.Length > 0) ? name : oracles.RandomRow("Ironlander Names", GameName.Ironsworn).Description;
        }

        public override Embed GetEmbed()
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
                .WithTitle($"__{NPCResources.NPC}__")
                .WithFields(new EmbedFieldBuilder().WithName(NPCResources.Name).WithValue(Name).WithIsInline(false))
                .WithFields(rolesField)
                .WithFields(goalFields)
                .WithFields(descFields)
                .Build();
        }
    }
}