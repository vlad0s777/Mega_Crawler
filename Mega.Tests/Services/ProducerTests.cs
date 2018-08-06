using System;
using System.Collections.Generic;
using Mega.Messaging;
using Mega.Services;
using NUnit.Framework;

namespace Mega.Tests.Services
{
    [TestFixture]
    public class ProducerTests
    {
        [Test]
        public void AddMessage()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<Uri>();
            var pattern = "href\\s*=\\s*(?:[\"'](?<uri>[^\"']*)[\"'])";
            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");
            var body = "href='https://docs.microsoft.com/ru-ru/kenguru'";
            reports.Send(new UriBody(rootUri, body));
            var producer = new Producer(messages, reports, pattern);
            producer.Work();
            Assert.IsTrue(!messages.IsEmpty() && reports.IsEmpty());
        }

        [Test]
        public void ReturnTheSame()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<Uri>();
            var pattern = "href\\s*=\\s*(?:[\"'](?<uri>[^\"']*)[\"'])";
            var producer = new Producer(messages, reports, pattern);
            Assert.AreSame(producer.Reports, reports);
            Assert.AreSame(producer.Messages, messages);
        }
    }
}