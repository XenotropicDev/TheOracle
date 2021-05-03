using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using TheOracle.BotCore;

namespace TheOracle.GameCore.Assets.Tests
{
    [TestClass()]
    public class AssetTests
    {
        [TestMethod()]
        public void AssetToJson()
        {
            var asset1 = new Asset();
            asset1.Name = "IRONCLAD";
            asset1.Description = "If you wear armor...";
            asset1.AssetType = "COMBAT TALENT";
            asset1.IconUrl = "www.someurl.com/image.jpg";

            asset1.AssetFields.Add(new AssetField { Enabled = true, Text = "When you equip or adjust your armor, choose one.\n• Lightly armored: When you Endure...\n• Geared for war: Mark encumbered..." });
            asset1.AssetFields.Add(new AssetField { Text = @"When you Clash while you are geared for war, add +1." });
            asset1.AssetFields.Add(new AssetField { Text = @"When you Compel in a situation where strength of arms is a factor, add +2." });
            asset1.MultiFieldAssetTrack.Fields.Add(new AssetEmbedField { ActiveText = "**Lightly Armored**", InactiveText = "-", Name = "Armor", IsActive = false });
            asset1.MultiFieldAssetTrack.Fields.Add(new AssetEmbedField { ActiveText = "**Geared For War**", InactiveText = "-", Name = "Armor", IsActive = false });

            var asset2 = new Asset();
            asset2.Name = "KINDRED";
            asset2.Description = "Your friend stands by you.";
            asset2.AssetType = "COMPANION";

            asset2.AssetFields.Add(new AssetField { Text = @"Skilled: When you make a move outside..." });
            asset2.AssetFields.Add(new AssetField { Text = @"Shield-Kin: When you Clash or Battle..." });
            asset2.AssetFields.Add(new AssetField { Text = @"Bonded: Once you mark a bond with..." });
            asset2.InputFields.Add("Name");
            asset2.InputFields.Add("Expertise");
            asset2.NumericAssetTrack = new NumericAssetTrack { Min = 0, Max = 4, ActiveNumber = 0 };

            var asset3 = new Asset();
            asset3.Name = "THUNDER-BRINGER";
            asset3.Description = "If you wield a mighty hammer";
            asset3.AssetType = "COMBAT TALENT";

            asset3.AssetFields.Add(new AssetField { Enabled = true, Text = "When you Face Danger, Secure an Advantage, or Compel by hitting or breaking an inanimate object, add + 1 and take + 1 momentum on a hit." });
            asset3.AssetFields.Add(new AssetField { Text = @"When you Strike a foe to knock them back, stun them, or put them off balance, inflict 1 harm (instead of 2) and take +2 momentum on a hit. On a strong hit, you also create an opening and add +1 on your next move." });

            var jsonSample = new List<Asset> { asset1, asset2, asset3 };
            
            
            System.Console.WriteLine(JsonConvert.SerializeObject(jsonSample, Formatting.Indented, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore })); 
        }

        [TestMethod()]
        public void JsonToAssets()
        {
            var assets = JsonConvert.DeserializeObject<List<Asset>>(File.ReadAllText("IronSworn\\assets.json"));
            Assert.IsTrue(assets.Count > 1);
        }

        [TestMethod()]
        public void FindAssetTest()
        {
            var AssetList = Asset.LoadAssetList();
            var services = new ServiceCollection()
                .AddSingleton(AssetList)
                .AddSingleton<HookedEvents>()
                .AddSingleton<ReactionService>()
                .BuildServiceProvider();

            var assets = new AssetCommands(services).FindMatchingAsset("Seer", AssetList, GameName.Ironsworn);
            Assert.AreEqual("Seer", assets.Name);
            Assert.AreEqual(GameName.Starforged, assets.Game);
        }
    }
}