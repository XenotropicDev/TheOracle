using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.Core;

namespace TheOracle.IronSworn
{
    public class OracleCommands : ModuleBase<SocketCommandContext>
    {
        public OracleCommands(OracleService oracleService)
        {
            _oracleService = oracleService;
        }

        private readonly OracleService _oracleService;

        [Command("OracleTable")]
        [Summary("Rolls an Oracle")]
        [Alias("Table")]
        public async Task OracleRoll([Remainder]string TableName)
        {
            var oracleTable = _oracleService.OracleList.SingleOrDefault(table => table.Name.Equals(TableName, StringComparison.OrdinalIgnoreCase));

            if (oracleTable == default)
            {
                await ReplyAsync($"Unknown Oracle Table: {TableName}");
                return;
            }

            var oracleResult = oracleTable.Oracles.GetRandomRow();

            await ReplyAsync(oracleResult.Description);
        }

        [Command("OracleList", ignoreExtraArgs: false)]
        [Summary("Lists Availble Oracles")]
        [Alias("List")]
        public async Task OracleList()
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.Title = "Oracles";

            foreach (var oracle in _oracleService.OracleList)
            {
                string sample = string.Join(", ", oracle.Oracles.Take(3).Select(o => o.Description));
                builder.AddField(oracle.Name, $"sample: {sample}");
            }

            await ReplyAsync(string.Empty, false, builder.Build());
        }
    }
}