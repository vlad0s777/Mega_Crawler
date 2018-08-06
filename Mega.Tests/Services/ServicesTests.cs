using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using NUnit.Framework;
using Mega.Services;

namespace Mega.Tests.Services
{
    [TestFixture]
    public class ServicesTests
    {
        [Test]
        public void ProducerRepTest() //проверка функции RepRecive класса Producer
        {
            //arrange
            Producer prod = new Producer();
            Consumer cons = new Consumer();
            UriBody ur;
            //act
            prod.RepReceive(out ur, cons);
            //assert
            Assert.IsFalse(prod.RepReceive(out ur, cons));
            cons.AddReport(new UriBody(new Uri("http://someurl"), "8"));
            Assert.IsTrue(prod.RepReceive(out ur, cons));
        }

        [Test]
        public void ConsumerTaskTest() //проверка функции TaskReceive класса Consumer
        {
            //arrange
            Producer prod = new Producer();
            Consumer cons = new Consumer();
            Uri ur;
            //act
            cons.TaskReceive(out ur, prod);
            //assert
            Assert.IsFalse(cons.TaskReceive(out ur, prod));
            prod.AddTask(new Uri("http://someurl"));
            Assert.IsTrue(cons.TaskReceive(out ur, prod));
        }

        [Test]
        public void WorkerConsumingTest() //проверка функции Consuming класса Worker
        {
            //arrange
            Worker work = new Worker();
            Producer prod = new Producer();
            Consumer cons = new Consumer();
            Uri ur = new Uri("https://msdn.microsoft.com/ru-ru/library");
            prod.AddTask(ur);
            work.Consuming(cons, prod, new Uri("https://msdn.microsoft.com/ru-ru"));
            Assert.IsTrue(prod.Tasks.isEmpty());
            Assert.IsFalse(cons.Reports.isEmpty());
            UriBody ur2;
            cons.Reports.TryReceive(out ur2);
            Assert.AreSame(ur, ur2.uri);
            Assert.IsTrue(work.visitedUrls.Count > 0);
        }

        [Test]
        public void WorkerProducingTest() //проверка функции Producing класса Worker
        {
            //arrange
            Worker work = new Worker();
            Producer prod = new Producer();
            Consumer cons = new Consumer();
            Uri ur = new Uri("https://msdn.microsoft.com/ru-ru/library");    
            var client = new WebClient();
            var documentBody = client.DownloadString(ur);
            UriBody uri = new UriBody(ur, documentBody);
            cons.AddReport(uri);
            string hrefPattern = "href\\s*=\\s*(?:[\"'](?<uri>[^\"']*)[\"'])";
            work.Producing(cons, prod, hrefPattern);
            Assert.IsFalse(prod.Tasks.isEmpty());
            Assert.IsTrue(cons.Reports.isEmpty());
            Uri ur2;
            Assert.IsFalse(prod.Tasks.isEmpty());
            prod.Tasks.TryReceive(out ur2);
            Assert.IsFalse(prod.Tasks.isEmpty());
        }
    }
}
