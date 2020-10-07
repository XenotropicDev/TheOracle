using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheOracle.Core;
using TheOracle.GameCore.Oracle;

namespace TheOracle.IronSworn
{
    public class OracleCommands : ModuleBase<SocketCommandContext>
    {
        //OracleService is loaded from DI
        public OracleCommands(OracleService oracleService)
        {
            _oracleService = oracleService;
        }

        private readonly OracleService _oracleService;

        [Command("OracleTable")]
        [Summary("Rolls an Oracle")]
        [Alias("Oracle", "Table")]
        public async Task OracleRollCommand([Remainder] string Fullcommand = "")
        {
            GameName game = GameName.None;
            string oracleTable = Fullcommand;
            foreach (var s in Enum.GetNames(typeof(GameName)).Where(g => !g.Equals("none", StringComparison.OrdinalIgnoreCase)))
            {
                if (Regex.IsMatch(Fullcommand, $"(^{s} | {s}( |$))", RegexOptions.IgnoreCase)  && Enum.TryParse(s, out game))
                {
                    oracleTable = Regex.Replace(Fullcommand, $"{s} ?", "", RegexOptions.IgnoreCase).Trim();
                    break;
                }
            }

            OracleRoller roller = new OracleRoller(_oracleService, game);

            await ReplyAsync(roller.RollTable(oracleTable));
        }

        [Command("OracleList", ignoreExtraArgs: false)]
        [Summary("Lists Availble Oracles")]
        [Alias("List")]
        public async Task OracleList()
        {
            string reply = string.Empty;
            foreach (var oracle in _oracleService.OracleList)
            {
                //string sample = string.Join(", ", oracle.Oracles.Take(1).Select(o => o.Description));
                reply += $"**{oracle.Name}**, ";
            }
            reply = reply.Remove(reply.LastIndexOf(", "));

            while (true)
            {
                if (reply.Length < 2000)
                {
                    await ReplyAsync(reply);
                    break;
                }

                int cutoff = reply.Substring(0, 2000).LastIndexOf('\n');
                await ReplyAsync(reply.Substring(0, cutoff));
                reply = reply.Substring(cutoff);
            }
        }
    }
}