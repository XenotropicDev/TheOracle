using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle.BotCore;
using TheOracle.GameCore.Oracle;

namespace TheOracle.StarForged.Settlements.Tests
{
    [TestClass()]
    public class StarSettlementTests
    {
        [TestMethod()]
        public void GenerateSettlementTest()
        {
            var services = new ServiceCollection()
                .AddSingleton(new OracleService().Load())
                .AddSingleton<HookedEvents>()
                .AddSingleton<ReactionService>()
                .BuildServiceProvider();

            var settlement = new StarSettlement(services, 1).WithName("TEST").WithRegion(SpaceRegion.Expanse).WithLocation("Planetside").GenerateSettlement();
            settlement.GetEmbedBuilder();
        }

        [TestMethod()]
        public void SetupFromUserOptionsTest()
        {
            var services = new ServiceCollection()
                .AddSingleton(new OracleService().Load())
                .AddSingleton<HookedEvents>()
                .AddSingleton<ReactionService>()
                .BuildServiceProvider();

            var settlement = new StarSettlement(services, 1);
            settlement.SetupFromUserOptions("PlanetSide   test  outlands");
            Assert.AreEqual("test", settlement.Name);
            Assert.AreEqual(SpaceRegion.Outlands, settlement.Region);
            Assert.AreEqual("Planetside", settlement.Location);

            settlement = new StarSettlement(services, 1);
            settlement.SetupFromUserOptions("test");
            Assert.AreEqual("test", settlement.Name);

            settlement = new StarSettlement(services, 1);
            settlement.SetupFromUserOptions("");
        }
    }
}