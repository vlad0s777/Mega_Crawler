using System;
using Mega.Messaging;
using Mega.Services;
using NUnit.Framework;

namespace Mega.Tests.Services
{
    [TestFixture]
    internal class ArticleUrlParcerTest
    {
        [Test]
        public void AddMessage()
        {
            var pageReports = new MessageBroker<UriBody>();
            var pageMessages = new MessageBroker<UriLimits>();
            var articleMessages = new MessageBroker<UriLimits>();
            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");
            var body = $"<li class='prev'><a href='/page/1485'>1485</a></li><div class='story' " +
                       $"id='story-13494'><h2><a href='/story/13494' > Нужны сильные программисты</a></h2></div>";

            pageReports.Send(new UriBody(rootUri, body));
            var uriFinder = new ArticleUrlParcer(pageMessages, pageReports, articleMessages);
            uriFinder.Work();
            Assert.IsFalse(pageMessages.IsEmpty());
            Assert.IsTrue(pageReports.IsEmpty());
            Assert.IsFalse(articleMessages.IsEmpty());
        }

        [Test]
        public void FalseParcePageTest()
        {
            var pageReports = new MessageBroker<UriBody>();
            var pageMessages = new MessageBroker<UriLimits>();
            var articleMessages = new MessageBroker<UriLimits>();
            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");
            var body =
                $"<div class='story' id='story-13494'><h2><a href='/story/13494' > Нужны сильные программисты</a></h2></div>";
            pageReports.Send(new UriBody(rootUri, body));
            var uriFinder = new ArticleUrlParcer(pageMessages, pageReports, articleMessages);
            uriFinder.Work();
            Assert.IsTrue(pageMessages.IsEmpty());
            Assert.IsTrue(pageReports.IsEmpty());
            Assert.IsFalse(articleMessages.IsEmpty());
        }

        [Test]
        public void FalseParceUrlTest()
        {
            var pageReports = new MessageBroker<UriBody>();
            var pageMessages = new MessageBroker<UriLimits>();
            var articleMessages = new MessageBroker<UriLimits>();
            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");
            var body = $"<li class='prev'><a href='/page/1485'>1485</a></li>";
            pageReports.Send(new UriBody(rootUri, body));
            var uriFinder = new ArticleUrlParcer(pageMessages, pageReports, articleMessages);
            uriFinder.Work();
            Assert.IsFalse(pageMessages.IsEmpty());
            Assert.IsTrue(pageReports.IsEmpty());
            Assert.IsTrue(articleMessages.IsEmpty());
        }

        [Test]
        public void TrueParceTest()
        {
            var pageReports = new MessageBroker<UriBody>();
            var pageMessages = new MessageBroker<UriLimits>();
            var articleMessages = new MessageBroker<UriLimits>();
            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");
            var body = $"<li class='prev'><a href='/page/1485'>1485</a></li><div class='story' " +
                       $"id='story-13494'><h2><a href='/story/13494' > Нужны сильные программисты</a></h2></div>";

            pageReports.Send(new UriBody(rootUri, body));
            var uriFinder = new ArticleUrlParcer(pageMessages, pageReports, articleMessages);
            uriFinder.Work();
            pageMessages.TryReceive(out var uri);
            Assert.AreEqual(uri.Uri.LocalPath, "/page/1485");
            articleMessages.TryReceive(out var uri2);
            Assert.AreEqual(uri2.Uri.LocalPath, "/story/13494");
        }
    }
}