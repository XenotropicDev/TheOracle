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
        public OracleRollerTests()
        {
            ServiceProvider = new ServiceCollection().AddSingleton(oracleService).BuildServiceProvider();
        }

        private ServiceProvider ServiceProvider;
        private OracleService oracleService = new OracleService().Load();

        [TestMethod()]
        public void BuildRollResultsTest()
        {

            Assert.IsTrue(oracleService.RandomRow("Action", GameName.Ironsworn).Description.Length > 0);
            Assert.IsTrue(oracleService.RandomRow("Theme", GameName.Ironsworn).Description.Length > 0);
            Assert.IsTrue(oracleService.RandomRow("Derelict Type Planetside", GameName.Starforged).Description.Length > 0);
            Assert.IsTrue(oracleService.RandomRow("Settlement Name", GameName.Starforged).Description.Length > 0);

            Assert.ThrowsException<ArgumentException>(() => oracleService.RandomRow("Action"));
        }

        [TestMethod()]
        public void BuildRollResultsTest1()
        {
            for (int i = 0; i < 100; i++)
            {
                var rand = new Random(i);
                var roller = new OracleRoller(ServiceProvider, GameName.Starforged, rand).BuildRollResults("Space Sighting Outlands");
                Console.WriteLine(string.Join(", ", roller.RollResultList.Select(rr => rr.Result?.Description)));
            }
        }

        [TestMethod()]
        public void BuildRollResultsTest2()
        {
            var roller = new OracleRoller(ServiceProvider, GameName.Ironsworn);

            for (int i = 0; i < 100; i++)
            {
                roller.BuildRollResults("Site Name Format");
                Console.WriteLine(string.Join(", ", roller.RollResultList.Select(rr => rr.Result.Description)));
            }

        }

        [TestMethod()]
        public void BuildRollResultsTest3()
        {
            var roller = new OracleRoller(ServiceProvider, GameName.Starforged);
            Assert.ThrowsException<MultipleOraclesException>(() => roller.BuildRollResults("Feature"));
        }

        [TestMethod()]
        //[DataRow("Derelict Operations Feature", GameName.Starforged)]
        [DataRow("creature scale", GameName.Starforged)]
        [DataRow("Settlement Authority", GameName.Starforged)]
        //[DataRow("access zone feature", GameName.Starforged)]
        [DataRow("Character First Look", GameName.Starforged)]
        //[DataRow("Area Access", GameName.Starforged)]
        [DataRow("Lifeless Planetside Opportunity", GameName.Starforged)] //Alias value test
        [DataRow("Planet Opportunity Lifeless", GameName.Starforged)]
        [DataRow("Settlement Name", GameName.Starforged)]
        [DataRow("faction [affiliation/identity]", GameName.Starforged)]
        //[DataRow("Theme Fortified", GameName.Starforged)]
        //[DataRow("Infested Peril", GameName.Starforged)]
        public void OracleTests(string oracleName, GameName game)
        {
            var roller = new OracleRoller(ServiceProvider, game);
            roller.BuildRollResults(oracleName);
        }
    }
}