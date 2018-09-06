namespace Mega.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Mega.Services.WebClient;

    using NUnit.Framework;

    [TestFixture]
    internal class ZadolbaliClientTest
    {
        [Test]
        public async Task EmptyTagsTest()
        {
            var articles = new Dictionary<string, ArticleInfo>();
            var body = $"<div class='story'><h2><a href='123'>Нужны сильные программисты</a></h2><div class='meta'><div class='date-time'>"
                       + $"3 декабря 2015, 08:00</div></div><div class='text'><p>1999 год</p></div></div>";

            await new ZadolbaliClient(articles, uri => Task.FromResult(body)).Handle(new Uri("https://someurlu"));

            foreach (var i in articles)
            {
                Assert.AreEqual(DateTime.Parse("3 декабря 2015, 08:00"), i.Value.DateCreate);
                Assert.AreEqual("Нужны сильные программисты", i.Value.Head);
                Assert.AreEqual("<p>1999 год</p>", i.Value.Text);
                Assert.IsEmpty(i.Value.Tags);
            }
        }

        [Test]
        public async Task EmptyTextTest()
        {
            var articles = new Dictionary<string, ArticleInfo>();
            var body = $"<h2><a href='123'>Нужны сильные программисты</a></h2> "
                + $"<div class='meta'><div class='date-time'> 3 декабря 2015, 08:00</div><div class='tags'><i class='icon-tags'></i>"
                + $"<ul><li><a href = '/tag/longago' > давным - давно </ a >"
                + $"</li><li><a href='/tag/only-in-russia'>только в России</a></li></ul></div></div>";

            await new ZadolbaliClient(articles, uri => Task.FromResult(body)).Handle(new Uri("https://someurlu"));

            Assert.IsEmpty(articles);
        }

        [Test]
        public async Task TrueParceTest()
        {
            var articles = new Dictionary<string, ArticleInfo>();

            var body = $"<h1>Нужны сильные программисты</h1> "
                + $"<div class='meta'><div class='date-time'> 3 декабря 2015, 08:00</div><div class='tags'><i class='icon-tags'></i>"
                + $"<ul><li><a href = '/tag/longago' > давным - давно </ a >"
                + $"</li><li><a href='/tag/only-in-russia'>только в России</a></li></ul></div></div><div class='text'><p>1999 год</p> </div>";

            await new ZadolbaliClient(articles, uri => Task.FromResult(body)).Handle(new Uri("https://someurlu"));

            foreach (var i in articles)
            {
                Assert.AreEqual(DateTime.Parse("3 декабря 2015, 08:00"), i.Value.DateCreate);
                Assert.AreEqual("Нужны сильные программисты", i.Value.Head);
                Assert.AreEqual("<p>1999 год</p>", i.Value.Text);
                Assert.AreEqual("давным - давно", i.Value.Tags["/tag/longago"]);
                Assert.AreEqual("только в России", i.Value.Tags["/tag/only-in-russia"]);
            }
        }

        [Test]
        public async Task TruePrevPageTest()
        {
            var body = $"<li class='prev'><a href='https://prevurl'></a></li>";

            var prevUri = await new ZadolbaliClient(new Dictionary<string, ArticleInfo>(), url => Task.FromResult(body)).Handle(new Uri("https://someurlu"));

            Assert.AreEqual("https://prevurl/", prevUri.AbsoluteUri);        
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