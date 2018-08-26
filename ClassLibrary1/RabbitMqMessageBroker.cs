namespace Mega.Messaging.External
{
    using System;
    using System.Text;
    using System.Threading;

    using Newtonsoft.Json;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class RabbitMqMessageBroker<TMessage> : IMessageBroker<TMessage>, IDisposable
    {
        private readonly IConnection connection;

        private readonly IModel model;

        private readonly string queue_name;

        private readonly Encoding encoding;

        private readonly EventingBasicConsumer consumer;

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
                autoDelete: true,
                arguments: null);

            this.model.QueuePurge(this.queue_name);

            this.consumer = new EventingBasicConsumer(this.model);
        }

        public bool IsEmpty()
        {
            Thread.Sleep(5);
            return this.model.MessageCount(this.queue_name) == 0;
        }

        public void Send(TMessage message)
        {
            var jsonSer = JsonConvert.SerializeObject(message);
            var body = this.encoding.GetBytes(jsonSer);
            this.model.BasicPublish(
                exchange: string.Empty, 
                routingKey: this.queue_name,
                basicProperties: null,
                body: body);
        }

        public bool TryReceive(out TMessage message)
        {
            var i = this.model.BasicGet(this.queue_name, false);
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

        public void ConsumeWith(Action<TMessage> onReceive)
        {
            this.consumer.Received += (model, ea) =>
                {
                    var body = this.encoding.GetString(ea.Body);
                    var message = JsonConvert.DeserializeObject<TMessage>(body);
                    onReceive(message);
                };
            this.model.BasicConsume(queue: this.queue_name, autoAck: true, consumer: this.consumer);
            //onReceive.Invoke();
        }

        public void Dispose()
        {
            this.model?.Dispose();
            this.connection?.Dispose();
        }
    }
}
