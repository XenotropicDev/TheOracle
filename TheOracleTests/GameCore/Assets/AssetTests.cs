using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.GameCore.Oracle.DataSworn;

namespace TheOracle.GameCore.Assets.Tests
{
    [TestClass()]
    public class AssetTests
    {
        [TestMethod()]
        public void AssetToJson()
        {
            var asset1 = new Asset();
            // asset1.AssetRadioSelect = new AssetRadioSelect();
            // asset1.AssetRadioSelect.Options = new List<IAssetRadioOption>();

            asset1.Name = "IRONCLAD";
            asset1.Description = "If you wear armor...";
            asset1.Category = "COMBAT TALENT";
            asset1.IconUrl = "www.someurl.com/image.jpg";

            asset1.AssetAbilities.Add(new AssetAbility { Enabled = true, Text = "When you equip or adjust your armor, choose one.\n• Lightly armored: When you Endure...\n• Geared for war: Mark encumbered..." });
            asset1.AssetAbilities.Add(new AssetAbility { Text = @"When you Clash while you are geared for war, add +1." });
            asset1.AssetAbilities.Add(new AssetAbility { Text = @"When you Compel in a situation where strength of arms is a factor, add +2." });
            // asset1.AssetRadioSelect.Options.Add(new AssetRadioOption { ActiveText = "**Lightly Armored**", InactiveText = "-", Name = "Armor", IsActive = false });
            // asset1.AssetRadioSelect.Options.Add(new AssetRadioOption { ActiveText = "**Geared For War**", InactiveText = "-", Name = "Armor", IsActive = false });

            var asset2 = new Asset();
            asset2.Name = "KINDRED";
            asset2.Description = "Your friend stands by you.";
            asset2.Category = "COMPANION";

            asset2.AssetAbilities.Add(new AssetAbility { Text = @"Skilled: When you make a move outside..." });
            asset2.AssetAbilities.Add(new AssetAbility { Text = @"Shield-Kin: When you Clash or Battle..." });
            asset2.AssetAbilities.Add(new AssetAbility { Text = @"Bonded: Once you mark a bond with..." });
            asset2.AssetTextInput.Add("Name");
            asset2.AssetTextInput.Add("Expertise");
            asset2.AssetConditionMeter = new AssetConditionMeter { Min = 0, Max = 4, ActiveNumber = 0 };

            var asset3 = new Asset();
            asset3.Name = "THUNDER-BRINGER";
            asset3.Description = "If you wield a mighty hammer";
            asset3.Category = "COMBAT TALENT";

            asset3.AssetAbilities.Add(new AssetAbility { Enabled = true, Text = "When you Face Danger, Secure an Advantage, or Compel by hitting or breaking an inanimate object, add + 1 and take + 1 momentum on a hit." });
            asset3.AssetAbilities.Add(new AssetAbility { Text = @"When you Strike a foe to knock them back, stun them, or put them off balance, inflict 1 harm (instead of 2) and take +2 momentum on a hit. On a strong hit, you also create an opening and add +1 on your next move." });

            var jsonSample = new List<Asset> { asset1, asset2, asset3 };

            System.Console.WriteLine(JsonConvert.SerializeObject(jsonSample, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        [TestMethod()]
        public void JsonToAssets()
        {
            var assets = JsonConvert.DeserializeObject<AssetRoot>(File.ReadAllText("IronSworn\\assets.json"));
            Assert.IsTrue(assets.Assets.Count > 1);
        }

        [TestMethod()]
        [DataRow("Seer", GameName.Ironsworn)] //Get asset for different game test
        [DataRow("Stealth Tech", GameName.Starforged)] //Tests multiple assets with similar names (1)
        [DataRow("Tech", GameName.Starforged)] //Tests multiple assets with similar names (1)
        public void FindAssetTest(string assetName, GameName game)
        {
            var AssetList = Asset.LoadAssetList();
            var services = new ServiceCollection()
                .AddSingleton(AssetList)
                .AddSingleton<HookedEvents>()
                .AddSingleton<ReactionService>()
                .BuildServiceProvider();

            var asset = new AssetCommands(services).FindMatchingAsset(assetName, AssetList, game);
            Assert.AreEqual(assetName, asset.Name);

            asset.GetEmbed();
        }
    }
}