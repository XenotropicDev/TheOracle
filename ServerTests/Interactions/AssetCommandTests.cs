using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.GameInterfaces;
using TheOracle2.Data;
using Server.Data;
using TheOracle2.UserContent;
using System.Text.RegularExpressions;

namespace TheOracle2.Tests
{
    [TestClass()]
    public class AssetCommandTests
    {
        IAssetRepository assetRepo;
        public AssetCommandTests()
        {
            assetRepo = new JsonAssetRepository();
        }

        [TestMethod()]
        public void BuildAssetTest()
        {
            var assetInfo = assetRepo.GetAsset("Ironsworn/Assets/Combat_Talent/Skirmisher");
            var AssetData = new AssetData(assetInfo, 0);

            var discordEntity = new DiscordAssetEntity(assetInfo, AssetData);

            Assert.IsNotNull(discordEntity);
            Assert.AreEqual("Skirmisher", assetInfo.Name);

            var markDownRegex = new Regex(@"\[.*\]\(.*\)");
            Assert.IsFalse(markDownRegex.IsMatch(discordEntity.GetEmbed().Description), "Embed description has unhandeled markdown formating");
        }
    }
}
