using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.GameCore.Oracle;

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

            if (!serviceProvider.GetRequiredService<HookedEvents>().NPCReationsLoaded)
            {
                serviceProvider.GetRequiredService<HookedEvents>().NPCReationsLoaded = true;
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => typeof(INpcGenerator).IsAssignableFrom(p) && !p.IsInterface);
                foreach (var type in types)
                {
                    Activator.CreateInstance(type, provider);
                }
            }
        }

        public DiscordSocketClient Client { get; }
        public CommandService Commands { get; }
        public OracleService OracleService { get; }
        public ServiceProvider serviceProvider { get; }

        [Command("NPC")]
        [Alias("CreateNPC", "NewNPC")]
        [Summary("Creates a NPC for the specified game, or default game if none is specified\n• Sample usage: `!NPC Ironsworn Tom Bombadil`")]
        public async Task NPCPost([Remainder] string NPCNameAndOptionalGame = "")
        {
            ChannelSettings channelSettings = await ChannelSettings.GetChannelSettingsAsync(Context.Channel.Id);
            var game = Utilities.GetGameContainedInString(NPCNameAndOptionalGame);

            if (game != GameName.None) NPCNameAndOptionalGame = Utilities.RemoveGameNamesFromString(NPCNameAndOptionalGame);
            if (game == GameName.None && channelSettings != null) game = channelSettings.DefaultGame;

            var NPCGen = new NpcFactory(serviceProvider).GetNPCGenerator(game);

            var msg = await ReplyAsync(embed: NPCGen.Build(NPCNameAndOptionalGame).GetEmbed());
            if (NPCGen.ReactionsToAdd != null)
            {
                await Task.Run(async () =>
                {
                    foreach (var emote in NPCGen.ReactionsToAdd)
                    {
                        await msg.AddReactionAsync(emote);
                    }
                }).ConfigureAwait(false);
            }
        }
    }
}