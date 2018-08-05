using System;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Mega.Messaging;

namespace Mega.Services
{
    public class UriBody  //класс для пары url и кода страницы
    {
        public Uri uri { get; set; }
        public string body { get; set; }
        public UriBody (Uri uri, string body)
        {
            this.uri = uri;
            this.body = body;
        }
    }
    public class Producer
    {
        public MessageBroker <Uri> Tasks = new MessageBroker <Uri>(); //очередь задач
        public Producer() { }
        public Producer(MessageBroker <Uri> Tasks) => this.Tasks = Tasks; 
        public void Do_task(Uri task) => this.Tasks.Send(task); //добавить задачу в очередь
        public bool rep(out UriBody uri, Consumer cons) => cons.Reports.TryReceive(out uri); //убрать отчет в очереди отчетов у исполнителя
    }

    public class Consumer
    {
        public MessageBroker<UriBody> Reports = new MessageBroker<UriBody>(); //очередь отчетов
        public Consumer() { }
        public Consumer(MessageBroker<UriBody> Reports) => this.Reports = Reports;
        public void Do_reports(UriBody task)  //добавить отчет в очередь
        {
            this.Reports.Send(task);
        } 
        public bool task(out Uri uri, Producer prod) => prod.Tasks.TryReceive(out uri); //убрать задачу в очереди задач у заказчика
    }

    public class Worker
    {
        public HashSet <Uri> visitedUrls = new HashSet<Uri>();
        public void Consuming(Consumer cons, Producer prod, Uri rootUri) //действия исполнителя
        {
            Uri uri;
            using (var client = new WebClient())
            {
                while (cons.task(out uri, prod))
                {
                    if (rootUri.IsBaseOf(uri) && visitedUrls.Add(uri))
                    {
                        var documentBody = client.DownloadString(uri);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"OK {uri}");
                        cons.Do_reports(new UriBody(uri, documentBody));
                    }
                }
            }
        }

        public void Producing(Consumer cons, Producer prod, string hrefPattern) //действия заказчика
        {
            UriBody uri;

            while (prod.rep(out uri, cons))
            {
                try
                {
                    var m = Regex.Match(uri.body, hrefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                    while (m.Success)
                    {
                        try
                        {
                            var absUri = new Uri(uri.uri, new Uri(m.Groups["uri"].Value, UriKind.RelativeOrAbsolute));
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
