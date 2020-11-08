using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheOracle.BotCore;

namespace TheOracle.GameCore.Assets
{
    public class AssetCommands : ModuleBase<SocketCommandContext>
    {
        public AssetCommands(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }

        [Command("Asset")]
        public async Task StandardAsset([Remainder] string AssetCommand)
        {
            var assets = Services.GetRequiredService<List<Asset>>();

            var asset = assets.FirstOrDefault(a => AssetCommand.Contains(a.Name, StringComparison.OrdinalIgnoreCase));
            if (asset == default) throw new ArgumentException(AssetResources.UnknownAssetError);

            string additionalInputsRaw = AssetCommand.ReplaceFirst(asset.Name, "").Replace("  ", " ").Trim();

            string[] seperators = new string[] { " " };
            if (additionalInputsRaw.Contains(",") || additionalInputsRaw.Contains("\n"))
            {
                seperators = new string[] { ",", "\n" };
                additionalInputsRaw.Replace("\"", string.Empty);
            }
            string[] arguments = additionalInputsRaw.Split(seperators, StringSplitOptions.RemoveEmptyEntries);

            await ReplyAsync(embed:asset.GetEmbed(arguments)).ConfigureAwait(false);
        }
    }


}