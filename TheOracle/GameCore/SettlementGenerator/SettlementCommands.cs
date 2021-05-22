using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.BotCore;

namespace TheOracle.GameCore.SettlementGenerator
{
    public class SettlementCommands : ModuleBase<SocketCommandContext>
    {
        private IServiceProvider Services;

        public SettlementCommands(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;

            if (!serviceProvider.GetRequiredService<HookedEvents>().NPCReationsLoaded)
            {
                serviceProvider.GetRequiredService<HookedEvents>().NPCReationsLoaded = true;
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => typeof(ISettlement).IsAssignableFrom(p) && !p.IsInterface);
                foreach (var type in types)
                {
                    ulong channelId = 0;
                    Activator.CreateInstance(type, Services, channelId);
                }
            }
        }

        [Command("GenerateSettlement", ignoreExtraArgs: true)]
        [Summary("Creates an interactive post for a settlement")]
        [Remarks("\uD83D\uDEE0 - Adds a settlement project\n" +
            "☎️ - Adds a settlement contact\n" +
            "🔥 - Adds a settlement trouble" +
            "📍 - Adds/rerolls the ironlands region (Ironsworn only)"
            )]

        [Alias("Settlement")]
        public async Task SettlementPost([Remainder] string SettlementNameAndOptions = "")
        {
            ChannelSettings channelSettings = await ChannelSettings.GetChannelSettingsAsync(Context.Channel.Id);
            var game = Utilities.GetGameContainedInString(SettlementNameAndOptions);

            if (game != GameName.None) SettlementNameAndOptions = Utilities.RemoveGameNamesFromString(SettlementNameAndOptions);
            if (game == GameName.None && channelSettings != null) game = channelSettings.DefaultGame;

            ISettlement SettlementGen = new SettlementFactory(Services, Context.Channel.Id).GetGenerator(game, SettlementNameAndOptions);

            SettlementGen.SetupFromUserOptions(SettlementNameAndOptions);

            var msg = await ReplyAsync(embed: SettlementGen.GetEmbedBuilder().Build());
            await SettlementGen.AfterMessageCreated(msg).ConfigureAwait(false);
        }
    }
}