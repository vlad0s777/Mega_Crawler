namespace Mega.Messaging.External
{
    using System;
    using System.Text;

    using Newtonsoft.Json;

    using RabbitMQ.Client;

    public class RabbitMqMessageBroker<T> : IMessageBroker<T>, IDisposable
    {
        private readonly IConnection connection;

        private readonly IModel model;

        public RabbitMqMessageBroker()
        {
            var factory = new ConnectionFactory();
            this.connection = factory.CreateConnection();
            this.model = this.connection.CreateModel();
            this.model.QueueDeclare(
                queue: typeof(T).FullName,
                durable: true,
                exclusive: false,
                autoDelete: true,
                arguments: null);
            this.model.QueuePurge(typeof(T).FullName);
        }

        public bool IsEmpty()
        {
            if (this.model.MessageCount(typeof(T).FullName) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Send(T message)
        {
            var jsonSer = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(jsonSer);
            this.model.BasicPublish(
                exchange: string.Empty, 
                routingKey: typeof(T).FullName,
                basicProperties: null,
                body: body);
        }

        public bool TryReceive(out T message)
        {
            var i = this.model.BasicGet(typeof(T).FullName, false);
            if (i != null)
            {
                var body = Encoding.UTF8.GetString(i.Body);
                message = JsonConvert.DeserializeObject<T>(body);
                return true;
            }
            else
            {
                message = default(T);
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
