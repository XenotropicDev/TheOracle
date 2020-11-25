using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace TheOracle.GameCore.DiceRoller.Tests
{
    [TestClass()]
    public class GenericDieRollerTests
    {
        [TestMethod()]
        public void GenericDieRollerTest()
        {
            var notation1D6 = new GenericDieRoller("1d6");
            var notation2D6 = new GenericDieRoller("2d6");
            var notation2D6p2 = new GenericDieRoller("2d6+2");
            var notationD6 = new GenericDieRoller("d6");
            var notation3D10x4 = new GenericDieRoller("3d10", 4);
            var notation1D2p6 = new GenericDieRoller("1d2+6");

            Assert.IsTrue(notationD6.RollTotal > 0 && notationD6.RollTotal <= 6);
            Assert.IsTrue(notation2D6p2.RollTotal >= 4 && notation2D6p2.RollTotal <= 14);
            Assert.IsTrue(notation1D2p6.RollTotal >= 7 && notation1D2p6.RollTotal <= 8, "Bonus Test");

            bool hasMax = false;
            for (int i = 0; i < 100; i++)
            {
                notation1D6 = new GenericDieRoller("1d6");
                Assert.IsTrue(notation1D6.RollTotal >= 1);
                Assert.IsTrue(notation1D6.RollTotal <= 6);
                if (notation1D6.RollTotal == 6) hasMax = true;
            }
            Assert.IsTrue(hasMax);
        }
    }
}