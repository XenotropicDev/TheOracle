using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using TheOracle.Core;
using TheOracle.IronSworn;

namespace TheOracle.GameCore.NpcGenerator
{
    public class NpcCommands : ModuleBase<SocketCommandContext>
    {
        public NpcCommands(DiscordSocketClient client, OracleService oracleService)
        {
            //Add client via DI, needed for reaction events
            Client = client;
            OracleService = oracleService;
        }

        public DiscordSocketClient Client { get; }
        public OracleService OracleService { get; }

        [Command("CreateNPC")]
        [Summary("Creates a NPC with a role, goal, and descriptor")]
        [Alias("NewNPC", "NPC")]
        public async Task CreateNPC(string NPCCreationOptions = "")
        {
            var game = GameName.Ironsworn;

            NPCGenerator npcGen = null;

            if (game == GameName.Ironsworn)
            {
                var npc = new IronNPC(OracleService, NPCCreationOptions) { };

                npcGen = npc;
            }

            if (npcGen != null) await ReplyAsync(embed: npcGen.GetEmbed());

            return;
        }
    }
}