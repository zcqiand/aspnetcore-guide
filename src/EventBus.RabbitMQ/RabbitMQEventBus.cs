using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EventBus.RabbitMQ
{
    public class RabbitMQEventBus : IEventBus
    {
        private static Dictionary<string, List<object>> eventHandlers = new Dictionary<string, List<object>>();

        private readonly IConnectionFactory connectionFactory;
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string exchangeName;
        private readonly string exchangeType;
        private readonly string queueName;
        private readonly bool autoAck;

        public RabbitMQEventBus(IConnectionFactory connectionFactory,
            string exchangeName,
            string exchangeType = ExchangeType.Fanout,
            string queueName = null,
            bool autoAck = false)
        {
            this.connectionFactory = connectionFactory;
            connection = this.connectionFactory.CreateConnection();
            channel = connection.CreateModel();
            this.exchangeType = exchangeType;
            this.exchangeName = exchangeName;
            this.autoAck = autoAck;

            channel.ExchangeDeclare(this.exchangeName, this.exchangeType);

            this.queueName = InitializeEventConsumer(queueName);
        }

        public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
        {
            var json = JsonConvert.SerializeObject(@event, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            var eventBody = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchangeName,
                @event.GetType().FullName,
                null,
                eventBody);
        }

        public void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : IEvent
        {
            var eventTypeName = typeof(TEvent).FullName;
            if (eventHandlers.ContainsKey(eventTypeName))
            {
                var handlers = eventHandlers[eventTypeName];
                handlers.Add(eventHandler);
            }
            else
            {
                eventHandlers.Add(eventTypeName, new List<object> { eventHandler });
            }
            channel.QueueBind(queueName, exchangeName, typeof(TEvent).FullName);
        }

        public void Unsubscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : IEvent
        {

            var eventType = typeof(TEvent).FullName;
            if (eventHandlers.ContainsKey(eventType))
            {
                var handlers = eventHandlers[eventType];
                if (handlers != null && handlers.Exists(s => s.GetType() == eventHandler.GetType()))
                {
                    var handlerToRemove = handlers.First(s => s.GetType() == eventHandler.GetType());
                    handlers.Remove(handlerToRemove);

                    channel.QueueUnbind(queueName, exchangeName, typeof(TEvent).FullName);
                }
            }
        }

        private string InitializeEventConsumer(string queue)
        {
            var localQueueName = queue;
            if (string.IsNullOrEmpty(localQueueName))
            {
                localQueueName = channel.QueueDeclare().QueueName;
            }
            else
            {
                channel.QueueDeclare(localQueueName, true, false, false, null);
            }

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, eventArgument) =>
            {
                var eventBody = eventArgument.Body.ToArray();
                var json = Encoding.UTF8.GetString(eventBody);
                var @event = (IEvent)JsonConvert.DeserializeObject(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                var eventTypeName = eventArgument.RoutingKey;

                if (eventHandlers.ContainsKey(eventTypeName))
                {
                    var handlers = eventHandlers[eventTypeName];
                    try
                    {
                        foreach (var handler in handlers)
                        {
                            MethodInfo meth = handler.GetType().GetMethod("Handle");
                            meth.Invoke(handler, new object[] { @event });
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                if (!autoAck)
                {
                    channel.BasicAck(eventArgument.DeliveryTag, false);
                }
            };

            channel.BasicConsume(localQueueName, autoAck: autoAck, consumer: consumer);

            return localQueueName;
        }
    }
}
