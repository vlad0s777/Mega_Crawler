namespace Mega.Messaging.External
{
    using System;
    using System.Text;

    using Newtonsoft.Json;

    using RabbitMQ.Client;

    public class RabbitMqMessageBroker<TMessage> : IMessageBroker<TMessage>, IDisposable
    {
        private readonly IConnection connection;

        private readonly IModel model;

        private readonly string queue_name;

        private readonly Encoding encoding;

        private readonly IBasicProperties properties;

        public RabbitMqMessageBroker()
        {
            this.queue_name = typeof(TMessage).FullName;

            this.encoding = Encoding.UTF8;

            var factory = new ConnectionFactory();

            this.connection = factory.CreateConnection();

            this.model = this.connection.CreateModel();

            this.model.QueueDeclare(
                queue: this.queue_name,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            this.properties = this.model.CreateBasicProperties();
            this.properties.Persistent = true;
        }

        public bool IsEmpty()
        {
            return this.model.MessageCount(this.queue_name) == 0;
        }

        public void Send(TMessage message)
        {
            var jsonSer = JsonConvert.SerializeObject(message);
            var body = this.encoding.GetBytes(jsonSer);
            this.model.BasicPublish(
                exchange: string.Empty, 
                routingKey: this.queue_name,
                basicProperties: this.properties,
                body: body);
        }

        public bool TryReceive(out TMessage message)
        {
            var i = this.model.BasicGet(this.queue_name, true);
            if (i != null)
            {
                var body = this.encoding.GetString(i.Body);
                message = JsonConvert.DeserializeObject<TMessage>(body);
                return true;
            }
            else
            {
                message = default(TMessage);
                return false;
            }
        }

        public void Dispose()
        {
            this.model?.Dispose();
            this.connection?.Dispose();
        }
    }
}
