using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using TheOracle.IronSworn;

namespace TheOracle.BotCore
{
    //[Group("Configuration")]
    //[Alias("Config")]
    //public class BotConfigCommands : ModuleBase<SocketCommandContext>
    //{
    //    public BotConfigCommands(IConfigurationRoot configuration)
    //    {
    //        Configuration = configuration;
    //    }
    //    public IConfigurationRoot Configuration { get; }

    //    [Command("Language")]
    //    public async Task SetChannelLocalization(string name)
    //    {
    //        await ReplyAsync("Saved.");
    //    }

    //    [Command("DefaultGame")]
    //    public async Task SetDefaultGame(GameName game)
    //    {
    //        var jsonFile = (File.Exists("channelsettings.json")) ? JObject.Parse(File.ReadAllText("channelsettings.json")) : new JObject();


    //        if (jsonFile.TryGetValue(Context.Channel.Id.ToString(), out JToken token))
    //        {
    //        }
    //        else
    //        {
    //            jsonFile.Add(Context.Channel.Id.ToString(), game.ToString());

    //        }

    //        await ReplyAsync("Saved.");
    //    }
    //}
}