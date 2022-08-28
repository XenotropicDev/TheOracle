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
    //[DataRow("Ironsworn/Oracles/Name/Ironlander/A")]
    //[DataRow("Ironsworn/Oracles/Moves/Pay_the_Price")]
    //[DataRow("Starforged/Oracles/Vaults/Interior/First_Look")]
    [DataRow("Starforged/Oracles/Planets/Desert/Settlements/Outlands")]
    public void RollOracleTest(string oracle)
    {
        var firstOracle = oracles.GetOracleById(oracle);
        Assert.IsNotNull(firstOracle);
    }

    [TestMethod()]
    //[DataRow("Guild", "Starforged/Oracles/Factions/Guild")]
    //[DataRow("Vault", "Starforged/Oracles/Vaults/Interior/First_Look")]
    [DataRow("Observed", "Starforged/Oracles/Planets/Desert/Observed_From_Space")]
    public void OracleSearchResultsTest(string query, string desiredOption)
    {
        var desiredOracle = oracles.GetOracleById(desiredOption);
        Assert.IsNotNull(desiredOracle);
        var searchResults = oracles.GetOracles().GetOraclesFromUserInput(query);

        Assert.IsTrue(searchResults.Any(sr => sr.Id == desiredOption), $"Couldn't find {desiredOption} in {query} results");
        Assert.AreEqual(1, searchResults.Count(sr => sr.Id == desiredOption));
    }
}
