using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle.IronSworn;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheOracle.IronSworn.Tests
{
    [TestClass()]
    public class PlanetGenCommandTests
    {
        [TestMethod()]
        public void GeneratePlanetTest()
        {
            var p = new PlanetCommands().GeneratePlanet();
        }
    }
}