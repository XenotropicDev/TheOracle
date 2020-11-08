using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle.StarForged;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using TheOracle.Core;
using TheOracle.StarForged.Planets;

namespace TheOracle.StarForged.Tests
{
    [TestClass()]
    public class PlanetTests
    {
        [TestMethod()]
        public void VitalWorldTest()
        {
            var services = new ServiceCollection().AddSingleton<OracleService>().BuildServiceProvider();
            Planet vitalWorld = null;

            //find a vital world
            for (int i = 0; i < 1000; i++)
            {
                var planet = Planet.GeneratePlanet("P-" + i.ToString(), SpaceRegion.Expanse, services, 0);

                planet.GetEmbedBuilder();
            }
        }
    }
}