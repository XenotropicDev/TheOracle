using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle.GameCore.Oracle;
using TheOracle.StarForged.Planets;

namespace TheOracle.StarForged.Tests
{
    [TestClass()]
    public class PlanetTests
    {
        [TestMethod()]
        public void GeneratePlanetTest()
        {
            var services = new ServiceCollection().AddSingleton(new OracleService().Load()).BuildServiceProvider();

            for (int i = 0; i < 1000; i++)
            {
                var planet = Planet.GeneratePlanet("P-" + i.ToString(), SpaceRegion.Expanse, services, 0);

                planet = planet.RevealCloserLook().RevealCloserLook().RevealLife();

                var embed = planet.GetEmbedBuilder();
            }
        }
    }
}