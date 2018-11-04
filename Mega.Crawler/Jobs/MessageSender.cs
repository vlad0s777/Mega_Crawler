namespace Mega.Crawler.Jobs
{
    using System;
    using System.Threading.Tasks;

    using Mega.Domain;
    using Mega.Messaging;
    using Mega.Services.UriRequest;

    using Quartz;

    public class MessageSender : IJob
    {
        private readonly IMessageBroker<UriRequest> broker;

        private readonly ISomeReportDataProvider someReportDataProvider;

        public MessageSender(IMessageBroker<UriRequest> broker, ISomeReportDataProvider someReportDataProvider)
        {
            this.broker = broker;
            this.someReportDataProvider = someReportDataProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            //this.broker.Send(await this.someReportDataProvider.CountTags() != 0 ? new UriRequest(string.Empty) : new UriRequest("tags"));
            await Console.Out.WriteLineAsync("Greetings from HelloJob!");
        }
    }
}
