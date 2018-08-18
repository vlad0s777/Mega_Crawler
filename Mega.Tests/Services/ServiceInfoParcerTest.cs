namespace Mega.Tests.Services
{
    using System;
    using System.Collections.Generic;

    using Mega.Messaging;
    using Mega.Services;

    using NUnit.Framework;

    [TestFixture]
    internal class ServiceInfoParserTest
    {
        [Test]
        public void EmptyTagsTest()
        {
            var infoDictionary = new Dictionary<string, ArticleInfo>();
            var messages = new MessageBroker<UriLimits>();
            var reports = new MessageBroker<UriBody>();

            reports.Send(new UriBody(
                uri: "https://someurlu",
                body: $"<div class='story'><h2><a href='123'>Нужны сильные программисты</a></h2><div class='meta'><div class='date-time'>"
                        +$"3 декабря 2015, 08:00</div></div><div class='text'><p>1999 год</p></div></div>"));

            new ServiceInfoParser(messages, reports, infoDictionary).Work();

            foreach (var i in infoDictionary)
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
            var infoDictionary = new Dictionary<string, ArticleInfo>();
            var messages = new MessageBroker<UriLimits>();
            var reports = new MessageBroker<UriBody>();

            reports.Send(new UriBody(
                uri: "https://someurl/",
                body: $"<h2><a href='123'>Нужны сильные программисты</a></h2> " +
                      $"<div class='meta'><div class='date-time'> 3 декабря 2015, 08:00</div><div class='tags'><i class='icon-tags'></i>" +
                      $"<ul><li><a href = '/tag/longago' > давным - давно </ a >" +
                      $"</li><li><a href='/tag/only-in-russia'>только в России</a></li></ul></div></div>"));

            new ServiceInfoParser(messages, reports, infoDictionary).Work();

            Assert.IsEmpty(infoDictionary);
        }

        [Test]
        public void TrueParceTest()
        {
            var infoDictionary = new Dictionary<string, ArticleInfo>();
            var messages = new MessageBroker<UriLimits>();
            var reports = new MessageBroker<UriBody>();

            reports.Send(new UriBody(
                uri: "https://someurl",
                body: $"<h1>Нужны сильные программисты</h1> "
                      + $"<div class='meta'><div class='date-time'> 3 декабря 2015, 08:00</div><div class='tags'><i class='icon-tags'></i>"
                      + $"<ul><li><a href = '/tag/longago' > давным - давно </ a >" +
                      $"</li><li><a href='/tag/only-in-russia'>только в России</a></li></ul></div></div><div class='text'><p>1999 год</p> </div>"));

            new ServiceInfoParser(messages, reports, infoDictionary).Work();

            foreach (var i in infoDictionary)
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
            var infoDictionary = new Dictionary<string, ArticleInfo>();
            var messages = new MessageBroker<UriLimits>();
            var reports = new MessageBroker<UriBody>();

            reports.Send(new UriBody(
                uri: "https://someurl",
                body: $"<li class='prev'><a href='https://prevurl'></a></li>"));
            new ServiceInfoParser(messages, reports, infoDictionary).Work();
            Assert.IsTrue(messages.TryReceive(out var uri));
            Assert.AreEqual("https://prevurl/", uri.Uri.AbsoluteUri);
            
        }
    }
}