using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle.GameCore.OracleObjectTemplate;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheOracle.GameCore.OracleObjectTemplate.Tests
{
    [TestClass()]
    public class OracleEmbedTests
    {
        [TestMethod()]
        public void FillinTemplateTest()
        {
            OracleEmbed oe = new OracleEmbed();
            oe.FillinTemplate();
        }
    }
}