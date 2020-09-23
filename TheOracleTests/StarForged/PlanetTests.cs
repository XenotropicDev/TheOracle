using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle.StarForged;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheOracle.StarForged.Tests
{
    [TestClass()]
    public class PlanetTests
    {
        [TestMethod()]
        public void GeneratePlanetTest()
        {
            var p1 = Planet.GeneratePlanet("Test");

            Assert.IsTrue(p1.Atmosphere != default);

            Assert.AreEqual(-1701808026, p1.Seed);
        }
    }
}