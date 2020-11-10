using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle.StarForged.Starships;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using TheOracle.Core;

namespace TheOracle.StarForged.Starships.Tests
{
    [TestClass()]
    public class StarshipTests
    {
        [TestMethod()]
        public void GenerateShipTest()
        {
            var services = new ServiceCollection().AddSingleton<OracleService>().BuildServiceProvider();

            for (int i = 0; i < 1000; i++)
            {
                try
                {
                    var ship = Starship.GenerateShip(services, SpaceRegion.Outlands, $"ship-{i}", 0);

                    ship.GetEmbedBuilder();
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