namespace Mega.Tests.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Mega.Services.WebClient;

    using NUnit.Framework;

    [TestFixture]
    internal class ZadolbaliClientTest
    {
        [Test]
        public async Task EmptyTagsTest()
        {
            var body = $"<div class='story'><h2><a href='1123'>Нужны сильные программисты</a></h2><div class='meta'><div class='date-time'>"
                       + $"3 декабря 2015, 08:00</div></div><div class='text'><p>1999 год</p></div></div>";

            var article = (await new ZadolbaliClient(Task.FromResult).GetArticles(body)).First();

            Assert.AreEqual(DateTime.Parse("3 декабря 2015, 08:00"), article.DateCreate);
            Assert.AreEqual("Нужны сильные программисты", article.Head);
            Assert.AreEqual("<p>1999 год</p>", article.Text);
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
            Assert.AreEqual("<p>1999 год</p>", article.Text);
            Assert.AreEqual("давным - давно", article.Tags["/tag/longago"]);
            Assert.AreEqual("только в России", article.Tags["/tag/only-in-russia"]);
        }

        [Test]
        public async Task TruePrevPageTest()
        {
            var body = $"<li class='prev'><a href='/123'></a></li>";

            var prevUri = (await new ZadolbaliClient(Task.FromResult).GetArticles(body)).RelatedPageIds.First();

            Assert.AreEqual("123", prevUri);        
        }

        [Test]
        public void GetDateTest()
        {
            Assert.AreEqual(DateTime.Today.AddHours(14).AddMinutes(48), ZadolbaliClient.GetDate("Сегодня, 14:48"));
            Assert.AreEqual(DateTime.Today.AddDays(-1).AddHours(14).AddMinutes(48), ZadolbaliClient.GetDate("Вчера, 14:48"));
            Assert.AreEqual(DateTime.Parse("4 сентября 2018, 14:48"), ZadolbaliClient.GetDate("4 сентября, 14:48"));
            Assert.AreEqual(DateTime.Parse("4 сентября 2015, 14:48"), ZadolbaliClient.GetDate("4 сентября 2015, 14:48"));
        }
    }
}