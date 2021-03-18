using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeCantSpell.Hunspell;

namespace TheOracle.IronSworn.Tests
{
    [TestClass()]
    public class OracleCommandsTests
    {
        [TestMethod()]
        public void RollOracleFacadeTest()
        {
            //var oracleService = new OracleService();
            //var commands = new OracleCommands(oracleService);

            //Console.WriteLine(commands.RollOracleFacade("Action"));

            //Assert.ThrowsException<ArgumentException>(() => commands.RollOracleFacade("Action"));
        }

        [TestMethod()]
        public void OracleRollSpellChecker()
        {
            var words = "the quick brown fox jumps over the lazy dog".Split(' ');
            var dictionary = WordList.CreateFromWords(words);
            var suggestion = dictionary.Suggest("teh");
            Assert.IsFalse(dictionary.Check("teh"));
            Assert.IsTrue(dictionary.Check("Fox"));
        }
    }
}