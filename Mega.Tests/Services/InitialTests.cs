namespace Mega.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Mega.Services;

    using NUnit.Framework;

    [TestFixture]
    internal class InitialTests
    {
        [Test]
        public void GetTagsTest()
        {
            var checkTagKey = new List<string> { "banks", "household", "hometech" };
            var checkTagName = new List<string> { "банки", "быт", "бытовая техника" };
            var body =
                "<ul class=\"cloud\" id=\"cloud\">\r\n<li class=\"tag4\" data-count=\"385\"><a href=\"/tag/banks\">банки</a></li><li class=\"tag5\" data-count=\"1070\"><a href=\"/tag/household\">быт</a></li><li class=\"tag4\" data-count=\"307\"><a href=\"/tag/hometech\">бытовая техника</a></li>\r\n</ul>";
            var tags = new Initial(new Settings("https://someurl/"), null).GetTags(x => body);
            for (var i = 0; i < tags.Count; i++)
            {
                Assert.AreEqual(checkTagKey[i], tags[i].TagKey);
                Assert.AreEqual(checkTagName[i], tags[i].Name);
            }
        }

        [Test]
        public void GenerateIdTest()
        {
            var dates = new Initial(new Settings("https://someurl/"), null).GenerateIDs(DateTime.Now.AddDays(-1)).ToList();
            Assert.AreEqual(DateTime.Now.ToString("yyyyMMdd"), dates.First());
            Assert.AreEqual(DateTime.Now.AddDays(-1).ToString("yyyyMMdd"), dates.Last());
        }
    }
}
