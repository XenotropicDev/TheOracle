using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle.BotCore;
using TheOracle.GameCore.Oracle;

namespace TheOracle.IronSworn.Settlements.Tests
{
    [TestClass()]
    public class IronSettlementTests
    {
        [TestMethod()]
        public void SetupFromUserOptionsTest()
        {
            var services = new ServiceCollection()
                .AddSingleton(new OracleService().Load())
                .AddSingleton<HookedEvents>()
                .AddSingleton<ReactionService>()
                .BuildServiceProvider();

            var settlement = new IronSettlement(services, 1);
            settlement.SetupFromUserOptions("test");
            Assert.AreEqual("test", settlement.Name);

            settlement = new IronSettlement(services, 1);
            settlement.SetupFromUserOptions("");

            Assert.IsTrue(settlement.Name.Length > 0);
        }
    }
}