using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TheOracle.GameCore.Oracle;

namespace TheOracle.StarForged.Starships.Tests
{
    [TestClass()]
    public class StarshipTests
    {
        [TestMethod()]
        public void GenerateShipTest()
        {
            var services = new ServiceCollection().AddSingleton(new OracleService().Load()).BuildServiceProvider();

            for (int i = 0; i < 100; i++)
            {
                try
                {
                    var ship = Starship.GenerateShip(services, SpaceRegion.Outlands, $"ship-{i}", 0);
                    ship.AddMission();

                    ship.GetEmbedBuilder();
                    Assert.IsTrue(!ship.Mission.Contains("2x"));
                    Assert.IsTrue(!ship.Mission.Contains("Roll twice"));
                }
                catch (Exception)
                {
                    Console.WriteLine(i);
                    throw;
                }
            }
        }
    }
}