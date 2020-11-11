using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.Core;
using TheOracle.GameCore.Oracle;

namespace TheOracle.IronSworn
{
    public class OracleCommands : ModuleBase<SocketCommandContext>
    {
        //OracleService is loaded from DI
        public OracleCommands(IServiceProvider services)
        {
            _oracleService = services.GetRequiredService<OracleService>();
            _client = services.GetRequiredService<DiscordSocketClient>();

            if (!services.GetRequiredService<HookedEvents>().OracleTableReactions)
            {
                var reactionService = services.GetRequiredService<ReactionService>();
                ReactionEvent reaction1 = new ReactionEventBuilder().WithEmoji("\uD83E\uDDE6").WithEvent(PairedTableReactionHandler).Build();

                reactionService.reactionList.Add(reaction1);

                services.GetRequiredService<HookedEvents>().OracleTableReactions = true;
            }
            Services = services;
        }

        private readonly OracleService _oracleService;
        private readonly DiscordSocketClient _client;

        public IServiceProvider Services { get; }

        [Command("OracleTable")]
        [Summary("Rolls an Oracle")]
        [Alias("Oracle", "Table")]
        [Remarks("\uD83E\uDDE6 - Rolls the paired oracle table, and adds it to the post")]
        public async Task OracleRollCommand([Remainder] string TableNameAndOptionalGame)
        {
            ChannelSettings channelSettings = await ChannelSettings.GetChannelSettingsAsync(Context.Channel.Id);

            GameName game = Utilities.GetGameContainedInString(TableNameAndOptionalGame);
            string oracleTable = Utilities.RemoveGameNamesFromString(TableNameAndOptionalGame);

            if (game == GameName.None && channelSettings != null) game = channelSettings.DefaultGame;

            OracleRoller roller = new OracleRoller(Services, game);

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
            ChannelSettings channelSettings = await ChannelSettings.GetChannelSettingsAsync(Context.Channel.Id);
            string reply = $"__Here's a list of available Oracle Tables:__\n";
            foreach (var oracle in _oracleService.OracleList.Where(orc => channelSettings == null || channelSettings.DefaultGame == GameName.None || orc.Game == channelSettings.DefaultGame))
            {
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
                reply = reply.Substring(cutoff + 1).Trim();
            }
        }

        private async Task PairedTableReactionHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (message.Author.Id != _client.CurrentUser.Id) return;

            var pairEmoji = new Emoji("\uD83E\uDDE6");
            if (reaction.Emote.Name == pairEmoji.Name)
            {
                await message.RemoveReactionAsync(pairEmoji, user).ConfigureAwait(false);
                await message.RemoveReactionAsync(pairEmoji, message.Author).ConfigureAwait(false);

                await message.ModifyAsync(msg => msg.Embed = AddRollToExisting(message)).ConfigureAwait(false);
            }

            return;
        }

        private Embed AddRollToExisting(IUserMessage message)
        {
            var embed = message.Embeds.First().ToEmbedBuilder();
            if (!embed.Title.Contains(OracleResources.OracleResult)) throw new ArgumentException("Unknown message type");

            OracleRoller existingRoller = OracleRoller.RebuildRoller(_oracleService, embed, Services);
            var rollerCopy = new List<RollResult>(existingRoller.RollResultList); //Copy the list so we can safely add to it using foreach

            foreach (var rollResult in rollerCopy.Where(tbl => tbl.ParentTable.Pair?.Length > 0))
            {
                var pairedTable = _oracleService.OracleList.Find(tbl => tbl.Name == rollResult.ParentTable.Name);
                if (existingRoller.RollResultList.Any(tbl => tbl.ParentTable.Name == pairedTable.Pair)) continue;

                var roller = new OracleRoller(Services, existingRoller.Game).BuildRollResults(pairedTable.Pair);

                roller.RollResultList.ForEach(res => res.ShouldInline = true);
                rollResult.ShouldInline = true;

                int index = existingRoller.RollResultList.IndexOf(rollResult) + 1;
                existingRoller.RollResultList.InsertRange(index, roller.RollResultList);
            }

            return existingRoller.GetEmbedBuilder().Build();
        }
    }
}