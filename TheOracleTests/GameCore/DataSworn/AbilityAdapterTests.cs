using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle.GameCore.Oracle.DataSworn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOracle.GameCore.Oracle.DataSworn.Tests
{
    [TestClass()]
    public class AbilityAdapterTests
    {
        [TestMethod()]
        public void ShallowCopyTest()
        {
            var abilty = new Ability();
            abilty.Enabled = true;
            var adapter = new AbilityAdapter(abilty);

            var clone = adapter.ShallowCopy();
            clone.Enabled = false;

            var clone2 = adapter.ShallowCopy();

            Assert.AreNotEqual(clone.Enabled, clone2.Enabled);
        }
    }
}