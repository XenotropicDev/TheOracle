using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using TheOracle.Core;

namespace TheOracle.GameCore.Oracle.Tests
{
    [TestClass()]
    public class OracleRollerTests
    {
        [TestMethod()]
        public void BuildRollResultsTest()
        {
            var oracleService = new OracleService().Load();

            Assert.IsTrue(oracleService.RandomRow("Action", GameName.Ironsworn).Description.Length > 0);
            Assert.IsTrue(oracleService.RandomRow("Theme", GameName.Ironsworn).Description.Length > 0);
            Assert.IsTrue(oracleService.RandomRow("Derelict Type Planetside", GameName.Starforged).Description.Length > 0);

            Assert.ThrowsException<ArgumentException>(() => oracleService.RandomRow("Action"));
        }

        [TestMethod()]
        public void BuildRollResultsTest1()
        {
            var services = new ServiceCollection().AddSingleton(new OracleService().Load()).BuildServiceProvider();

            for (int i = 0; i < 100; i++)
            {
                var rand = new Random(i);
                var roller = new OracleRoller(services, GameName.Starforged, rand).BuildRollResults("Space Sighting Outlands");
                Console.WriteLine(string.Join(", ", roller.RollResultList.Select(rr => rr.Result?.Description)));
            }
        }

        [TestMethod()]
        public void BuildRollResultsTest2()
        {
            var services = new ServiceCollection().AddSingleton(new OracleService().Load()).BuildServiceProvider();

            var roller = new OracleRoller(services, GameName.Ironsworn);

            for (int i = 0; i < 100; i++)
            {
                roller.BuildRollResults("Site Name Format");
                Console.WriteLine(string.Join(", ", roller.RollResultList.Select(rr => rr.Result.Description)));
            }
            
        }
    }
}