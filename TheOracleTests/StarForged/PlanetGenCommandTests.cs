using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle.IronSworn;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using TheOracle.Core;
using System.IO;

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

        [TestMethod()]
        public void LoadJsonTest()
        {
            var oraclesTest = JsonConvert.DeserializeObject<List<OracleList>>(File.ReadAllText("StarForged\\oracles.json"), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            Assert.IsNotNull(oraclesTest);
        }



        public class OracleList
        {
            public string Name { get; set; }
            public List<IOracleEntry> Oracles { get; set; }
        }

    }
}