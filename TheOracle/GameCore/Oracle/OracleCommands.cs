using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TheOracle.Core;
using TheOracle.GameCore.Oracle;

namespace TheOracle.IronSworn
{
    public class OracleCommands : ModuleBase<SocketCommandContext>
    {
        //OracleService is loaded from DI
        public OracleCommands(ServiceProvider services)
        {
            _oracleService = services.GetRequiredService<OracleService>();
            _client = services.GetRequiredService<DiscordSocketClient>();

            if (!services.GetRequiredService<HookedEvents>().OracleReactions)
            {
                _client.ReactionAdded += PairedTableReactionHandler;
            }
        }

        private readonly OracleService _oracleService;
        private readonly DiscordSocketClient _client;

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

            try
            {
                var msg = await ReplyAsync("", false, roller.BuildRollResults(oracleTable).GetEmbedBuilder().Build());
                if (roller.RollResultList.Count == 1 && roller.RollResultList[0].ParentTable.Pair?.Length > 0)
                {
                    await msg.AddReactionAsync(new Emoji("\uD83E\uDDE6"));
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"{Context.User} triggered an ArgumentException: {ex.Message}");
                await ReplyAsync(ex.Message);
            }
        }

        [Command("OracleList", ignoreExtraArgs: false)]
        [Summary("Lists Available Oracles")]
        [Alias("List")]
        public async Task OracleList()
        {
            string reply = $"__Here's a list of available Oracle Tables:__\n";
            foreach (var oracle in _oracleService.OracleList)
            {
                //string sample = string.Join(", ", oracle.Oracles.Take(1).Select(o => o.Description));
                string aliases = string.Empty;
                if (oracle.Aliases != null)
                {
                    aliases = $"{string.Join(", ", oracle.Aliases)}, ";
                }
                reply += $"**{oracle.Name}**, {aliases}";
            }
            reply = reply.Remove(reply.LastIndexOf(", "));

            while (true)
            {
                if (reply.Length < DiscordConfig.MaxMessageSize)
                {
                    await ReplyAsync(reply);
                    break;
                }

                int cutoff = reply.Substring(0, DiscordConfig.MaxMessageSize).LastIndexOf(',');
                await ReplyAsync(reply.Substring(0, cutoff));
                reply = reply.Substring(cutoff+1).Trim();
            }
        }

        private Task PairedTableReactionHandler(Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (!reaction.User.IsSpecified || reaction.User.Value.IsBot) return Task.CompletedTask;

            var message = userMessage.GetOrDownloadAsync().Result;
            if (message.Author.Id != _client.CurrentUser.Id) return Task.CompletedTask;

            var pairEmoji = new Emoji("\uD83E\uDDE6");
            if (reaction.Emote.Name == pairEmoji.Name)
            {
                message.RemoveReactionAsync(pairEmoji, reaction.User.Value);
                message.RemoveReactionAsync(pairEmoji, message.Author);

                message.ModifyAsync(msg => msg.Embed = AddRollToExisting(message));
            }

            return Task.CompletedTask;
        }

        private Embed AddRollToExisting(IUserMessage message)
        {
            var embed = message.Embeds.First().ToEmbedBuilder();
            if (!embed.Title.Contains(OracleResources.OracleResult)) throw new ArgumentException("Unknown message type");

            OracleRoller existingRoller = OracleRoller.RebuildRoller(_oracleService, embed);
            var rollerCopy = new List<OracleRoller.RollResult>(existingRoller.RollResultList); //Copy the list so we can safely add to it using foreach

            foreach (var rollResult in rollerCopy.Where(tbl => tbl.ParentTable.Pair?.Length > 0))
            {
                var pairedTable = _oracleService.OracleList.Find(tbl => tbl.Name == rollResult.ParentTable.Name);
                if (existingRoller.RollResultList.Any(tbl => tbl.ParentTable.Name == pairedTable.Pair)) continue;

                var roller = new OracleRoller(_oracleService).WithGame(existingRoller.Game).BuildRollResults(pairedTable.Pair);

                roller.RollResultList.ForEach(res => res.ShouldInline = true);
                rollResult.ShouldInline = true;

                int index = existingRoller.RollResultList.IndexOf(rollResult) + 1;
                existingRoller.RollResultList.InsertRange(index, roller.RollResultList);
            }

            return existingRoller.GetEmbedBuilder().Build();
        }
    }
}