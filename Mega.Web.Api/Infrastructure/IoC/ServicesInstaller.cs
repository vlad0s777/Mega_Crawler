namespace Mega.Web.Api.Infrastructure.IoC
{
    using Mega.Messaging;
    using Mega.Messaging.External;
    using Mega.Messaging.MessageTypes;

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
