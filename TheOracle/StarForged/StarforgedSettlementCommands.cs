using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.Core;
using TheOracle.IronSworn;

namespace TheOracle.StarForged
{
    public class StarforgedSettlementCommands : ModuleBase<SocketCommandContext>
    {
        public OracleService oracleService { get; set; }

        [Command("GenerateSettlement", ignoreExtraArgs: true)]
        [Summary("Creates a template post for a new Starforged settlement")]
        [Alias("Settlement")]
        public async Task SettlementPost(SpaceRegion region, [Remainder] string SettlementName = "")
        {
            var settlement = Settlement.GenerateSettlement(oracleService, region, SettlementName);

            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle($"__{settlement.Name}__")
                .AddField("Location", settlement.Location)
                .AddField("Population", settlement.Population, true)
                .AddField("Authority", settlement.Authority, true)

                .AddField("First Look", settlement.FirstLooks[0])
                .AddField("Initial Contact", settlement.InitialContact)
                .AddField("Settlement Projects", settlement.Projects[0])
                .AddField("Settlement Trouble", settlement.SettlementTrouble);

            //embedBuilder.ThumbnailUrl = planet.Thumbnail; //TODO (maybe location hex?)
            var message = await ReplyAsync("", false, embedBuilder.Build());
        }
    }

    public class Settlement
    {
        public string Authority { get; set; }
        public List<string> FirstLooks { get; set; }
        public string InitialContact { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public string Population { get; set; }
        public List<string> Projects { get; set; }
        public int Seed { get; set; }
        public string SettlementTrouble { get; set; }
        public SpaceRegion Region { get; set; }

        public static Settlement GenerateSettlement(OracleService oracleService, SpaceRegion spaceRegion, string SettlementName = "")
        {
            if (SettlementName == string.Empty)
                SettlementName = oracleService.RandomRow("Settlement Name").Description;

            var s = new Settlement();
            s.Seed = $"{SettlementName}{spaceRegion}".GetDeterministicHashCode();
            s.Region = spaceRegion;
            s.Name = SettlementName;

            Random random = new Random(s.Seed);

            s.Authority = oracleService.RandomRow("Settlement Authority", GameName.Starforged, random).Description;

            s.FirstLooks = oracleService.OracleList.Single(o => o.Name == "Settlement First Look" && o.Game == GameName.Starforged)
                .Oracles.Select(o => o.Description).ToList();
            s.FirstLooks.Shuffle(random);

            s.InitialContact = oracleService.RandomRow("Settlement Initial Contact", GameName.Starforged, random).Description;

            s.Location = oracleService.RandomRow("Settlement Location", GameName.Starforged, random).Description;

            s.Population = oracleService.RandomRow($"Settlement Population {s.Region}", GameName.Starforged, random).Description;

            s.Projects = oracleService.OracleList.Single(o => o.Name == "Settlement Projects" && o.Game == GameName.Starforged)
                .Oracles.Select(o => o.Description).ToList();
            s.Projects.Shuffle(random);

            s.SettlementTrouble = oracleService.RandomRow($"Settlement Trouble", GameName.Starforged, random).Description;

            return s;
        }
    }
}