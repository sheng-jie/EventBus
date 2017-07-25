using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EventBus.EventStore;
using EventBus.Handlers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBus.RabbitMQ
{
    // ReSharper disable once InconsistentNaming
    public class RabbitMQEventBus : IEventBus
    {
        private readonly IEventStore _eventStore;
        public IWindsorContainer IocContainer { get; private set; }
        const string BrokerName = "demo_event_bus";

        private readonly IConnectionFactory _connectionFactory;
        private IModel _channel;
        private string _queueName;

        public RabbitMQEventBus()
        {
            _eventStore = new InMemoryEventStore();
            IocContainer = new WindsorContainer();

            _connectionFactory = new ConnectionFactory() { HostName = "localhost" };
            _channel = CreateConsumerChannel();
        }

        public void Register<TEventData>(IEventHandler eventHandler) where TEventData : IEventData
        {
            Register(typeof(TEventData), eventHandler.GetType());
        }

        public void Register<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            //1.构造ActionEventHandler
            var actionHandler = new ActionEventHandler<TEventData>(action);

            //2.将ActionEventHandler的实例注入到Ioc容器
            IocContainer.Register(
                Component.For<IEventHandler<TEventData>>()
                    .UsingFactoryMethod(() => actionHandler));

            //注册到事件总线
            Register<TEventData>(actionHandler);
        }

        public void Register(Type eventType, Type handler)
        {
            //注册IEventHandler<T>到IOC容器
            var handlerInterface = handler.GetInterface("IEventHandler`1");
            if (!IocContainer.Kernel.HasComponent(handlerInterface))
            {
                IocContainer.Register(
                    Component.For(handlerInterface, handler));
            }
            if (!_eventStore.HasRegisterForEvent(eventType))
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueBind(queue: _queueName, exchange: BrokerName, routingKey: eventType.Name);
                    }
                }
            }
            _eventStore.AddRegister(eventType, handler);
        }

        public void RegisterAllEventHandlerFromAssembly(Assembly assembly)
        {
            //1.将IEventHandler注册到Ioc容器
            IocContainer.Register(Classes.FromAssembly(assembly)
                .BasedOn(typeof(IEventHandler<>))
                .WithService.Base());

            //2.从IOC容器中获取注册的所有IEventHandler
            var handlers = IocContainer.Kernel.GetAssignableHandlers(typeof(IEventHandler));
            foreach (var handler in handlers)
            {
                //循环遍历所有的IEventHandler<T>
                var interfaces = handler.ComponentModel.Implementation.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    if (!typeof(IEventHandler).IsAssignableFrom(@interface))
                    {
                        continue;
                    }

                    //获取泛型参数类型
                    var genericArgs = @interface.GetGenericArguments();
                    if (genericArgs.Length == 1)
                    {
                        //注册到事件源与事件处理的映射字典中
                        Register(genericArgs[0], handler.ComponentModel.Implementation);
                    }
                }
            }
        }

        public void UnRegister<TEventData>(Type handlerType) where TEventData : IEventData
        {
            _eventStore.RemoveRegister(typeof(TEventData), handlerType);
        }

        public void UnRegisterAll<TEventData>() where TEventData : IEventData
        {
            //获取所有映射的EventHandler
            List<Type> handlerTypes = _eventStore.GetHandlersForEvent(typeof(TEventData)).ToList();
            foreach (var handlerType in handlerTypes)
            {
                _eventStore.RemoveRegister(typeof(TEventData), handlerType);
            }
        }

        public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: BrokerName, type: "direct");

                    var message = JsonConvert.SerializeObject(eventData);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: BrokerName,
                        routingKey: eventData.GetType().Name,
                        basicProperties: null,
                        body: body);
                }
            }
        }

        public void Trigger<TEventData>(Type eventHandlerType, TEventData eventData) where TEventData : IEventData
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: BrokerName, type: "direct");

                    var message = JsonConvert.SerializeObject(eventData);
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: BrokerName,
                        routingKey: eventData.GetType().Name,
                        basicProperties: null,
                        body: body);
                }
            }
        }

        public Task TriggerAsync<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            return Task.Run(() => Trigger<TEventData>(eventData));
        }

        public Task TriggerAsycn<TEventData>(Type eventHandlerType, TEventData eventData) where TEventData : IEventData
        {
            return Task.Run(() => Trigger(eventHandlerType, eventData));
        }

        public IModel CreateConsumerChannel()
        {
            var connection = _connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: BrokerName, type: "direct");
            _queueName = channel.QueueDeclare(exclusive: false).QueueName;
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);
                HandleEvent(eventName, message);
            };

            channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return channel;
        }

        private void HandleEvent(string eventName, string message)
        {
            var eventType = _eventStore.GetEventTypeByName(eventName);

            if (eventType != null)
            {
                if (_eventStore.HasRegisterForEvent(eventType))
                {
                    var eventData = JsonConvert.DeserializeObject(message, eventType) as IEventData;
                    var handlerTypes = _eventStore.GetHandlersForEvent(eventType);

                    foreach (var handlerType in handlerTypes)
                    {
                        //获取类型实现的泛型接口
                        var handlerInterface = handlerType.GetInterface("IEventHandler`1");

                        var eventHandlers = IocContainer.ResolveAll(handlerInterface);
                        //循环遍历，仅当解析的实例类型与映射字典中事件处理类型一致时，才触发事件
                        foreach (var eventHandler in eventHandlers)
                        {
                            if (eventHandler.GetType() == handlerType)
                            {
                                var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                                concreteType.GetMethod("HandleEvent").Invoke(eventHandler, new object[] { eventData });
                            }
                        }
                    }


                }
            }
        }
    }
}