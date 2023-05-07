using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server.DiscordServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DiscordServer.Tests
{
    [TestClass()]
    public class DiscordHelpersTests
    {
        [TestMethod()]
        public void FormatMarkdownLinksTest()
        {
            var markDownText = @"[Some Link](Should/Not/See/This) some more text [Second Link](Should/Not/See/This)";
            var noLinkText = DiscordHelpers.FormatMarkdownLinks(markDownText);

            Console.WriteLine(noLinkText);
            Assert.AreNotEqual(markDownText, noLinkText);
            Assert.IsFalse(noLinkText.Contains("Should/Not/See/This"), "The markdown string had text links remaining");
        }
    }
}
