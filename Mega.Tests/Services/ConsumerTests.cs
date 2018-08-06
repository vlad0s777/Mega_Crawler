using System;
using System.Collections.Generic;
using Mega.Messaging;
using Mega.Services;
using NUnit.Framework;

namespace Mega.Tests.Services
{
    [TestFixture]
    public class ConsumerTests
    {
        [Test]
        public void AddReports()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<Uri>();
            var visitedUrls = new HashSet<Uri>();
            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");
            var consumer = new Consumer(messages, reports, visitedUrls, rootUri);
            consumer.Work();
            Assert.IsTrue(messages.IsEmpty() && !reports.IsEmpty());
        }

        [Test]
        public void ReturnTheSame()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<Uri>();
            var visitedUrls = new HashSet<Uri>();
            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");
            var consumer = new Consumer(messages, reports, visitedUrls, rootUri);
            Assert.AreSame(consumer.Reports, reports);
            Assert.AreSame(consumer.Messages, messages);
        }
    }
}