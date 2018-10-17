namespace Mega.Web.Api.Infrastructure.IoC
{
    using Mega.Messaging;
    using Mega.Messaging.External;
    using Mega.Services.UriRequest;

    using StructureMap;

    public class ServicesInstaller : Registry
    {
        public ServicesInstaller()
        {
            ForSingletonOf(typeof(IMessageBroker<>)).Use(typeof(RabbitMqMessageBroker<>));

            Forward<IMessageBroker<UriRequest>, IMessageBroker>();
        }
    }
}
