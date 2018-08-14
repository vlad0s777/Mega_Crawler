using System;
using System.Collections.Generic;
using Mega.Messaging;
using Mega.Services;
using NUnit.Framework;

namespace Mega.Tests.Services
{
    [TestFixture]
    internal class ArticleInfoParcerTest
    {
        [Test]
        public void EmptyTagsTest()
        {
            var infoDictionary = new Dictionary<string, ArticleInfo>();
            var articleReports = new MessageBroker<UriBody>();
            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");
            var body = $"<h1>Нужны сильные программисты</h1> " +
                       $"<div class='meta'><div class='date-time'> 3 декабря 2015, 08:00</div></div><div class='text'><p>1999 год</p> </div>";
            articleReports.Send(new UriBody(rootUri, body));
            var uriFinder = new ArticleInfoParcer(infoDictionary, articleReports);
            uriFinder.Work();
            foreach (var i in infoDictionary)
            {
                Assert.AreEqual(i.Value.DateCreate, TimeSpan.Parse("3 декабря 2015, 08:00"));
                Assert.AreEqual(i.Value.Head, "Нужны сильные программисты");
                Assert.AreEqual(i.Value.Text, "<p>1999 год</p>");
                Assert.IsEmpty(i.Value.Tags);
            }
        }

        [Test]
        public void EmptyTextTest()
        {
            var infoDictionary = new Dictionary<string, ArticleInfo>();
            var articleReports = new MessageBroker<UriBody>();
            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");
            var body = $"<h1>Нужны сильные программисты</h1> " +
                       $"<div class='meta'><div class='date-time'> 3 декабря 2015, 08:00</div><div class='tags'><i class='icon-tags'></i>" +
                       $"<ul><li><a href = '/tag/longago' > давным - давно </ a >" +
                       $"</li><li><a href='/tag/only-in-russia'>только в России</a></li></ul></div></div>";
            articleReports.Send(new UriBody(rootUri, body));
            var uriFinder = new ArticleInfoParcer(infoDictionary, articleReports);
            uriFinder.Work();
            Assert.IsEmpty(infoDictionary);
        }

        [Test]
        public void TrueParceTest()
        {
            var infoDictionary = new Dictionary<string, ArticleInfo>();
            var articleReports = new MessageBroker<UriBody>();
            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");
            var body = $"<h1>Нужны сильные программисты</h1> " +
                       $"<div class='meta'><div class='date-time'> 3 декабря 2015, 08:00</div><div class='tags'><i class='icon-tags'></i>" +
                       $"<ul><li><a href = '/tag/longago' > давным - давно </ a >" +
                       $"</li><li><a href='/tag/only-in-russia'>только в России</a></li></ul></div></div><div class='text'><p>1999 год</p> </div>";
            articleReports.Send(new UriBody(rootUri, body));
            var uriFinder = new ArticleInfoParcer(infoDictionary, articleReports);
            uriFinder.Work();
            foreach (var i in infoDictionary)
            {
                Assert.AreEqual(i.Value.DateCreate, TimeSpan.Parse("3 декабря 2015, 08:00"));
                Assert.AreEqual(i.Value.Head, "Нужны сильные программисты");
                Assert.AreEqual(i.Value.Text, "<p>1999 год</p>");
                Assert.AreEqual(i.Value.Tags["/tag/longago"], "давным - давно");
                Assert.AreEqual(i.Value.Tags["/tag/only-in-russia"], "только в России");
            }
        }
    }
}