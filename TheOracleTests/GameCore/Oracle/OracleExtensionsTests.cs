using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using TheOracle.Core;
using System.Linq;

namespace TheOracle.Tests
{
    [TestClass()]
    public class OracleExtensionsTests
    {
        [TestMethod()]
        public void GetOracleResultTest()
        {
            var services = new ServiceCollection().AddSingleton<OracleService>().BuildServiceProvider();

            var testTable = services.GetRequiredService<OracleService>().OracleList.FirstOrDefault(tbl => tbl.Oracles.Any(oracle => oracle.Description == "[2x]"));
            Assert.IsNotNull(testTable, "Couldn't find a 2x table");

            var oracle = testTable.Oracles.First(oracle => oracle.Description == "[2x]");
            var final = oracle.GetOracleResult(services, testTable.Game.Value);

            Assert.Inconclusive();
        }
    }
}