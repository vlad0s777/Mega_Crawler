namespace Mega.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Mega.Services.ZadolbaliClient;

    using NUnit.Framework;

    [TestFixture]
    internal class ZadolbaliClientTests
    {
        [Test]
        public async Task EmptyTagsTest()
        {
            var body = $"<div class='story'><h2><a href='1123'>Нужны сильные программисты</a></h2><div class='meta'><div class='date-time'>"
                       + $"3 декабря 2015, 08:00</div></div><div class='text'><p>1999 год</p></div></div>";

            var article = (await new ZadolbaliClient(Task.FromResult).GetArticles(body)).First();

            Assert.AreEqual(DateTime.Parse("3 декабря 2015, 08:00"), article.DateCreate);
            Assert.AreEqual("Нужны сильные программисты", article.Head);
            Assert.AreEqual("1999 год", article.Text);
            Assert.IsEmpty(article.Tags);
        }

        [Test]
        public async Task EmptyTextTest()
        {
            var body = $"<h2><a href='123'>Нужны сильные программисты</a></h2> "
                + $"<div class='meta'><div class='date-time'> 3 декабря 2015, 08:00</div><div class='tags'><i class='icon-tags'></i>"
                + $"<ul><li><a href = '/tag/longago' > давным - давно </ a >"
                + $"</li><li><a href='/tag/only-in-russia'>только в России</a></li></ul></div></div>";

            Assert.AreEqual(0, (await new ZadolbaliClient(Task.FromResult).GetArticles(body)).Count);
        }

        [Test]
        public async Task TrueParceTest()
        {
            var body = $"<div class='story'><h2><a href='123'>Нужны сильные программисты</a></h2>"
                       + $"<div class='meta'><div class='date-time'>3 декабря 2015, 08:00</div><div class='tags'><i class='icon-tags'></i>"
                       + $"<ul><li><a href = '/tag/longago'>давным - давно</a>"
                       + $"</li><li><a href='/tag/only-in-russia'>только в России</a></li></ul></div></div><div class='text'><p>1999 год</p></div></div>";

            var article = (await new ZadolbaliClient(Task.FromResult).GetArticles(body)).First();

            Assert.AreEqual(DateTime.Parse("3 декабря 2015, 08:00"), article.DateCreate);
            Assert.AreEqual("Нужны сильные программисты", article.Head);
            Assert.AreEqual("1999 год", article.Text);
            Assert.AreEqual("давным - давно", article.Tags[0].Name);
            Assert.AreEqual("только в России", article.Tags[1].Name);
        }

        [Test]
        public void GetDateTest()
        {
            Assert.AreEqual(DateTime.Today.AddHours(14).AddMinutes(48), ZadolbaliClient.GetDate("Сегодня, 14:48"));
            Assert.AreEqual(DateTime.Today.AddDays(-1).AddHours(14).AddMinutes(48), ZadolbaliClient.GetDate("Вчера, 14:48"));
            Assert.AreEqual(DateTime.Parse("4 сентября 2018, 14:48"), ZadolbaliClient.GetDate("4 сентября, 14:48"));
            Assert.AreEqual(DateTime.Parse("4 сентября 2015, 14:48"), ZadolbaliClient.GetDate("4 сентября 2015, 14:48"));
        }

        [Test]
        public async Task GetTagsTest()
        {
            var checkTagKey = new List<string> { "banks", "household", "hometech" };
            var checkTagName = new List<string> { "банки", "быт", "бытовая техника" };
            var body =
                "<ul class=\"cloud\" id=\"cloud\">\r\n<li class=\"tag4\" data-count=\"385\"><a href=\"/tag/banks\">банки</a></li><li class=\"tag5\" data-count=\"1070\"><a href=\"/tag/household\">быт</a></li><li class=\"tag4\" data-count=\"307\"><a href=\"/tag/hometech\">бытовая техника</a></li>\r\n</ul>";

            var tags = await new ZadolbaliClient(async _ => body).GetTags();
            for (var i = 0; i < tags.Count; i++)
            {
                Assert.AreEqual(checkTagKey[i], tags[i].TagKey);
                Assert.AreEqual(checkTagName[i], tags[i].Name);
            }
        }

        [Test]
        public async Task GenerateIdTest()
        {
            var body = $"<ul><li class='first'><a href='zadolba.li/{DateTime.Now.AddDays(-1).Date:yyyyMMdd}'/></li></ul>";
            var dates = await new ZadolbaliClient(async _ => body).GenerateIDs();
            Assert.AreEqual(DateTime.Now.ToString("yyyyMMdd"), dates.First());
            Assert.AreEqual(DateTime.Now.AddDays(-1).ToString("yyyyMMdd"), dates.Last());
        }
    }
}