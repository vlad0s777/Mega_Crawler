namespace Mega.Tests.Services
{
    using System;
    using System.Collections.Generic;

    using Mega.Crawler.Infrastructure.IoC;
    using Mega.Messaging;
    using Mega.Services;
    using Mega.Services.ContentCollector;
    using Mega.Services.InfoParser;

    using NUnit.Framework;

    [TestFixture]
    internal class ServiceInfoParserTest
    {
        [Test]
        public void EmptyTagsTest()
        {
            var articles = new Dictionary<string, ArticleInfo>();
            var requests = new MessageBroker<UriRequest>();
            var bodies = new MessageBroker<UriBody>();

            bodies.Send(new UriBody(
                uri: "https://someurlu",
                body: $"<div class='story'><h2><a href='123'>Нужны сильные программисты</a></h2><div class='meta'><div class='date-time'>"
                        +$"3 декабря 2015, 08:00</div></div><div class='text'><p>1999 год</p></div></div>"));

            new ServiceInfoParser(requests, bodies, articles).Run();

            foreach (var i in articles)
            {
                Assert.AreEqual(DateTime.Parse("3 декабря 2015, 08:00"), i.Value.DateCreate);
                Assert.AreEqual("Нужны сильные программисты", i.Value.Head);
                Assert.AreEqual("<p>1999 год</p>", i.Value.Text);
                Assert.IsEmpty(i.Value.Tags);
            }
        }

        [Test]
        public void EmptyTextTest()
        {
            var articles = new Dictionary<string, ArticleInfo>();
            var requests = new MessageBroker<UriRequest>();
            var bodies = new MessageBroker<UriBody>();

            bodies.Send(new UriBody(
                uri: "https://someurl/",
                body: $"<h2><a href='123'>Нужны сильные программисты</a></h2> " +
                      $"<div class='meta'><div class='date-time'> 3 декабря 2015, 08:00</div><div class='tags'><i class='icon-tags'></i>" +
                      $"<ul><li><a href = '/tag/longago' > давным - давно </ a >" +
                      $"</li><li><a href='/tag/only-in-russia'>только в России</a></li></ul></div></div>"));

            new ServiceInfoParser(requests, bodies, articles).Run();

            Assert.IsEmpty(articles);
        }

        [Test]
        public void TrueParceTest()
        {
            var articles = new Dictionary<string, ArticleInfo>();
            var requests = new MessageBroker<UriRequest>();
            var bodies = new MessageBroker<UriBody>();

            bodies.Send(new UriBody(
                uri: "https://someurl",
                body: $"<h1>Нужны сильные программисты</h1> "
                      + $"<div class='meta'><div class='date-time'> 3 декабря 2015, 08:00</div><div class='tags'><i class='icon-tags'></i>"
                      + $"<ul><li><a href = '/tag/longago' > давным - давно </ a >" +
                      $"</li><li><a href='/tag/only-in-russia'>только в России</a></li></ul></div></div><div class='text'><p>1999 год</p> </div>"));

            new ServiceInfoParser(requests, bodies, articles).Run();

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
        public void TruePrevPageTest()
        {
            var articles = new Dictionary<string, ArticleInfo>();
            var requests = new MessageBroker<UriRequest>();
            var bodies = new MessageBroker<UriBody>();

            bodies.Send(new UriBody(
                uri: "https://someurl",
                body: $"<li class='prev'><a href='https://prevurl'></a></li>"));
            new ServiceInfoParser(requests, bodies, articles).Run();
            Assert.IsTrue(requests.TryReceive(out var uri));
            Assert.AreEqual("https://prevurl/", uri.Uri.AbsoluteUri);
            
        }

        [Test]
        public void ContainerTest()
        {
            var articles = new Dictionary<string, ArticleInfo>();
            var rootUri = "https://docs.microsoft.com/ru-ru";
            var container = new InstallClass(new Settings(rootUri)).Container;
            var bodies = (IMessageBroker<UriBody>)container.GetInstance<IMessageBroker>("bodies");
            var requests = (IMessageBroker<UriRequest>)container.GetInstance<IMessageBroker>("requests");
            bodies.Send(new UriBody(
                uri: "https://someurl",
                body: $"<li class='prev'><a href='https://prevurl'></a></li>"));
            container.With(articles).GetInstance<IMessageProcessor<UriBody>>("ServiceInfoParser").Run();
            Assert.IsFalse(bodies.TryReceive(out var _));
            Assert.IsTrue(requests.TryReceive(out var uri));
            Assert.AreEqual("https://prevurl/", uri.Uri.AbsoluteUri);
        }
    }
}