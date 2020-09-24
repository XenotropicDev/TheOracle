using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOracle.Core;

namespace TheOracle.IronSworn
{
    public class OracleCommands : ModuleBase<SocketCommandContext>
    {
        public OracleCommands(OracleService oracleService)
        {
            _oracleService = oracleService;

            var oracleData = JsonConvert.DeserializeObject<List<OracleTable>>(File.ReadAllText("IronSworn\\oracles.json"));
            _oracleService.OracleList.AddRange(oracleData);
            
        }

        private readonly OracleService _oracleService;

        [Command("OracleTable", ignoreExtraArgs: false)]
        [Summary("Rolls an Oracle")]
        [Alias("Table")]
        public async Task OracleRoll(string TableName)
        {
            var oracleTable = _oracleService.OracleList.SingleOrDefault(table => table.Name.Equals(TableName, StringComparison.OrdinalIgnoreCase));
            var oracleResult = oracleTable?.Oracles.GetRandomRow();

            if (oracleTable == default) 
            {
                await ReplyAsync($"Unknown Oracle Table: {TableName}");
                return;
            }

            await ReplyAsync(oracleResult.Description);
        }
    }
}
