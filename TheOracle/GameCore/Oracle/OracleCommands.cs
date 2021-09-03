using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheOracle.BotCore;
using WeCantSpell.Hunspell;

namespace TheOracle.GameCore.Oracle
{
    public class OracleCommands : InteractiveBase
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

        [Command("PayThePrice")]
        [Alias("Ptp")]
        [Summary("Alias for the command `!OracleTable Pay The Price`")]
        [NoHelp]
        public async Task PayThePriceCommand()
        {
            await OracleRollCommand("Pay The Price");
        }

        [Command("OracleTable", RunMode = RunMode.Async)]
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
                var msg = await ReplyAsync("", false, roller.BuildRollResults(oracleTable).GetEmbed());
                if (roller.RollResultList.Count == 1 && roller.RollResultList[0].ParentTable.Pair?.Length > 0)
                {
                    await msg.AddReactionAsync(new Emoji("\uD83E\uDDE6"));
                }
            }
            catch (ArgumentException ex)
            {
                string word = ex.Message.Replace(OracleResources.UnknownTableError, "");
                var dict = Services.GetService<WordList>();
                var suggestions = dict.Suggest(word);
                Console.WriteLine($"{Context.User} triggered an ArgumentException: {ex.Message}");

                string suggest = (suggestions?.Count() > 0) ? $"\n{String.Format(OracleResources.DidYouMean, suggestions.FirstOrDefault())}" : string.Empty;
                await ReplyAsync($"{ex.Message}{suggest}");
            }
            catch (MultipleOraclesException ex)
            {
                string tableOptions = string.Empty;
                int i = 0;
                foreach (var ot in ex.Tables)
                {
                    i++;
                    tableOptions += $"{i}. {ot.Parent} {ot.Category} {ot.Name} {ot.Requires}\n".Replace("  ", " ");
                }

                var message = await ReplyAsync($"Multiple possible results. Reply with the number of the table you want:\n{tableOptions}");
                var response = await NextMessageAsync(timeout: TimeSpan.FromMinutes(60));

                if (response != null)
                {
                    if (!int.TryParse(response.Content, out int value)) throw new ArgumentException($"Unknown input value");

                    var ot = ex.Tables.ElementAt(value - 1);

                    await message.ModifyAsync(msg =>
                    {
                        msg.Content = "";
                        msg.Embed = roller.BuildRollResults(ot).GetEmbed();
                    });

                    if (roller.RollResultList.Count == 1 && roller.RollResultList[0].ParentTable.Pair?.Length > 0)
                    {
                        await message.AddReactionAsync(new Emoji("\uD83E\uDDE6"));
                    }
                    await response.DeleteAsync();
                }
            }
        }

        private class OracleListEntry
        {
            public OracleListEntry(string category, string entry)
            {
                Category = category;
                Entry = entry;
            }

            public string Category { get; set; }
            public string Entry { get; set; }
        }

        [Command("OracleList", ignoreExtraArgs: false)]
        [Summary("Lists Available Oracles")]
        [Alias("List")]
        public async Task OracleList([Remainder] string OracleListOptions = "")
        {
            var ShowPostInChannel = OracleResources.ShowListInChannel.Split(',').Any(s => OracleListOptions.Contains(s, StringComparison.OrdinalIgnoreCase));
            if (ShowPostInChannel) foreach (var s in OracleResources.ShowListInChannel.Split(',')) OracleListOptions = OracleListOptions.Replace(s, "", StringComparison.OrdinalIgnoreCase);

            var UserGame = Utilities.GetGameContainedInString(OracleListOptions);
            OracleListOptions = Utilities.RemoveGameNamesFromString(OracleListOptions);

            if (UserGame == GameName.None)
            {
                ChannelSettings channelSettings = await ChannelSettings.GetChannelSettingsAsync(Context.Channel.Id);
                UserGame = channelSettings?.DefaultGame ?? GameName.None;
            }

            var baseList = _oracleService.OracleList.Where(orc => UserGame == GameName.None || orc.Game == UserGame || orc.Game == GameName.None);
            baseList.ToList().ForEach(o => 
            { 
                if (o.Category == null || o.Category.Length == 0) o.Category = o.Parent ?? o.Game?.ToString() ?? "Misc"; 
            });

            IEnumerable<string> catsToShow = null;
            if (!string.IsNullOrEmpty(OracleListOptions)) catsToShow = baseList
                        .Where(o => o.Category.Contains(OracleListOptions, StringComparison.OrdinalIgnoreCase))
                        .Select(o => o.Category);

            if (catsToShow?.Count() == 0) throw new ArgumentException($"I don't know any oracles with category '{OracleListOptions}'.");

            List<OracleListEntry> categoryEntryList = new List<OracleListEntry>();

            foreach (var game in baseList.GroupBy(o => o.Game).Select(o => o.First().Game))
            {
                EmbedBuilder builder = new EmbedBuilder().WithTitle(String.Format(OracleResources.OracleListTitle, game));
                string currentCategory = string.Empty;

                foreach (var oracle in baseList.Where(o => (o.Game == game || o.Game == GameName.None) && (catsToShow == default || catsToShow.Contains(o.Category))).OrderBy(o => o.Category))
                {
                    string aliases = string.Empty;
                    if (oracle.Aliases != null)
                    {
                        aliases = $" • {string.Join("\n• ", oracle.Aliases)}";
                    }
                    if (aliases.Length > 0) aliases = $"\n{aliases}";

                    string entry = $" __`{oracle.Name}`__{aliases}";

                    categoryEntryList.Add(new OracleListEntry(oracle.Category ?? oracle.Parent, entry));
                }

                var distinctCatList = categoryEntryList.DistinctBy(c => new { c.Category, c.Entry });
                if (distinctCatList.Count() <= 2) ShowPostInChannel = true;

                
                foreach (var groupEntries in distinctCatList.GroupBy(ol => ol.Category))
                {
                    string content = string.Empty;

                    foreach (var entry in groupEntries)
                    {
                        if (builder.Length + groupEntries.Key.Length + content.Length + entry.Entry.Length > EmbedBuilder.MaxEmbedLength || builder.Fields.Count() + 1 > EmbedBuilder.MaxFieldCount)
                        {
                            if (ShowPostInChannel) await ReplyAsync(embed: builder.Build()).ConfigureAwait(false);
                            else
                            {
                                await SendDMWithFailover(Context.User, Context.Channel, embed: builder.Build());
                            }

                            builder.Fields = new List<EmbedFieldBuilder>();
                            content = string.Empty;
                        }

                        if (content.Length + entry.Entry.Length + 5 > EmbedFieldBuilder.MaxFieldValueLength)
                        {
                            builder.AddField(groupEntries.Key, content, true);
                            content = string.Empty;
                        }

                        content += $"{entry.Entry}\n";
                    }

                    builder.AddField(groupEntries.Key, content, true);

                }

                if (ShowPostInChannel) await ReplyAsync(embed: builder.Build()).ConfigureAwait(false);
                else await SendDMWithFailover(Context.User, Context.Channel, embed: builder.Build()).ConfigureAwait(false);
            }

            if (!ShowPostInChannel) await ReplyAsync(OracleResources.ListSentInDM).ConfigureAwait(false);
        }

        private async Task SendDMWithFailover(SocketUser user, ISocketMessageChannel channel, string msg = null, Embed embed = null)
        {
            try
            {
                await user.SendMessageAsync(msg, embed: embed);
            }
            catch (HttpException ex)
            {
                if (ex.DiscordCode != 50007) throw;

                await channel.SendMessageAsync(msg, embed: embed);
            }
        }

        private async Task PairedTableReactionHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (message.Author.Id != _client.CurrentUser.Id) return;

            var pairEmoji = new Emoji("\uD83E\uDDE6");
            if (reaction.Emote.IsSameAs(pairEmoji))
            {
                await message.ModifyAsync(msg => msg.Embed = AddRollToExisting(message)).ConfigureAwait(false);

                await message.RemoveReactionAsync(pairEmoji, user).ConfigureAwait(false);
                await message.RemoveReactionAsync(pairEmoji, message.Author).ConfigureAwait(false);
            }

            return;
        }

        private Embed AddRollToExisting(IUserMessage message)
        {
            var embed = message.Embeds.First().ToEmbedBuilder();
            if (!embed.Title.Contains(OracleResources.OracleResult)) throw new ArgumentException("Unknown message type");

            OracleRoller existingRoller = OracleRoller.RebuildRoller(_oracleService, embed);
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

            return existingRoller.GetEmbed();
        }
    }
}