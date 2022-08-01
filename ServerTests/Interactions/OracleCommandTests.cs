using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Data;

namespace TheOracle2.Tests;

[TestClass()]
public class OracleCommandTests
{
    IOracleRepository oracles;
    public OracleCommandTests()
    {
        oracles = new JsonOracleRepository();
    }

    [TestMethod()]
    [DataRow("Ironsworn/Oracles/Name/Ironlander/A")]
    [DataRow("Ironsworn/Oracles/Moves/Pay_the_Price")]
    public void RollOracleTest(string oracle)
    {
        var firstOracle = oracles.GetOracleById(oracle);
        Assert.IsNotNull(firstOracle);
    }
}
