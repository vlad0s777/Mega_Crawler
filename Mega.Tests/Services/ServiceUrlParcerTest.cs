using System;
using Mega.Messaging;
using Mega.Services;
using NUnit.Framework;

namespace Mega.Tests.Services
{
    [TestFixture]
    internal class ServiceUrlParcerTest
    {
        [Test]
        public void AddMessage()
        {
            var pageReports = new MessageBroker<UriBody>();
            var pageMessages = new MessageBroker<UriLimits>();
            var articleMessages = new MessageBroker<UriLimits>();

            pageReports.Send(new UriBody(
                uri: "https://someurl",
                body: $"<li class='prev'><a href='/page/1485'>1485</a></li><div class='story' " +
                      $"id='story-13494'><h2><a href='/story/13494' > Нужны сильные программисты</a></h2></div>"));

            new ServiceUrlParcer(pageMessages, pageReports, articleMessages).Work();

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
       
            pageReports.Send(new UriBody(
                uri: "https://someurl", 
                body: $"<div class='story' id='story-13494'><h2><a href='/story/13494' > Нужны сильные программисты</a></h2></div>"));

            new ServiceUrlParcer(pageMessages, pageReports, articleMessages).Work();

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

            pageReports.Send(new UriBody(
                uri: "https://someurl", 
                body: $"<li class='prev'><a href='/page/1485'>1485</a></li>"));

            new ServiceUrlParcer(pageMessages, pageReports, articleMessages).Work();

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

            pageReports.Send(new UriBody(
                uri: "https://someurl/",
                body: $"<li class='prev'><a href='/page/1485'>1485</a></li><div class='story' " +
                      $"id='story-13494'><h2><a href='/story/13494' > Нужны сильные программисты</a></h2></div>"));

            new ServiceUrlParcer(pageMessages, pageReports, articleMessages).Work();

            Assert.IsTrue(pageMessages.TryReceive(out var uri));
            Assert.AreEqual("/page/1485", uri.Uri.LocalPath);
            Assert.IsTrue(articleMessages.TryReceive(out var uri2));
            Assert.AreEqual("/story/13494",uri2.Uri.LocalPath);
        }
    }
}