using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.Core;
using TheOracle.IronSworn;

namespace TheOracle.GameCore.NpcGenerator
{

    public class NpcCommands : ModuleBase<SocketCommandContext>
    {
        public NpcCommands(DiscordSocketClient client, OracleService oracleService, ServiceProvider provider)
        {
            //Add client via DI, needed for reaction events
            Client = client;
            OracleService = oracleService;
            serviceProvider = provider;
        }

        public DiscordSocketClient Client { get; }
        public CommandService Commands { get; }
        public OracleService OracleService { get; }
        public ServiceProvider serviceProvider { get; }

        [Command("NPC")]
        [Alias("CreateNPC", "NewNPC")]
        public async Task NPCPost([Remainder]string NPCArguments = "")
        {
            ChannelSettings channelSettings = await ChannelSettings.GetChannelSettingsAsync(Context.Channel.Id);
            var game = Utilities.GetGameContainedInString(NPCArguments);

            if (game != GameName.None) NPCArguments = Utilities.RemoveGameNamesFromString(NPCArguments);
            if (game == GameName.None) game = channelSettings.DefaultGame;

            var NPCGen = new NpcFactory(serviceProvider).GetNPCGenerator(game);

            await ReplyAsync(embed: NPCGen.Build(NPCArguments).GetEmbed());
        }
    }
}