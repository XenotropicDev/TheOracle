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
            var ol = new List<OracleList>();
            var item1 = new OracleList
            {
                Name = "First",
                Oracles = new List<IOracleEntry> { new StandardOracle {d = 100, Chance = 50, Description = "Test Desc1", type = OracleType.standard },
                                                                                                new StandardOracle {d = 100, Chance = 100, Description = "Test Desc2", type = OracleType.standard }}
            };
            var item2 = new OracleList
            {
                Name = "Second",
                Oracles = new List<IOracleEntry> { new StandardOracle {d = 100, Chance = 50, Description = "Test Desc1", type = OracleType.standard },
                                                                                                new StandardOracle {d = 100, Chance = 100, Description = "Test Desc2", type = OracleType.standard }}
            };

            ol.Add(item1);
            ol.Add(item2);

            string json = JsonConvert.SerializeObject(ol, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All});
            Console.WriteLine(json);

            //var oraclesTest = JsonConvert.DeserializeObject<List<OracleList>>(File.ReadAllText("StarForged\\oracles.json"), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            //Assert.IsNotNull(oraclesTest);
        }



        public class OracleList
        {
            public string Name { get; set; }
            public List<IOracleEntry> Oracles { get; set; }
        }

    }
}