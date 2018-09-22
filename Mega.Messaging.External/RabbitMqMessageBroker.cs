namespace Mega.Messaging.External
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class RabbitMqMessageBroker<TMessage> : IMessageBroker<TMessage>, IDisposable
    {
        private readonly IConnection connection;

        private readonly IModel model;

        private readonly List<IModel> consumerModels = new List<IModel>();

        private readonly string queueName;

        private readonly Encoding encoding;

        private readonly IBasicProperties properties;

        public RabbitMqMessageBroker()
        {
            this.queueName = typeof(TMessage).FullName;

            this.encoding = Encoding.UTF8;

            var factory = new ConnectionFactory { DispatchConsumersAsync = true };

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
        }

        public bool IsEmpty() => this.model.MessageCount(this.queueName) == 0;

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
            var basicGetResult = this.model.BasicGet(this.queueName, true);
            if (basicGetResult != null)
            {
                var body = this.encoding.GetString(basicGetResult.Body);
                message = JsonConvert.DeserializeObject<TMessage>(body);
                return true;
            }
            else
            {
                message = default(TMessage);
                return false;
            }
        }

        public void ConsumeWith(Func<TMessage, Task> onReceive, CancellationToken token)
        {
            var consumerModel = this.connection.CreateModel();
            this.consumerModels.Add(consumerModel);
            consumerModel.QueueDeclare(
                queue: this.queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            consumerModel.BasicQos(0u, 1, false);
            var consumer = new AsyncEventingBasicConsumer(consumerModel);    
            
            consumer.Received += async (_, ea) =>
                {
                        var body = this.encoding.GetString(ea.Body);
                        var message = JsonConvert.DeserializeObject<TMessage>(body);
                        await onReceive(message);                   
                        consumerModel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
            var tag = model.BasicConsume(queue: this.queueName, autoAck: false, consumer: consumer);

            token.Register(() => consumerModel.BasicCancel(tag));
        }

        public void Dispose()
        {
            this.model?.Dispose();
            foreach (var consumerModel in this.consumerModels)
            {
                consumerModel?.Dispose();
            }

            this.consumerModels?.Clear();
    
            this.connection?.Close();
        }
    }
}
