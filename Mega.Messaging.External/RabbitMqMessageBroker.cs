﻿namespace Mega.Messaging.External
{
    using System;
    using System.Text;

    using Newtonsoft.Json;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class RabbitMqMessageBroker<TMessage> : IMessageBroker<TMessage>, IDisposable
    {
        private readonly IConnection connection;

        private readonly IModel model;

        private readonly string queueName;

        private readonly Encoding encoding;

        private readonly IBasicProperties properties;

        private readonly EventingBasicConsumer consumer;

        public RabbitMqMessageBroker()
        {
            this.queueName = typeof(TMessage).FullName;

            this.encoding = Encoding.UTF8;

            var factory = new ConnectionFactory();

            this.connection = factory.CreateConnection();

            this.model = this.connection.CreateModel();

            this.model.QueueDeclare(
                queue: this.queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            this.properties = this.model.CreateBasicProperties();
            this.properties.Persistent = true;
            this.consumer = new EventingBasicConsumer(this.model);
            this.model.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        }

        public bool IsEmpty()
        {
            return this.model.MessageCount(this.queueName) == 0;
        }

        public void Send(TMessage message)
        {
            var jsonSer = JsonConvert.SerializeObject(message);
            var body = this.encoding.GetBytes(jsonSer);
            this.model.BasicPublish(
                exchange: string.Empty, 
                routingKey: this.queueName,
                basicProperties: this.properties,
                body: body);
        }

        public bool TryReceive(out TMessage message)
        {
            var i = this.model.BasicGet(this.queueName, true);
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
                    this.model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
        }

        public void Dispose()
        {
            this.model?.Dispose();
            this.connection?.Dispose();
        }
    }
}
