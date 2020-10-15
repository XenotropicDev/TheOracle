using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TheOracle.Core;

namespace TheOracle.GameCore.Oracle.Tests
{
    [TestClass()]
    public class OracleRollerTests
    {
        [TestMethod()]
        [TestCategory("Integration")]
        public void BuildRollResultsTest()
        {
            var oracleService = new OracleService();

            oracleService.RandomRow("Action", GameName.Ironsworn);
            oracleService.RandomRow("Theme", GameName.Ironsworn);

            Assert.ThrowsException<ArgumentException>(() => oracleService.RandomRow("Action"));
        }
    }
}