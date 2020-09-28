using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle.IronSworn;
using System;
using System.Collections.Generic;
using System.Text;
using TheOracle.Core;

namespace TheOracle.IronSworn.Tests
{
    [TestClass()]
    public class OracleCommandsTests
    {
        [TestMethod()]
        public void RollOracleFacadeTest()
        {
            var oracleService = new OracleService();
            var commands = new OracleCommands(oracleService);
            Console.WriteLine(commands.RollOracleFacade("Location"));
        }
    }
}