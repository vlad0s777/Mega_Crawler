using System;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Mega.Messaging;

namespace Mega.Services
{
   
    public class Producer
    {
        public MessageBroker <Uri> Tasks = new MessageBroker <Uri>(); //очередь задач
        public Producer() { }
        public Producer(MessageBroker <Uri> Tasks) => this.Tasks = Tasks; 
        public void Do_task(Uri task) => this.Tasks.Send(task); //добавить задачу в очередь
        public bool rep(out Uri uri, Consumer cons) => cons.Reports.TryReceive(out uri); //убрать отчет в очереди отчетов у исполнителя
    }

    public class Consumer
    {
        public MessageBroker<Uri> Reports = new MessageBroker<Uri>(); //очередь отчетов
        public Consumer() { }
        public Consumer(MessageBroker<Uri> Reports) => this.Reports = Reports;
        public void Do_reports(Uri task) => this.Reports.Send(task); //добавить отчет в очередь
        public bool task(out Uri uri, Producer prod) => prod.Tasks.TryReceive(out uri); //убрать задачу в очереди задач у заказчика
    }

    public class Worker
    {
        public HashSet <Uri> visitedUrls = new HashSet<Uri>();
        public void Consuming(Consumer cons, Producer prod, Uri rootUri) //действия исполнителя
        {
            Uri uri;
            while (cons.task(out uri, prod))
            {
                if (rootUri.IsBaseOf(uri) && visitedUrls.Add(uri))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"OK {uri}");
                    cons.Do_reports(uri);
                }
            }
        }

        public void Producing(Consumer cons, Producer prod, string hrefPattern) //действия заказчика
        {
            Uri uri;
            using (var client = new WebClient())
            {
                while (prod.rep(out uri, cons))
                {
                    try
                    {
                        var documentBody = client.DownloadString(uri);
                        var m = Regex.Match(documentBody, hrefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                        while (m.Success)
                        {
                            try
                            {
                                var absUri = new Uri(uri, new Uri(m.Groups["uri"].Value, UriKind.RelativeOrAbsolute));
                                prod.Do_task(absUri);
                            }
                            catch (Exception)
                            {
                                Console.ResetColor();
                                Console.WriteLine($"Ignoring {m.Value}");
                            }

                            m = m.NextMatch();
                        }
                    }
                    catch (Exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"NO {uri}");
                    }
                }
            }
        }

    }
}
